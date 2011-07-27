using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Process.Domains.Products
{
    public class ProductDto : NotifyPropertyChanged, IHaveIdentifier
    {
        public Guid Id { get; private set; }

        public string Name
        {
            get { return GetValue(() => Name); }
            set { SetValue(() => Name, value); }
        }

        public ProductDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
