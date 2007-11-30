using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Hit : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("hit", Factory, 1);

        private static Operation Factory(Human who, Entity[] what)
        {
            if (what.Length != 1) throw new ArgumentException("Hit takes a single argument");

            // Don't do it if the target is too far
            if (Vector.Distance(who.Position, what[0].Position) > 0.5f) return null;

            return new Hit(who, what[0]);
        }
        #endregion

        #region Data members
        private Human mWho;
        private Entity mWhat;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Hit(Human who, Entity what)
        {
            mWho = who;
            mWhat = what;
        }
        #endregion

        #region Properties
        public override OperationPrototype Prototype
        {
            get { return new OperationPrototype(Operator, new Species[] { mWhat.Species }); }
        }

        public override bool IsOver
        {
            get { return mTimeLeft <= 0.0f; }
        }
        #endregion

        #region Overriden methods
        public override void Update(Timer timer, Random random)
        {
            mWhat.Integrity -= timer.TimeDelta;
            if (!mWhat.IsAlive) mTimeLeft = 0.0f;
            else mTimeLeft -= timer.TimeDelta;
        }

        public override string ToString()
        {
            return "hit " + mWhat.Name;
        }
        #endregion
    }
}
