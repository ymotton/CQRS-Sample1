using CQRS.Sample1.EventStore;
using CQRS.Sample1.Shared;
using CQRS.Sample1.Domain;

namespace CQRS.Sample1.Commands
{
    public class ProductListCommandHandlers : IHandle<ProductCreation>, IHandle<ProductRenaming>
    {
        public void Handle(ProductCreation message)
        {
            var repository = new Repository<Product>(new FakeEventStore(IoCManager.Get<IServiceBus>()));
            var product = Product.Create(message.Id, message.Name);
            repository.Save(product);
        }

        public void Handle(ProductRenaming message)
        {
            var repository = new Repository<Product>(new FakeEventStore(IoCManager.Get<IServiceBus>()));
            var product = repository.GetById(message.Id);
            product.Rename(message.NewName);
            repository.Save(product);
        }
    }
}
