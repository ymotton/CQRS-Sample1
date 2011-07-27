using System;
using System.Linq;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;
using Newtonsoft.Json;

namespace CQRS.Sample1.Client.Domains.Products
{
    public class ProductListViewModel : ExtendedScreen<ProductListModel>, IHandle<ProductRenamed>, IHandle<ProductCreated>
    {
        #region Properties

        private readonly ProductListModel _productListModel;
        public DispatchedCollection<ProductDto> Products
        {
            get { return _productListModel.Products; }
            set { _productListModel.Products = value; }
        }
        public DispatchedCollection<ProductDetailDto> ProductDetails
        {
            get { return _productListModel.ProductDetails; }
            set { _productListModel.ProductDetails = value; }
        }

        public ProductDto SelectedProduct
        {
            get { return GetValue(() => SelectedProduct); }
            set
            {
                SetValue(() => SelectedProduct, value);
                NotifyOfPropertyChange(() => SelectedProductDetail);
                NotifyOfPropertyChange(() => SelectedProductName);
            }
        }
        public ProductDetailDto SelectedProductDetail
        {
            get { return SelectedProduct != null ? ProductDetails.FirstOrDefault(d => d.Id == SelectedProduct.Id) : null; }
        }
        public string SelectedProductName
        {
            get { return SelectedProductDetail != null ? SelectedProductDetail.Name : null; }
            set { _selectedProductName = value; }
        }
        private string _selectedProductName;

        [JsonIgnore]
        public Action<string> SaveProductName
        {
            get
            {
                return
                    (value) =>
                        {
                            if (value != null) ServiceBus.Send(new ProductRenaming(SelectedProductDetail.Id, _selectedProductName, SelectedProductDetail.Version));
                        };
            }
        }

        #endregion 

        #region Ctors

        public ProductListViewModel(ProductListModel model) : base(model)
        {
            _productListModel = model;

            var serviceBus = IoCManager.Get<IServiceBus>();
            serviceBus.SubscribeEventHandler<ProductRenamed>(this);
            serviceBus.SubscribeEventHandler<ProductCreated>(this);
        }

        #endregion

        public void Handle(ProductRenamed message)
        {
            Products.First(p => p.Id == message.Id).Name = message.NewName;
            
            if (SelectedProductDetail != null && SelectedProductDetail.Id == message.Id)
            {
                SelectedProductDetail.Name = message.NewName;
                SelectedProductDetail.Version = message.Version;
                NotifyOfPropertyChange(() => SelectedProductName);
            }

            ReadOnlyStore.Put(_productListModel);
        }
        public void Handle(ProductCreated message)
        {
            Products.Add(new ProductDto(message.Id, message.Name));
            ProductDetails.Add(new ProductDetailDto(message.Id, message.Name, 0, message.Version));

            ReadOnlyStore.Put(_productListModel);
        }
        public void AddProduct()
        {
            ServiceBus.Send(new ProductCreation(Guid.NewGuid(), "FooBar"));
        }
    }
}
