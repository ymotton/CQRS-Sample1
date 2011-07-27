using System;

namespace CQRS.Sample1.Process
{
    public static class DispatcherManager
    {
        private static IDispatcher _current;
        public static IDispatcher Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DefaultDispatcher();
                }
                return _current;
            }
            set { _current = value; }
        }

        public static void OnUIThread(Action action)
        {
            Current.Dispatch(action);
        }
    }

    public class DefaultDispatcher : IDispatcher
    {
        public void Dispatch(Action action)
        {
            action();
        }
    }
}
