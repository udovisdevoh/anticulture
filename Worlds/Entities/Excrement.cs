using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Excrement : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Excrement()
        {
            Species = new Species("excrement", "scrunge", Factory);
            Species.Properties["weight"] = 0.25f;
            Species.Properties["nutrition"] = 0.02f;
            Species.Properties["healing"] = -0.4f;
        }

        private static Entity Factory(World world) { return new Excrement(); }
        #endregion

        public Excrement()
            : base(Species)
        {
            Integrity = 1.0f;
        }
    }
}