using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VisionPlate
{
    public abstract class DispatchableBase
    {
        public SynchronizationContext InitialContext { get; private set; }

        public DispatchableBase()
        {
            InitialContext = SynchronizationContext.Current;
        }

        protected void InvokeOnInitialThread(Action action)
        {
            if (action == null) return;
            if (InitialContext == null) return;

            InitialContext.Send(o => action(), null);
        }

        protected void InvokeOnInitialThreadAsync(Action action)
        {
            if (action == null) return;
            if (InitialContext == null) return;

            InitialContext.Post(o => action(), null);
        }
    }
}
