using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public abstract class Entity
    {
        #region PropertyAccessor structure
        public class PropertyAccessor
        {
            private Dictionary<string, float> mEntityProperties;
            private Species mSpecies;

            public PropertyAccessor(Dictionary<string, float> entityProperties, Species species)
            {
                mEntityProperties = entityProperties;
                mSpecies = species;
            }

            public float this[string property]
            {
                get
                {
                    string lowerCaseProperty = property.ToLower();
                    float value;
                    // If it's not defined by the entity, it might be defined by it's species
                    if (!mEntityProperties.TryGetValue(lowerCaseProperty, out value))
                        return mSpecies.Properties[lowerCaseProperty];
                    return value;
                }
                set { mEntityProperties[property.ToLower()] = value; }
            }
        };
        #endregion

        #region Data members
        private string mInstanceName = null;
        private Species mSpecies;
        private Vector mPosition = new Vector(0, 0);
        private Dictionary<string, float> mProperties = new Dictionary<string, float>();
        private float mIntegrity = 1.0f;
        #endregion

        #region Events
        public event EventHandler Dying;
        #endregion

        #region Constructor
        public Entity(Species species)
        {
            mSpecies = species;
        }
        #endregion

        #region Properties
        public string InstanceName
        {
            get { return mInstanceName; }
            set { mInstanceName = value; }
        }

        public string Name
        {
            get
            {
                if (mInstanceName != null) return mInstanceName;
                return Species.Name + " #" + base.GetHashCode().ToString();
            }
        }
        
        public Species Species
        {
            get { return mSpecies; }
            protected set { mSpecies = value; }
        }

        public virtual Vector Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public PropertyAccessor Properties
        {
            get { return new PropertyAccessor(mProperties, mSpecies); }
        }

        public virtual float Integrity
        {
            get { return mIntegrity; }
            set
            {
                if (IsAlive)
                {
                    mIntegrity = value;
                    if (mIntegrity <= 0.0f)
                        Die();
                }
            }
        }

        public bool IsAlive
        {
            get { return mIntegrity > 0.0f; }
        }
        #endregion

        #region Methods
        public virtual void Die()
        {
            if (!IsAlive) return;
            if (Dying != null) Dying(this, new EventArgs());
            OnDie();
            mIntegrity = 0.0f;
        }

        protected virtual void OnDie() { }
        public virtual void Update(Timer timer, Random random) { }
        #endregion
    }
}
