using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Pee : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Pee()
        {
            Species = new Species("pee", "titi", Factory);
            Species.Properties["weight"] = float.MaxValue;
            Species.Properties["healing"] = -0.4f;
            Species.Properties["hydration"] = 0.5f;
        }

        private static Entity Factory(World world) { return new Pee(); }
        #endregion

        public Pee()
            : base(Species)
        {
            Integrity = 1.0f;
        }
    }
}