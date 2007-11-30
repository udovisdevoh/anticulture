using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public static class RandomOperation
    {
        // FIXME : Avoid having to create a new random here
        private static readonly Random Random = new Random();
        public static readonly Operator Operator = new Operator("random", Factory, 0);

        private static Operation Factory(Human who, Entity[] what)
        {
            // Shuffle a list of available operators
            List<Operator> operators = new List<Operator>(who.World.Encyclopedia.Operators);
            List<Operator> shuffledOperators = new List<Operator>();
            while (operators.Count != 0)
            {
                int index = Random.Next(operators.Count);
                shuffledOperators.Add(operators[index]);
                operators.RemoveAt(index);
            }

            // List of nearby entities, computed as needed
            List<Entity> nearbyEntities = null;

            // Iterate through operators to find one that works
            foreach (Operator op in shuffledOperators)
            {
                // Skip RandomOperation or we'll have an endless recursive loop
                if (op == Operator) continue;

                // If it needs no operands, try it
                if (op.OperandCount == 0)
                {
                    Operation operation = op.Factory(who, null);
                    if (operation != null) return operation;
                }
                else
                {
                    // We need operands, retreive nearby entities from world
                    if (nearbyEntities == null)
                    {
                        nearbyEntities = who.World.GetEntitiesInRadiusSorted(who.Position, Human.EyeSight);
                        nearbyEntities.Remove(who);
                    }

                    // Act based on operand count
                    if (op.OperandCount == 1)
                    {
                        foreach (Entity entity in nearbyEntities)
                        {
                            Operation operation = op.Factory(who, new Entity[] { entity });
                            if (operation != null) return operation;
                        }
                    }
                    else if (op.OperandCount == 2)
                    {
                        foreach (Entity entity1 in nearbyEntities)
                        {
                            foreach (Entity entity2 in nearbyEntities)
                            {
                                Operation operation = op.Factory(who, new Entity[] { entity1, entity2 });
                                if (operation != null) return operation;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
