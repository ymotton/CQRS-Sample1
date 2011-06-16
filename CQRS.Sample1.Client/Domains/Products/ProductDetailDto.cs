using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.Domains.Products
{
    public class ProductDetailDto : IHaveIdentifier
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public int StockCount { get; set; }
        public byte[] RowVersion { get; set; }

        public ProductDetailDto(Guid id, string name, int stockCount, byte[] rowVersion)
        {
            Id = id;
            Name = name;
            StockCount = stockCount;
            RowVersion = rowVersion;
        }
    }
}
