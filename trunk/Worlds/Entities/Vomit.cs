using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Vomit : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Vomit()
        {
            Species = new Species("vomit", "roadrash", Factory);
            Species.Properties["weight"] = float.MaxValue;
            Species.Properties["nutrition"] = 0.05f;
            Species.Properties["healing"] = -0.2f;
            Species.Properties["hydration"] = 0.25f;
        }

        private static Entity Factory(World world) { return new Vomit(); }
        #endregion

        public Vomit()
            : base(Species)
        {
            Integrity = 1.0f;
        }
    }
}