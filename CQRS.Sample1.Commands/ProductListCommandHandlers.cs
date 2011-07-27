using System.Collections.Generic;
using CQRS.Sample1.Domain;
using CQRS.Sample1.EventStore;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    public class ProductListCommandHandlers : CommandHandlerBase, IHandle<ProductCreation>, IHandle<ProductRenaming>
    {
        public void Handle(ProductCreation message)
        {
            var repository = new Repository<Product>(EventStore);
            var product = Product.Create(message.Id, message.Name);
            repository.Save(product);
        }

        public void Handle(ProductRenaming message)
        {
            var repository = new Repository<Product>(EventStore);
            var product = repository.GetById(message.Id);
            if (product == null)
            {
                throw new KeyNotFoundException(string.Format("An aggregate with id '{0}' cannot be found.", message.Id));
            }
            product.Rename(message.NewName);
            repository.Save(product);
        }
    }
}
