using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public class World
    {
        #region Data members
        private Encyclopedia mEncyclopedia; // No encyclopedia by default, user must provide one
        private List<Entity> mEntities = new List<Entity>();
        #endregion

        #region Properties
        public Encyclopedia Encyclopedia
        {
            get { return mEncyclopedia; }
            set { mEncyclopedia = value; }
        }

        public List<Entity> Entities
        {
            get { return mEntities; }
        }
        #endregion

        #region Methods
        public void Update(Timer timer, Random random)
        {
            // Remove dead entities
            for (int i = 0; i < mEntities.Count; )
            {
                if (!mEntities[i].IsAlive)
                    mEntities.RemoveAt(i);
                else
                    ++i;
            }

            // We need a local copy for iterating because the
            // updated entities might modify the entitiy list
            // (by adding or removing entities) while we're
            // iterating through it, and that would be really,
            // really bad (like, boom headshotly bad)
            List<Entity> entities = new List<Entity>(mEntities);

            // Update entities
            foreach (Entity entity in entities)
                entity.Update(timer, random);
        }

        public Entity FindNamed(string name)
        {
            foreach (Entity entity in mEntities)
                if (entity.InstanceName == name)
                    return entity;
            return null;
        }

        public List<Entity> GetEntitiesInRadius(Vector position, float radius)
        {
            List<Entity> entities = new List<Entity>();
            float squaredRadius = radius*radius;
            foreach(Entity entity in mEntities)
                if (Vector.SquaredDistance(entity.Position, position) <= squaredRadius)
                    entities.Add(entity);
            return entities;
        }

        public List<Entity> GetEntitiesInRadiusSorted(Vector position, float radius)
        {
            List<Entity> entities = GetEntitiesInRadius(position, radius);
            Comparison<Entity> sorter = delegate(Entity a, Entity b)
            {
                return Vector.SquaredDistance(a.Position, position).CompareTo(Vector.SquaredDistance(b.Position, position));
            };
            entities.Sort(sorter);
            return entities;
        }
        #endregion
    }
}
