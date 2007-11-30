using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public struct OperationPrototype
    {
        public Operator Operator;
        public Species[] Operands;

        public OperationPrototype(Operator op, Species[] operands)
        {
            Operator = op;
            Operands = operands;
        }

        public override bool Equals(object other)
        {
            return other.GetType() == this.GetType() && this == (OperationPrototype)other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(OperationPrototype lhs, OperationPrototype rhs)
        {
            if ((lhs.Operands == null) ^ (rhs.Operands == null)) return false;
            if (lhs.Operator == rhs.Operator)
            {
                if (lhs.Operands == null && rhs.Operands == null) return true;
                return lhs.Operands.Equals(rhs.Operands);
            }
            return false;
        }

        public static bool operator !=(OperationPrototype lhs, OperationPrototype rhs)
        {
            return !(lhs == rhs);
        }
    }
}
