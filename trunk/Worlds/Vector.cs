using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public struct Vector
    {
        #region Data members
        public float X, Y;
        #endregion

        #region Constructors
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector(Vector original)
        {
            X = original.X;
            Y = original.Y;
        }
        #endregion

        #region Properties
        public float Magnitude
        {
            get { return (float)Math.Sqrt((double)(X * X + Y * Y)); }
        }

        public float SquaredMagnitude
        {
            get { return X * X + Y * Y; }
        }

        public float this[int index]
        {
            get { return index == 0 ? X : Y; }
            set
            {
                if (index == 0) X = value;
                else Y = value;
            }
        }
        #endregion

        #region Setter
        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Operations
        public Vector Normalize()
        {
            float mag = Magnitude;
            if(mag > 0.01f) return this / Magnitude;
            return this;
        }
        #endregion

        #region Static operations
        public static float Distance(Vector v1, Vector v2)
        {
            return (v1 - v2).Magnitude;
        }

        public static float SquaredDistance(Vector v1, Vector v2)
        {
            return (v1 - v2).SquaredMagnitude;
        }
        #endregion

        #region Operators
        public static Vector operator -(Vector vec)
        {
            return new Vector(-vec.X, -vec.Y);
        }

        public static Vector operator + (Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector operator *(Vector lhs, float rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vector operator /(Vector lhs, float rhs)
        {
            return new Vector(lhs.X / rhs, lhs.Y / rhs);
        }

        public static bool operator ==(Vector lhs, Vector rhs)
        {
            return (lhs.X == rhs.X) && (lhs.Y == rhs.Y);
        }

        public static bool operator !=(Vector lhs, Vector rhs)
        {
            return (lhs.X != rhs.X) || (lhs.Y != rhs.Y);
        }
        #endregion

        #region Object overrides
        public override bool Equals(object obj)
        {
            return this == (Vector)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
