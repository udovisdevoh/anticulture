using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public abstract class Operation
    {
        public abstract OperationPrototype Prototype { get; }

        public abstract bool IsOver { get; }

        public abstract void Update(Timer timer, Random random);
    }

    public class OperationEventArgs : EventArgs
    {
        private Operation mOperation;

        public OperationEventArgs(Operation operation)
        {
            mOperation = operation;
        }

        public Operation Operation
        {
            get { return mOperation; }
        }
    }

    public delegate void OperationEventHandler(object sender, OperationEventArgs e);
}
