using System;
using System.Collections.Generic;
namespace CQRS.Sample1.Client.WebMvc.Models
{
    public class ProductCreation
    {
        public string Name { get; set; }
        public List<Guid> Guids { get; set; }
        public string Test { get; set; }
    }
}