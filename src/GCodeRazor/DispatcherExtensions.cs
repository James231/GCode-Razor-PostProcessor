using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GCodeRazor
{
    public static class DispatcherExtensions
    {
        public static SwitchToUiAwaitable SwitchToUi(this Dispatcher dispatcher)
        {
            return new SwitchToUiAwaitable(dispatcher);
        }

        public struct SwitchToUiAwaitable : INotifyCompletion
        {
            private readonly Dispatcher _dispatcher;

            public SwitchToUiAwaitable(Dispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public SwitchToUiAwaitable GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }

            public bool IsCompleted => _dispatcher.CheckAccess();

            public void OnCompleted(Action continuation)
            {
                _dispatcher.BeginInvoke(continuation);
            }
        }
    }
}
