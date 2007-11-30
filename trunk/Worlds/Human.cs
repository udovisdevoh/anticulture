using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public delegate void SimpleFunction();

    public class Human : Entity
    {
        #region Species
        public new static readonly Species Species = new Species("human", "shooray", Factory);

        static Human Factory(World world) { return new Human(world); }
        #endregion

        #region Need subclass
        public class Need
        {
            #region Solution subclass
            public class Solution
            {
                private OperationPrototype mPrototype;
                private float mEffectiveness;

                public Solution(OperationPrototype prototype, float effectiveness)
                {
                    mPrototype = prototype;
                    mEffectiveness = effectiveness;
                }

                public OperationPrototype Prototype
                {
                    get { return mPrototype; }
                }

                public float Effectiveness
                {
                    get { return mEffectiveness; }
                    set { mEffectiveness = value; }
                }
            }
            #endregion

            #region Data members
            private string mName;
            private float mIdeal;
            private float mIntolerence;
            private float mNaturalAlteration;
            // The floats correspond to the enfficiency of the operation
            private List<Solution> mSolutions = new List<Solution>();
            public float mValue;
            #endregion

            #region Constructor
            public Need(string name, float ideal, float intolerence, float naturalAlteration)
            {
                mName = name;
                mIdeal = ideal;
                mIntolerence = intolerence;
                mNaturalAlteration = naturalAlteration;
                mValue = mIdeal;
            }
            #endregion

            #region Properties
            public string Name
            {
                get { return mName; }
            }

            public float Ideal
            {
                get { return mIdeal; }
            }

            public float Intolerence
            {
                get { return mIntolerence; }
            }

            public float NaturalAlteration
            {
                get { return mNaturalAlteration; }
            }

            public List<Solution> Solutions
            {
                get { return mSolutions; }
            }

            public float Value
            {
                get { return mValue; }
                set { mValue = value; }
            }

            public float AbsoluteDepreciation
            {
                get { return Math.Abs(mValue - mIdeal) * mIntolerence; }
            }

            public float Depreciation
            {
                get { return (mValue - mIdeal) * mIntolerence; }
            }

            public bool IsTooLow
            {
                get { return mValue < mIdeal; }
            }

            public bool IsTooHigh
            {
                get { return mValue > mIdeal; }
            }
            #endregion

            #region Methods
            public void Update(float timeDelta)
            {
                mValue += mNaturalAlteration * timeDelta;
            }

            public Solution GetSolution(OperationPrototype prototype)
            {
                foreach (Solution i in mSolutions)
                    if (i.Prototype == prototype)
                        return i;
                return null;
            }
            #endregion
        }
        #endregion

        #region Constants
        public const float Randomness = 0.3f;
        public const float EyeSight = 10.0f;
        public const float LifeSpan = 60.0f * 5.0f; // 5 minutes
        #endregion

        #region Data members
        private World mWorld;
        // The float corresponds to the current value of the stat
        private List<Need> mNeeds = new List<Need>();
        private float mAge = 0.0f;
        private List<Entity> mInventory = new List<Entity>();
        private List<Operation> mRecentOperations = new List<Operation>();
        private Operation mCurrentOperation = null;
        #endregion

        #region Events
        public event OperationEventHandler OperationStarted;
        public event OperationEventHandler OperationEnded;
        #endregion

        #region Constructor
        public Human(World world)
            : base(Species)
        {
            mWorld = world;
            Properties["weight"] = 70.0f;
            Properties["integrity"] = 4.0f;
            mNeeds.Add(new Need("hunger", 0.0f, 1.0f, 0.0125f));
            mNeeds.Add(new Need("sleepiness", 0.0f, 1.0f, 0.025f));
            mNeeds.Add(new Need("integrity", 5.0f, 1.0f, 0.0f));
            mNeeds.Add(new Need("thirst", 0.0f, 1.0f, 0.0125f));
        }
        #endregion

        #region Overriden Properties
        public override float Integrity
        {
            get { return FindNeed("integrity").Value; }
            set
            {
                if (IsAlive)
                {
                    float integrity = (FindNeed("integrity").Value = value);
                    if (integrity <= 0.0f)
                        base.Die();
                }
            }
        }
        #endregion

        #region Properties
        public World World
        {
            get { return mWorld; }
        }

        public List<Need> Needs
        {
            get { return mNeeds; }
        }

        public float Age
        {
            get { return mAge; }
        }

        public List<Entity> Inventory
        {
            get { return mInventory; }
        }

        public Operation CurrentOperation
        {
            get { return mCurrentOperation; }
        }
        #endregion

        #region Overriden methods
        protected override void OnDie()
        {
            // Drop inventory items
            //foreach (Entity entity in mInventory)
            //    mWorld.Entities.Add(entity);
            mInventory.Clear();

            // We spawn a steak
            Species steakSpecies = World.Encyclopedia.FindSpecies("steak");
            if(steakSpecies != null)
            {
                Entity steak = steakSpecies.Factory(World);
                steak.Position = Position;
                mWorld.Entities.Add(steak);
            }
        }

        public override void Update(Timer timer, Random random)
        {
            // Update the inventory
            UpdateInventory();

            // Update vital stats
            foreach (Need i in mNeeds)
                i.Update(timer.TimeDelta);

            // Update age
            mAge += timer.TimeDelta;
            if (mAge > LifeSpan)
            {
                Die();
                return;
            }

            // Find something to do if we haven't
            if (mCurrentOperation == null && random.NextDouble() > Randomness)
            {
                // Get the list of our needs and sort it from the most urgent to the least urgent
                Comparison<Need> needSorter = delegate(Need a, Need b)
                {
                    return b.AbsoluteDepreciation.CompareTo(a.AbsoluteDepreciation);
                };
                mNeeds.Sort(needSorter);

                // Store a list of nearby entities
                List<Entity> nearbyEntities = null;

                foreach (Need need in mNeeds)
                {
                    // There's some chance the need gets skipped
                    if (random.NextDouble() < Randomness) continue;

                    // Internal debbugging commenting function
                    //SimpleFunction PrintMotivation = delegate()
                    //{
                    //    Console.WriteLine(Name + " " + need.Name + " " + (need.IsTooLow ? "too low" : "too high") + ", " + Name + " " + mCurrentOperation.ToString());
                    //};

                    // Get the desirable solutions and sort them according to their efficiency
                    List<Need.Solution> sortedSolutions = new List<Need.Solution>(need.Solutions);
                    Predicate<Need.Solution> remover = delegate(Need.Solution a) { return need.IsTooLow == (a.Effectiveness < 0.0f); };
                    sortedSolutions.RemoveAll(remover);
                    Comparison<Need.Solution> solutionSorter = delegate(Need.Solution a, Need.Solution b)
                    {
                        return Math.Abs(b.Effectiveness).CompareTo(Math.Abs(a.Effectiveness));
                    };
                    sortedSolutions.Sort(solutionSorter);

                    foreach (Need.Solution solution in sortedSolutions)
                    {
                        // There's some chance the solution gets skipped
                        if (random.NextDouble() < Randomness) continue;

                        uint operandCount = solution.Prototype.Operator.OperandCount;
                        if (operandCount == 0)
                        {
                            mCurrentOperation = solution.Prototype.Operator.Factory(this, null);
                            // FIXME : ZOMG a goto!
                            if (mCurrentOperation != null)
                            {
                                //PrintMotivation();
                                goto OperationFound;
                            }
                        }
                        else
                        {
                            // We need operands, retreive nearby entities from world
                            if (nearbyEntities == null)
                            {
                                nearbyEntities = mWorld.GetEntitiesInRadiusSorted(Position, Human.EyeSight);
                                nearbyEntities.Remove(this);
                            }

                            // Act based on operand count
                            if (operandCount == 1)
                            {
                                foreach (Entity entity in nearbyEntities)
                                {
                                    if (entity.Species == solution.Prototype.Operands[0])
                                    {
                                        mCurrentOperation = solution.Prototype.Operator.Factory(this, new Entity[] { entity });
                                        if (mCurrentOperation != null)
                                        {
                                            //PrintMotivation();
                                            goto OperationFound;
                                        }
                                    }
                                }
                            }
                            else if (operandCount == 2)
                            {
                                foreach (Entity entity1 in nearbyEntities)
                                {
                                    foreach (Entity entity2 in nearbyEntities)
                                    {
                                        mCurrentOperation = solution.Prototype.Operator.Factory(this, new Entity[] { entity1, entity2 });
                                        if (mCurrentOperation != null)
                                        {
                                            //PrintMotivation();
                                            goto OperationFound;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            OperationFound:
                if (mCurrentOperation == null)
                {
                    // No solutions were found to our problem.
                    // Let's try doing something random
                    mCurrentOperation = RandomOperation.Operator.Factory(this, null);
                }

                if (mCurrentOperation != null && OperationStarted != null)
                    OperationStarted(this, new OperationEventArgs(mCurrentOperation));
            }

            // Update our current action
            if (mCurrentOperation != null)
            {
                mCurrentOperation.Update(timer, random);
                if (mCurrentOperation.IsOver)
                {
                    Operation operation = mCurrentOperation;
                    mCurrentOperation = null;
                    mRecentOperations.Add(operation);
                    if (OperationEnded != null) OperationEnded(this, new OperationEventArgs(operation));
                }
            }

            // Keep only the last 5 operations
            if (mRecentOperations.Count > 5)
                mRecentOperations.RemoveRange(0, mRecentOperations.Count - 5);
        }
        #endregion

        #region New public methods
        public void Stimulate(string name, float value)
        {
            if(value == 0) return;

            // FIXME : Base this on time elapsed since recent operations?

            Need need = FindNeed(name);
            if (mCurrentOperation != null)
            {
                Need.Solution solution = need.GetSolution(mCurrentOperation.Prototype);
                if (solution == null)
                    need.Solutions.Add(new Need.Solution(mCurrentOperation.Prototype, value));
                else
                    solution.Effectiveness += value;
            }
            for (int i = 0; i < mRecentOperations.Count; ++i)
            {
                OperationPrototype prototype = mRecentOperations[mRecentOperations.Count - i - 1].Prototype;
                Need.Solution solution = need.GetSolution(prototype);
                float effectiveness = value / (float)(2 << i);
                if (solution == null)
                    need.Solutions.Add(new Need.Solution(prototype, effectiveness));
                else
                    solution.Effectiveness += effectiveness;
            }
            if (need != null) need.Value += value;
        }

        public void StimulatePassive(string name, float value)
        {
            Need need = FindNeed(name);
            if (need != null) need.Value += value;
        }
        #endregion

        #region Private methods
        private Need FindNeed(string name)
        {
            foreach (Need i in mNeeds)
            {
                if (i.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;
            }
            return null;
        }

        private void UpdateInventory()
        {
            for (int i = 0; i < mInventory.Count; )
            {
                if (!mInventory[i].IsAlive)
                {
                    // Remove deads
                    mWorld.Entities.Remove(mInventory[i]);
                    mInventory.RemoveAt(i);
                }
                else
                {
                    // Update position
                    mInventory[i].Position = Position;
                    ++i;
                }
            }
        }
        #endregion
    }
}
