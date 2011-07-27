using CQRS.Sample1.Process;
using CQRS.Sample1.Process.Domains;

namespace CQRS.Sample1.Client
{
    public abstract class ExtendedScreen<TModel> : NotifyPropertyChanged
        where TModel : IModel
    {
        protected ExtendedScreen(TModel model)
        {
            model.PropertyChanged += (o, e) => RaisePropertyChanged(e.PropertyName);
        }
    }
}
