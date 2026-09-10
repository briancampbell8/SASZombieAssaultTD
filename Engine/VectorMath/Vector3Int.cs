// ====================================================================================================
//  FILE: Vector3Int.cs
//  PATH: ./Engine/VectorMath/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Vector3Int module.
//
//  RESPONSIBILITIES:
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide FloorToInt() behavior for the Core subsystem.
//      - Provide ToVector3() behavior for the Core subsystem.
//      - Provide Distance() behavior for the Core subsystem.
//      - Provide SqrDistance() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Dictionary;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    ///<summary>
    ///Integer-based 3D vector for grid coordinates and discrete positions.
    ///Used for tile-based positioning and grid calculations.
    ///</summary>
    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;

        //Properties for case compatibility
        public int X => x;
        public int Y => y;
        public int Z => z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        ///<summary>
        ///2D constructor for grid positions (z defaults to 0)
        ///</summary>
        public Vector3Int(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static readonly Vector3Int Zero = new(0, 0, 0);
        public static readonly Vector3Int One = new(1, 1, 1);
        public static readonly Vector3Int Up = new(0, 1, 0);
        public static readonly Vector3Int Down = new(0, -1, 0);
        public static readonly Vector3Int Left = new(-1, 0, 0);
        public static readonly Vector3Int Right = new(1, 0, 0);
        public static readonly Vector3Int Forward = new(0, 0, 1);
        public static readonly Vector3Int Back = new(0, 0, -1);

        public static Vector3Int operator +(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3Int operator -(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3Int operator *(Vector3Int a, int b)
        {
            return new Vector3Int(a.x * b, a.y * b, a.z * b);
        }

        public static bool operator ==(Vector3Int a, Vector3Int b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3Int a, Vector3Int b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Int other && this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        ///<summary>
        ///Convert from Vector3 to Vector3Int (floor conversion).
        ///</summary>
        public static Vector3Int FloorToInt(Vector3 v)
        {
            return new Vector3Int((int)v.X, (int)v.Y, (int)v.Z);
        }

        ///<summary>
        ///Convert from Vector3Int to Vector3.
        ///</summary>
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        ///<summary>
        ///Get the magnitude squared.
        ///</summary>
        public int SqrMagnitude => x * x + y * y + z * z;

        ///<summary>
        ///Get the magnitude as float.
        ///</summary>
        public float Magnitude => (float)System.Math.Sqrt(SqrMagnitude);

        ///<summary>
        ///Get distance to another Vector3Int.
        ///</summary>
        public float Distance(Vector3Int other)
        {
            return (this - other).Magnitude;
        }

        ///<summary>
        ///Get squared distance to another Vector3Int.
        ///</summary>
        public int SqrDistance(Vector3Int other)
        {
            return (this - other).SqrMagnitude;
        }
    }
}

