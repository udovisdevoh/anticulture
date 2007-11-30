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
            private Entity mEntity;

            public PropertyAccessor(Entity entity)
            {
                mEntity = entity;
            }

            public float this[string propertyName]
            {
                get { return mEntity.GetProperty(propertyName); }
                set { mEntity.SetProperty(propertyName, value); }
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

        // Cute wrapper to GetProperty/SetProperty methods
        public PropertyAccessor Properties
        {
            get { return new PropertyAccessor(this); }
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
        public virtual float GetProperty(string name)
        {
            string lowerCaseName = name.ToLower();
            float value;
            // If it's not defined by the entity, it might be defined by it's species
            if (!mProperties.TryGetValue(lowerCaseName, out value))
                return mSpecies.Properties[lowerCaseName];
            return value;
        }

        public virtual void SetProperty(string name, float value)
        {
            mProperties[name.ToLower()] = value;
        }

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
