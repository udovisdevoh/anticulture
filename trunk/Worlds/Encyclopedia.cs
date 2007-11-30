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
