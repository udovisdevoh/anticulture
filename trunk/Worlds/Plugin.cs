using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        public abstract void Init(World world);
        public abstract bool Tick();
        public abstract void Shutdown();
    }
}
