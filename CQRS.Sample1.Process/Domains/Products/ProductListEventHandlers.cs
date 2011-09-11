using System;
using System.Linq;
using CQRS.Sample1.Events;
using CQRS.Sample1.Process;
using CQRS.Sample1.Shared;
using System.Threading;

namespace CQRS.Sample1.Process.Domains.Products
{
    public class ProductListEventHandlers : IHandle<ProductRenamed>, IHandle<ProductCreated>
    {
        private TimeSpan _commitTimeout;
        private DateTime _commitStartTime;

        private readonly ProductListModel _productListModel;
        public ProductListEventHandlers(TimeSpan? commitTimeout = null)
        {
            if (!commitTimeout.HasValue)
            {
                commitTimeout = TimeSpan.FromSeconds(10);
            }
            _commitTimeout = commitTimeout.Value;

            Type modelType = typeof(ProductListModel);

            _productListModel = (ProductListModel)ReadOnlyStore.Get(modelType);
            if (_productListModel == null)
            {
                _productListModel = (ProductListModel)Activator.CreateInstance(modelType, null);

                Rebuild(_productListModel);

                ReadOnlyStore.Put(_productListModel);
            }
        }

        private void Rebuild(ProductListModel productListModel)
        {
            var eventStore = IoCManager.Get<IEventStore>();

            foreach (Event @event in eventStore.GetAllEvents())
            {
                this.AsDynamic().Handle(@event);
            }
        }

        private void ResetCommitTimer()
        {
            _commitStartTime = DateTime.Now + _commitTimeout;
        }
        private void DelayedCommit()
        {
            if (DateTime.Now < _commitStartTime)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    ResetCommitTimer();

                    while (DateTime.Now < _commitStartTime)
                    {
                        Thread.Sleep(1);
                    }

                    ReadOnlyStore.Put(_productListModel);
                });
        }

        public void Handle(ProductRenamed message)
        {
            _productListModel.Products.First(p => p.Id == message.Id).Name = message.NewName;

            DelayedCommit();
        }
        public void Handle(ProductCreated message)
        {
            _productListModel.Products.Add(new ProductDto(message.Id, message.Name));
            _productListModel.ProductDetails.Add(new ProductDetailDto(message.Id, message.Name, 0, message.Version));

            DelayedCommit();
        }
    }
}