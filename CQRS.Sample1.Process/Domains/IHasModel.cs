namespace CQRS.Sample1.Process.Domains
{
    public interface IHasModel
    {
        void Initialize(object model);

        object Model { get; }
    }

    public interface IHasModel<TModel> : IHasModel
        where TModel : IModel
    {
        void Initialize(TModel model);

        TModel Model { get; }
    }
}
