using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public delegate Entity EntityFactory(World world);

    public class Species
    {
        #region PropertyAccessor structure
        public class PropertyAccessor
        {
            private Dictionary<string, float> mProperties;

            public PropertyAccessor(Dictionary<string, float> properties)
            {
                mProperties = properties;
            }

            public float this[string property]
            {
                get
                {
                    string lowerCaseProperty = property.ToLower();
                    float value;
                    // If it's not defined, return 0
                    if (!mProperties.TryGetValue(lowerCaseProperty, out value))
                        return 0.0f;
                    return value;
                }
                set { mProperties[property.ToLower()] = value; }
            }
        };
        #endregion

        #region Data members
        private string mName;
        private string mFictionalName = "shamballah";
        private Dictionary<string, float> mProperties = new Dictionary<string, float>();
        private EntityFactory mFactory;
        #endregion

        #region Constructors
        public Species(string name, EntityFactory factory)
        {
            mName = name.ToLower();
            mFactory = factory;
        }

        public Species(string name, string fictionalName, EntityFactory factory)
            : this(name, factory)
        {
            mFictionalName = fictionalName;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return mName; }
        }

        public string FictionalName
        {
            get { return mFictionalName; }
            set { mFictionalName = value; }
        }

        public PropertyAccessor Properties
        {
            get { return new PropertyAccessor(mProperties); }
        }

        public EntityFactory Factory
        {
            get { return mFactory; }
        }
        #endregion
    }
}
