using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Sleep : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("sleep", Factory, 0);

        private static Operation Factory(Human who, Entity[] what)
        {
            return new Sleep(who);
        }
        #endregion

        #region Data members
        private Human mWho;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Sleep(Human who)
        {
            mWho = who;
        }
        #endregion

        #region Properties
        public override OperationPrototype Prototype
        {
            get { return new OperationPrototype(Operator, null); }
        }

        public override bool IsOver
        {
            get { return mTimeLeft <= 0.0f; }
        }
        #endregion

        #region Overriden methods
        public override void Update(Timer timer, Random random)
        {
            mWho.Stimulate("sleep", -timer.TimeDelta);
            mTimeLeft -= timer.TimeDelta;
        }

        public override string ToString()
        {
            return "sleep";
        }
        #endregion
    }
}
