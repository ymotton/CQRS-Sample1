using System.Linq;
using CQRS.Sample1.Events;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process.Domains.Products
{
    public class ProductListEventHandlers : EventHandlerBase<ProductListModel>, IHandle<ProductRenamed>, IHandle<ProductCreated>
    {
        public ProductListEventHandlers(IReadOnlyStore readOnlyStore) : base(readOnlyStore) { }

        public void Handle(ProductRenamed message)
        {
            Model.Products.First(p => p.Id == message.Id).Name = message.NewName;

            DelayedCommit();
        }

        public void Handle(ProductCreated message)
        {
            Model.Products.Add(new ProductDto(message.Id, message.Name));
            Model.ProductDetails.Add(new ProductDetailDto(message.Id, message.Name, 0, message.Version));

            DelayedCommit();
        }
    }
}