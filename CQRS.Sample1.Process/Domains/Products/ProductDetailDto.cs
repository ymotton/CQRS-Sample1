using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process.Domains.Products
{
    public class ProductDetailDto : NotifyPropertyChanged, IHaveIdentifier
    {
        public Guid Id { get; private set; }

        public string Name
        {
            get { return GetValue(() => Name); }
            set { SetValue(() => Name, value); }
        }
        public int StockCount
        {
            get { return GetValue(() => StockCount); }
            set { SetValue(() => StockCount, value); }
        }
        public int Version
        {
            get { return GetValue(() => Version); }
            set { SetValue(() => Version, value); }
        }

        public ProductDetailDto(Guid id, string name, int stockCount, int version)
        {
            Id = id;
            Name = name;
            StockCount = stockCount;
            Version = version;
        }
    }
}
