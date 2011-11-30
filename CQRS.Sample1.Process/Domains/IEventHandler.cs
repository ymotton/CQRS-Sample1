namespace CQRS.Sample1.Process.Domains
{
    public interface IEventHandler
    {
        object Model { get; }
    }

    public interface IEventHandler<TModel> : IEventHandler
        where TModel : IModel
    {
        TModel Model { get; }
    }
}
