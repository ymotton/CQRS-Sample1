namespace CQRS.Sample1.Shared
{
    public interface IRepository
    {
        void Put<T>(T instance);
        T Get<T>(string id);
    }
}
