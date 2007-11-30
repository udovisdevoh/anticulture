using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Eat : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("eat", Factory, 1);

        private static Operation Factory(Human who, Entity[] what)
        {
            if (what.Length != 1) throw new ArgumentException("Eat takes a single argument");

            // Don't do it if the target is too far
            if (Vector.Distance(who.Position, what[0].Position) > 0.5f) return null;
            // Disable intraspecies cannibalism
            if (((Entity)who).Species == what[0].Species) return null;

            return new Eat(who, what[0]);
        }
        #endregion

        #region Data members
        private Human mWho;
        private Entity mWhat;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Eat(Human who, Entity what)
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
            // Negative tastiness makes more hunger
            float nutrition = mWhat.Properties["nutrition"];
            if (nutrition != 0) mWho.Stimulate("hunger", -nutrition * timer.TimeDelta);
            float healing = mWhat.Properties["healing"];
            if (healing != 0) mWho.Stimulate("integrity", healing * timer.TimeDelta);
            float hydration = mWhat.Properties["hydration"];
            if (hydration != 0) mWho.Stimulate("thirst", hydration * timer.TimeDelta);
            mWhat.Integrity -= timer.TimeDelta;
            if (!mWhat.IsAlive) mTimeLeft = 0.0f;
            else mTimeLeft -= timer.TimeDelta;
        }

        public override string ToString()
        {
            //When mWhat is more liquid than solid, we say "drink" instead of "eat"
            if (mWhat.Properties["hydration"] > mWhat.Properties["nutrition"])
            {
                return "drink " + mWhat.Name;
            }
            else
            {
                return "eat " + mWhat.Name;
            }
        }
        #endregion
    }
}
