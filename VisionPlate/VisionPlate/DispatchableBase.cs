using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VisionPlate
{
    public abstract class DispatchableBase
    {
        protected SynchronizationContext InitialContext { get; private set; }

        public DispatchableBase()
        {
            InitialContext = SynchronizationContext.Current;
        }

        public void InvokeOnContext(Action action)
        {
            if (action == null) return;

            if (InitialContext != null)
            {
                InitialContext.Send(o => action(), null);
            }
            else
            {
                action();
            }
        }

        public void InvokeOnContextAsync(Action action)
        {
            if (action == null) return;

            if (InitialContext != null)
            {
                InitialContext.Post(o => action(), null);
            }
            else
            {
                action();
            }
        }
    }
}
