using System;
using System.Collections.Generic;

namespace AntiCulture.Worlds
{
    public delegate Operation OperationFactory(Human who, Entity[] what);

    public class Operator
    {
        #region Data members
        private string mName;
        private string mFictionnalName = "munk";
        private OperationFactory mFactory;
        private uint mOperandCount;
        #endregion

        #region Constructors
        public Operator(string name, OperationFactory factory, uint operandCount)
        {
            mName = name;
            mFactory = factory;
            mOperandCount = operandCount;
        }

        public Operator(string name, string fictionalName, OperationFactory factory, uint operandCount)
        {
            mName = name;
            mFictionnalName = fictionalName;
            mFactory = factory;
            mOperandCount = operandCount;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return mName; }
        }

        public string FictionnalName
        {
            get { return mFictionnalName; }
        }

        public OperationFactory Factory
        {
            get { return mFactory; }
        }

        public uint OperandCount
        {
            get { return mOperandCount; }
        }
        #endregion
    }
}
