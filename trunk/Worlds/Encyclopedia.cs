using System;
using System.Collections.Generic;

namespace AntiCulture.Worlds
{
    public class Encyclopedia
    {
        #region Data members
        private List<Species> mSpecies = new List<Species>();
        private List<Operator> mOperators = new List<Operator>();
        #endregion

        #region Constructor
        public Encyclopedia()
        {
            // Register species
            mSpecies.Add(Human.Species);

            mSpecies.Add(Entities.Steak.Species);
            mSpecies.Add(Entities.Apple.Species);
            mSpecies.Add(Entities.Water.Species);
            mSpecies.Add(Entities.Rock.Species);
            mSpecies.Add(Entities.Tree.Species);
            mSpecies.Add(Entities.Vomit.Species);
            mSpecies.Add(Entities.HealingPlant.Species);
            mSpecies.Add(Entities.Poo.Species);
            mSpecies.Add(Entities.Pee.Species);

            // Register operators
            mOperators.Add(RandomOperation.Operator);

            mOperators.Add(Operations.Go.Operator);
            mOperators.Add(Operations.Eat.Operator);
            mOperators.Add(Operations.Hit.Operator);
            mOperators.Add(Operations.Sleep.Operator);
            mOperators.Add(Operations.Puke.Operator);
            mOperators.Add(Operations.Push.Operator);
            mOperators.Add(Operations.Shit.Operator);
            mOperators.Add(Operations.Piss.Operator);
        }
        #endregion

        #region Properties
        public List<Species> Species
        {
            get { return mSpecies; }
        }

        public List<Operator> Operators
        {
            get { return mOperators; }
        }
        #endregion

        #region Methods
        public Species FindSpecies(string name)
        {
            foreach (Species i in mSpecies)
                if (i.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;
            return null;
        }

        public Operator FindOperator(string name)
        {
            foreach (Operator i in mOperators)
                if (i.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;
            return null;
        }
        #endregion
    }
}
