using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Rock : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Rock()
        {
            Species = new Species("rock", "catl", Factory);
            Species.Properties["weight"] = 4.0f;
            Species.Properties["healing"] = -0.5f;
        }

        private static Entity Factory(World world) { return new Rock(); }
        #endregion

        public Rock()
            : base(Species)
        {
            Integrity = 6.0f;
        }
    }
}
