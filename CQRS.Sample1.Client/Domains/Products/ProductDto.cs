using System;
using Caliburn.Micro;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.Domains.Products
{
    public class ProductDto : PropertyChangedBase, IHaveIdentifier
    {
        public Guid Id { get; private set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyOfPropertyChange(() => Name); }
        }

        public ProductDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
