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
    public class ProductListViewModel : ExtendedScreen<ProductListModel>, IHandle<ProductRenamed>
    {
        #region Properties

        public DispatchedCollection<ProductDto> Products
        {
            get { return Model.Products; }
            set { Model.Products = value; }
        }
        public DispatchedCollection<ProductDetailDto> ProductDetails
        {
            get { return Model.ProductDetails; }
            set { Model.ProductDetails = value; }
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

        public void Handle(ProductRenamed message)
        {
            if (SelectedProductDetail != null && SelectedProductDetail.Id == message.Id)
            {
                SelectedProductDetail.Name = message.NewName;
                SelectedProductDetail.Version = message.Version;
                NotifyOfPropertyChange(() => SelectedProductName);
            }
        }
        public void AddProduct()
        {
            ServiceBus.Send(new ProductCreation(Guid.NewGuid(), "FooBar"));
        }
    }
}
