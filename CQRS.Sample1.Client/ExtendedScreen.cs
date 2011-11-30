using System;
using System.Linq;
using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Client
{
    public abstract class ExtendedScreen<TModel> : NotifyPropertyChanged, IHasModel<TModel>
        where TModel : IModel
    {
        protected ExtendedScreen()
        {
            var serviceBus = IoCManager.Get<IServiceBus>();
            
            Type type = GetType();
            Type serviceBusType = serviceBus.GetType();
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IHandle<>))
                {
                    serviceBusType.GetMethod("SubscribeEventHandler")
                                  .MakeGenericMethod(@interface.GetGenericArguments().First())
                                  .Invoke(serviceBus, new[] {this});
                }
            }
        }

        #region IHasModel{TModel} Members

        public TModel Model { get; protected set; }

        public void Initialize(TModel model)
        {
            Model = model;
            Model.PropertyChanged += (o, e) => RaisePropertyChanged(e.PropertyName);
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
