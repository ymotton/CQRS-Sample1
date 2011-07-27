namespace CQRS.Sample1.Process.Domains.Products
{
    public class ProductListModel : NotifyPropertyChanged, IModel
    {
        public string Id { get { return GetType().FullName; } }

        public DispatchedCollection<ProductDto> Products
        {
            get { return GetValue(() => Products); }
            set { SetValue(() => Products, value); }
        }
        public DispatchedCollection<ProductDetailDto> ProductDetails
        {
            get { return GetValue(() => ProductDetails); }
            set { SetValue(() => ProductDetails, value); }
        }

        public ProductListModel()
        {
            Products = new DispatchedCollection<ProductDto>();
            ProductDetails = new DispatchedCollection<ProductDetailDto>();
        }
    }
}
