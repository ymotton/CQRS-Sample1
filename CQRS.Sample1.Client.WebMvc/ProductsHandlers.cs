using System;
using System.Linq;
using CQRS.Sample1.Events;
using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.WebMvc
{
    public class ProductsHandlers : IHandle<ProductRenamed>, IHandle<ProductCreated>
    {
        private readonly ProductListModel _productListModel;
        public ProductsHandlers()
        {
            Type modelType = typeof(ProductListModel);

            _productListModel = (ProductListModel)ReadOnlyStore.Get(modelType);
            if (_productListModel == null)
            {
                _productListModel = (ProductListModel)Activator.CreateInstance(modelType, null);
                ReadOnlyStore.Put(_productListModel);
            }
        }

        public void Handle(ProductRenamed message)
        {
            _productListModel.Products.First(p => p.Id == message.Id).Name = message.NewName;

            //if (SelectedProductDetail != null && SelectedProductDetail.Id == message.Id)
            //{
            //    SelectedProductDetail.Name = message.NewName;
            //    SelectedProductDetail.Version = message.Version;
            //    NotifyOfPropertyChange(() => SelectedProductName);
            //}

            ReadOnlyStore.Put(_productListModel);
        }
        public void Handle(ProductCreated message)
        {
            _productListModel.Products.Add(new ProductDto(message.Id, message.Name));
            _productListModel.ProductDetails.Add(new ProductDetailDto(message.Id, message.Name, 0, message.Version));

            ReadOnlyStore.Put(_productListModel);
        }
    }
}