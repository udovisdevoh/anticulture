using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    internal class SimpleEntity : Entity
    {
        #region Data members
        private World mWorld;
        private float mTimeLeft;
        #endregion

        #region Constructors
        public SimpleEntity(SimpleSpecies species, World world, float lifeSpan)
            : base(species)
        {
            mWorld = world;
            mTimeLeft = lifeSpan;
        }

        public SimpleEntity(SimpleSpecies species, World world)
            : this(species, world, -1.0f)
        { }
        #endregion

        #region Overriden methods
        public override void Update(Timer timer, Random random)
        {
            // Negative time left means no life span
            if (mTimeLeft > 0.0f)
            {
                mTimeLeft -= timer.TimeDelta;
                if (mTimeLeft <= 0.0f) Die();
            }
        }

        public override void SetProperty(string name, float value)
        {
            if (name == "integrity" && value <= 0.0f) Die();
            base.SetProperty(name, value);
        }

        protected override void OnDie()
        {
            // Spawn successor, if there's one
            SimpleSpecies species = Species as SimpleSpecies;
            if (species.Successor != null)
            {
                Species successorSpecies = mWorld.Encyclopedia.FindSpecies(species.Successor);
                if(successorSpecies != null)
                {
                    Entity successor = successorSpecies.Factory(mWorld);
                    successor.Position = Position;
                    mWorld.Entities.Add(successor);
                }
            }
        }
        #endregion
    }
}
