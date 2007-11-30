using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds.Entities
{
    public class Apple : Entity
    {
        #region Species stuff
        public new static readonly Species Species;

        static Apple()
        {
            Species = new Species("apple", "pumbaa", Factory);
            Species.Properties["weight"] = 1.0f;
            Species.Properties["nutrition"] = 0.6f;
            Species.Properties["hydration"] = 0.1f;
        }

        private static Entity Factory(World world) { return new Apple(); }
        #endregion

        public Apple()
            : base(Species)
        {
            Integrity = 3.0f;
        }
    }
}
