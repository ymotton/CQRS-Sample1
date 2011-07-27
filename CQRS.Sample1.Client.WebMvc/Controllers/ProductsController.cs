using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CQRS.Sample1.Commands;
using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.WebMvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductListModel _productListModel;
        public ProductsController()
        {
            Type modelType = typeof(ProductListModel);

            _productListModel = (ProductListModel)ReadOnlyStore.Get(modelType);
            if (_productListModel == null)
            {
                _productListModel = (ProductListModel)Activator.CreateInstance(modelType, null);
                ReadOnlyStore.Put(_productListModel);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts()
        {
            return JsonGridResult(
                _productListModel.Products,
                p =>
                    new[]
                    {
                        p.Id.ToString(),
                        p.Name
                    });
        }

        public JsonResult GetProductDetails(Guid id)
        {
            return JsonGridResult(
                _productListModel.ProductDetails.Where(p => p.Id == id),
                p =>
                    new[]
                    {
                        p.Id.ToString(),
                        p.Name,
                        p.StockCount.ToString()
                    });
        }

        public void AddProduct()
        {
            ServiceBus.Send(new ProductCreation(Guid.NewGuid(), "FooBar"));
        }

        private JsonResult JsonGridResult<T>(IEnumerable<T> items, Func<T, object[]> transform)
            where T : IHaveIdentifier
        {
            return Json(
                new
                {
                    page = 1,
                    total = 1,
                    records = items.Count(),
                    rows = items.Select(i => new { id = i.Id, cell = transform(i) })
                },
                JsonRequestBehavior.AllowGet);
        }
    }
}
