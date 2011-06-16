using System;
using System.Linq;
using Caliburn.Micro;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Events;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.Domains.Products
{
    public interface IProductListViewModel
    {
        BindableCollection<ProductDto> Products { get; set; }
        ProductDetailDto GetProductDetails(Guid productId);
        ProductDto SelectedProduct { get; set; }
        ProductDetailDto SelectedProductDetail { get; }
    }
    public class ProductListViewModel : ExtendedScreen, IProductListViewModel, Shared.IHandle<ProductRenamed>
    {
        #region Properties

        private ProductDto _selectedProduct;
        public ProductDto SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => SelectedProductDetail);
                NotifyOfPropertyChange(() => SelectedProductName);
            }
        }

        public ProductDetailDto SelectedProductDetail
        {
            get { return SelectedProduct != null ? GetProductDetails(SelectedProduct.Id) : null; }
        }
        public string SelectedProductName
        {
            get { return GetValue(() => SelectedProductDetail.Name); }
            set { _selectedProductName = value; }
        }
        private string _selectedProductName;

        public Action<string> SaveProductName
        {
            get
            {
                return
                    (value) =>
                        {
                            if (value != null) ServiceBus.Send(new ProductRenaming(SelectedProductDetail.Id, _selectedProductName));
                        };
            }
        }

        #endregion 

        #region Ctors

        public ProductListViewModel() : this(null){ }
        public ProductListViewModel(BindableCollection<ProductDto> products)
        {
            Products = products;
            
            var serviceBus = IoCManager.Get<IServiceBus>();
            serviceBus.SubscribeEventHandler(this);
        }

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();

            BindableCollection<ProductDto> products;
            if (ReadOnlyStore.TryGet(out products))
            {
                Products = products;
            }
        }

        public void Handle(ProductRenamed message)
        {
            Products.First(p => p.Id == message.Id).Name = message.NewName;
            
            if (SelectedProductDetail != null && SelectedProductDetail.Id == message.Id)
            {
                SelectedProductDetail.Name = message.NewName;
                NotifyOfPropertyChange(() => SelectedProductName);
            }
        }

        #region IProductListViewModel Members

        private BindableCollection<ProductDto> _products;
        public BindableCollection<ProductDto> Products
        {
            get { return _products; }
            set { _products = value; NotifyOfPropertyChange(() => Products); }
        }

        public ProductDetailDto GetProductDetails(Guid productId)
        {
            return ReadOnlyStore.GetWithId<ProductDetailDto>(productId);
        }

        #endregion
    }
}
