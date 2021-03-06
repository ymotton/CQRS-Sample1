﻿using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Sample1.Shared;
using CQRS.Sample1.Events;

namespace CQRS.Sample1.Domain
{
    public class Product : AggregateRoot
    {
        #region Properties

        public string Name { get; private set; }

        #endregion

        #region Ctors

        protected Product(Guid id) : base(id) { }
        public Product(Guid id, IEnumerable<Event> history)
            : base(
                id,
                history,
                history.Any() ? history.Last().Version : 0)
        { }

        #endregion

        protected override void Initialize()
        {
            RegisterHandler((ProductCreated p) => Handle(p));
            RegisterHandler((ProductRenamed p) => Handle(p));
        }
        
        public static Product Create(Guid id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter 'name' cannot be null, empty or whitespace. Fill out a meaningful name!");
            }

            var product = new Product(id);
            product.Apply(new ProductCreated(id, -1, name));
            return product;
        }
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Parameter 'newName' cannot be null, empty or whitespace. Fill out a meaningful name!");
            }
            
            Apply(new ProductRenamed(Id, -1, newName));
        }

        #region Event handlers

        /// <summary>
        /// This method is called by convention. AggregateRoot looks for a Handle method that takes the event as parameter
        /// </summary>
        /// <param name="message"></param>
        private void Handle(ProductCreated message)
        {
            Name = message.Name;
        }
        private void Handle(ProductRenamed message)
        {
            Name = message.NewName;
        }

        #endregion
    }
}
