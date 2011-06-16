using System.Linq;
using Caliburn.Micro;
using CQRS.Sample1.Events;

namespace CQRS.Sample1.Client.Domains.Products
{
    public class ProductListHandlers : Shared.IHandle<ProductRenamed>, Shared.IHandle<ProductCreated>
    {
        // Whenever a ProductRenamed event comes in
        // We need to make sure to mutate the state inside the ReadOnlyStore
        public void Handle(ProductRenamed message)
        {
            var products = ReadOnlyStore.Get<BindableCollection<ProductDto>>();
            products.First(p => p.Id == message.Id).Name = message.NewName;
            ReadOnlyStore.Set(products);

            var productDetail = ReadOnlyStore.GetWithId<ProductDetailDto>(message.Id);
            productDetail.Name = message.NewName;
            ReadOnlyStore.SetWithId(productDetail);
        }

        public void Handle(ProductCreated message)
        {
            var products = ReadOnlyStore.Get<BindableCollection<ProductDto>>();
            products.Add(new ProductDto(message.Id, message.Name));
            ReadOnlyStore.Set(products);

            var productDetail = new ProductDetailDto(message.Id, message.Name, 0, null);
            ReadOnlyStore.SetWithId(productDetail);
        }
    }
}
