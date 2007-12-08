using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Push : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("push", Factory, 1);

        private static Operation Factory(Human who, Entity[] what)
        {
            if (what.Length != 1) throw new ArgumentException("Push takes a single argument");

            // Don't push yourself!
            if (who == what[0]) return null;
            // Don't do it if the target is too massive
            if (what[0].Properties["weight"] > 20.0f) return null;
            // Don't do if it's in the guy's inventory
            if (who.Inventory.Contains(what[0])) return null;
            // Don't do it if the target is too far
            if (Vector.Distance(who.Position, what[0].Position) > 0.5f) return null;
            // Don't do it if the entity is anchored (i.e. a plant)
            if (what[0].Properties["anchored"] > 0) return null;

            return new Push(who, what[0]);
        }
        #endregion

        #region Data members
        private Human mWho;
        private Entity mWhat;
        private float mTimeLeft = 0.5f;
        #endregion

        #region Constructor
        public Push(Human who, Entity what)
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
            mTimeLeft -= timer.TimeDelta;
            Vector direction = mWhat.Position - mWho.Position;
            mWhat.Position += direction.Normalize() * timer.TimeDelta;
        }

        public override string ToString()
        {
            return "push " + mWhat.Name;
        }
        #endregion
    }
}
