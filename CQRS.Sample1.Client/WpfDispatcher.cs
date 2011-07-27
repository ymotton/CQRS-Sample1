using System;
using System.Windows;
using CQRS.Sample1.Process;

namespace CQRS.Sample1.Client
{
    public class WpfDispatcher : IDispatcher
    {
        public void Dispatch(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
