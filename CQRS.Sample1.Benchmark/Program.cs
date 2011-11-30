using System;
using System.Diagnostics;
using System.Threading;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Benchmark
{
    class Program: IHandle<ProductRenamed>, IHandle<ProductCreated>
    {
        private object _syncLock = new object();
        private int _iterationsReceived;

        private static Program instance;
        static void Main(string[] args)
        {
            Setup();

            Benchmark(
                10000,
                () => ServiceBus.Send(new ProductCreation(Guid.NewGuid(), "FooBar"))
            );

            Console.ReadKey();
        }

        static void Setup()
        {
            IServiceBus serviceBus = new MsmqServiceBus();
            //IServiceBus serviceBus = new FakeServiceBus();
            IRepository repository = new RavenRepository();

            IoCManager.InjectInstance<IServiceBus>(serviceBus);
            IoCManager.InjectInstance<IEventStore>(new RavenEventStore(repository, serviceBus));
            //IoCManager.InjectInstance<IEventStore>(new FakeEventStore(serviceBus));

            // Command handlers
            var productListCommandHandlers = new ProductListCommandHandlers();
            serviceBus.SubscribeCommandHandler<ProductRenaming>(productListCommandHandlers);
            serviceBus.SubscribeCommandHandler<ProductCreation>(productListCommandHandlers);
            instance = new Program();
            serviceBus.SubscribeEventHandler<ProductRenamed>(instance);
            serviceBus.SubscribeEventHandler<ProductCreated>(instance);
        }
        static void Benchmark(int iterations, Action target)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                target();
            }

            long ms = stopwatch.ElapsedMilliseconds;
            float ratio = 1000f / ms;
            Console.WriteLine("{0} iterations sent to the queue after {1} ms, {2} commands/s...", iterations, ms, iterations * ratio);

            while (instance._iterationsReceived < 1)
            {
                Thread.Sleep(1);
            }
            Console.WriteLine("first result received after {0} ms", stopwatch.ElapsedMilliseconds);

            while (instance._iterationsReceived < iterations)
            {
                Thread.Sleep(1);
            }
            stopwatch.Stop();

            ratio = 1000f/stopwatch.ElapsedMilliseconds;
            Console.WriteLine("{0} iterations/s", iterations * ratio);
        }

        public void Handle(ProductRenamed message)
        {
        }
        public void Handle(ProductCreated message)
        {
            lock (_syncLock)
            {
                _iterationsReceived++;
            } 
        }
    }
}
