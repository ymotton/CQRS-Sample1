using CQRS.Sample1.Events;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    public class ProductListCommandHandlers : IHandle<ProductRenaming>
    {
        public void Handle(ProductRenaming message)
        {
            var @event = new ProductRenamed(message.Id, message.NewName);
            ServiceBus.Publish(@event);
        }
    }
}
