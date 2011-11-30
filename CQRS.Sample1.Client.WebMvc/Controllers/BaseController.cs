using System.Web.Mvc;
using CQRS.Sample1.Process.Domains;

namespace CQRS.Sample1.Client.WebMvc.Controllers
{
    public abstract class ControllerBase<TModel> : Controller, IHasModel<TModel>
        where TModel : IModel
    {
        #region IHasModel{TModel} Members

        public TModel Model { get; protected set; }
    
        public void Initialize(TModel model)
        {
            Model = model;
        }

        #endregion

        #region IHasModel Members

        object IHasModel.Model { get { return Model; } }

        void IHasModel.Initialize(object model)
        {
            Initialize((TModel)model);
        }

        #endregion
    }
}
