using System.Linq;
using Caliburn.Micro;
using CQRS.Sample1.Events;

namespace CQRS.Sample1.Client.Domains.Products
{
    public class ProductListHandlers : Shared.IHandle<ProductRenamed>
    {
        // Whenever a ProductRenamed event comes in
        // We need to make sure to mutate the state inside the ReadOnlyStore
        public void Handle(ProductRenamed message)
        {
            var product = ReadOnlyStore.Get<BindableCollection<ProductDto>>();
            product.First(p => p.Id == message.Id).Name = message.NewName;
            ReadOnlyStore.Set(product);

            var productDetail = ReadOnlyStore.GetWithId<ProductDetailDto>(message.Id);
            productDetail.Name = message.NewName;
            ReadOnlyStore.SetWithId(productDetail);
        }
    }
}
