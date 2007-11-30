using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Piss : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("piss", Factory, 0);

        private static Operation Factory(Human who, Entity[] what)
        {
            return new Piss(who);
        }
        #endregion

        #region Data members
        private Human mWho;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Piss(Human who)
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
                Species species = mWho.World.Encyclopedia.FindSpecies("pee");
                if (species != null)
                {
                    Entity pee = species.Factory(mWho.World);
                    pee.Position = mWho.Position;
                    mWho.World.Entities.Add(pee);
                }
            }
        }

        public override string ToString()
        {
            return "piss";
        }
        #endregion
    }
}
