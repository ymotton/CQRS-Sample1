namespace CQRS.Sample1.Shared
{
    public interface IHandle<in T>
        where T : Message
    {
        void Handle(T message);
    }
}
