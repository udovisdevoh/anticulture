using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Urine : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Urine()
        {
            Species = new Species("urine", "titi", Factory);
            Species.Properties["weight"] = float.MaxValue;
            Species.Properties["healing"] = -0.4f;
            Species.Properties["hydration"] = 0.5f;
        }

        private static Entity Factory(World world) { return new Urine(); }
        #endregion

        public Urine()
            : base(Species)
        {
            Integrity = 1.0f;
        }
    }
}