using CQRS.Sample1.Process.Domains;

namespace CQRS.Sample1.Client
{
    public interface IInitializesViewModel<TModel>
        where TModel : IModel
    {
        void Initialize(TModel model);
    }
}
