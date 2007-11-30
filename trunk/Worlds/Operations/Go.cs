using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Go : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("go", Factory, 1);

        private static Operation Factory(Human who, Entity[] what)
        {
            if (what.Length != 1) throw new ArgumentException("Go takes a single argument");

            // Don't it if the target is near
            if (Vector.Distance(who.Position, what[0].Position) <= 0.5f) return null;

            return new Go(who, what[0]);
        }
        #endregion

        #region Data members
        private Human mWho;
        private Entity mWhat;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        private Go(Human who, Entity what)
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
            Vector direction = mWhat.Position - mWho.Position;

            if (direction.Magnitude <= 0.5f)
            {
                mTimeLeft = 0.0f;
            }
            else
            {
                mWho.Position += direction.Normalize() * timer.TimeDelta;
                mTimeLeft -= timer.TimeDelta;
            }
        }

        public override string ToString()
        {
            return "go to " + mWhat.Name;
        }
        #endregion
    }
}
