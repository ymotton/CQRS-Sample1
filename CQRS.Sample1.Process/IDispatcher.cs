using System;

namespace CQRS.Sample1.Process
{
    public interface IDispatcher
    {
        void Dispatch(Action action);
    }
}
