using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Urinate : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("urinate", Factory, 0);

        private static Operation Factory(Human who, Entity[] what)
        {
            return new Urinate(who);
        }
        #endregion

        #region Data members
        private Human mWho;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Urinate(Human who)
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
            mWho.Stimulate("thirst", timer.TimeDelta);
            mTimeLeft -= timer.TimeDelta;
            if (mTimeLeft <= 0.0f)
            {
                Species species = mWho.World.Encyclopedia.FindSpecies("urine");
                if (species != null)
                {
                    Entity urinate = species.Factory(mWho.World);
                    urinate.Position = mWho.Position;
                    mWho.World.Entities.Add(urinate);
                }
            }
        }

        public override string ToString()
        {
            return "pee";
        }
        #endregion
    }
}
