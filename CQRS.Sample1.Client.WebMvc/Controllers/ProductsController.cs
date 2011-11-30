using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CQRS.Sample1.Process.Domains.Products;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client.WebMvc.Controllers
{
    public class ProductsController : ControllerBase<ProductListModel>
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts()
        {
            return JsonGridResult(
                Model.Products,
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
                Model.ProductDetails.Where(p => p.Id == id),
                p =>
                    new[]
                    {
                        p.Id.ToString(),
                        p.Name,
                        p.StockCount.ToString()
                    });
        }

        public ActionResult AddProduct()
        {
            TempData["Foo"] = "bar";
            return View(new Models.ProductCreation() { Test = "Test", Guids = new List<Guid> { Guid.NewGuid() }});
        }

        [HttpPost]
        public ActionResult AddProduct(Models.ProductCreation productCreation)
        {
            Console.WriteLine(TempData["Foo"]);
            
            ServiceBus.Send(new Commands.ProductCreation(Guid.NewGuid(), productCreation.Name));

            return RedirectToAction("Index");
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
