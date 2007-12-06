using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AntiCulture.Worlds
{
    public class SimpleSpecies : Species
    {
        #region Data Members
        private float mInitialIntegrity = 1.0f;
        private float mLifeSpan = -1.0f;
        private string mSuccessor = null;
        #endregion

        #region Constructor
        public SimpleSpecies(string name)
            : base(name)
        {
            base.Factory = CreateEntity;
        }
        #endregion

        #region Properties
        public float InitialIntegrity
        {
            get { return mInitialIntegrity; }
            set { mInitialIntegrity = value; }
        }

        public float LifeSpan
        {
            get { return mLifeSpan; }
            set { mLifeSpan = value; }
        }

        public string Successor
        {
            get { return mSuccessor; }
            set { mSuccessor = value; }
        }
        #endregion

        #region Private methods
        private Entity CreateEntity(World world)
        {
            SimpleEntity entity = new SimpleEntity(this, world, mLifeSpan);
            entity.Properties["integrity"] = mInitialIntegrity;
            return entity;
        }
        #endregion

        #region Static methods
        public static SimpleSpecies FromFile(string path)
        {
            int dot = path.LastIndexOf('.');
            if (dot == -1) dot = path.Length - 1;
            int slash = path.IndexOfAny("/\\".ToCharArray());
            if(slash == -1) slash = 0;
            SimpleSpecies species = new SimpleSpecies(path.Substring(slash+1, dot-slash-1));
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            TextReader reader = new StreamReader(stream);
            string contents = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();
            string[] lines = contents.Split('\n');
            foreach (string line in lines)
            {
                string[] operands = line.Split('=');
                if (operands.Length == 2)
                {
                    string property = operands[0].Trim();
                    string value = operands[1].Trim();

                    if (property.Equals("successor", StringComparison.CurrentCultureIgnoreCase)) species.mSuccessor = value;
                    else
                    {
                        float floatValue;
                        try
                        {
                            floatValue = float.Parse(value);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (property.Equals("lifespan", StringComparison.CurrentCultureIgnoreCase)) species.mLifeSpan = floatValue;
                        else if (property.Equals("integrity", StringComparison.CurrentCultureIgnoreCase)) species.mInitialIntegrity = floatValue;
                        else species.Properties[property] = floatValue;
                    }
                }
            }
            return species;
        }
        #endregion
    }
}
