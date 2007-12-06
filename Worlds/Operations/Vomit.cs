using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Operations
{
    public class Vomit : Operation
    {
        #region Static operator
        public static readonly Operator Operator = new Operator("vomit", Factory, 0);

        private static Operation Factory(Human who, Entity[] what)
        {
            return new Vomit(who);
        }
        #endregion

        #region Data members
        private Human mWho;
        private float mTimeLeft = 1.0f;
        #endregion

        #region Constructor
        public Vomit(Human who)
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
            mWho.Stimulate("food", timer.TimeDelta);//Increase need for food
            mWho.Stimulate("health", timer.TimeDelta * 0.25f);//Increase need for better health

            mTimeLeft -= timer.TimeDelta;
            if (mTimeLeft <= 0.0f)
            {
                Species species = mWho.World.Encyclopedia.FindSpecies("vomit");
                if (species != null)
                {
                    Entity vomit = species.Factory(mWho.World);
                    vomit.Position = mWho.Position;
                    mWho.World.Entities.Add(vomit);
                }
            }
        }

        public override string ToString()
        {
            return "puke";
        }
        #endregion
    }
}
