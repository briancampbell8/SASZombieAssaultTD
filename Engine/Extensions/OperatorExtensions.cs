/*
File:    OperatorExtensions.cs
Purpose:  Extension methods for operator compatibility between different types.
Features:  Type conversion and operator overloads for mismatched types.
*/

using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Extensions
{
    ///<summary>
    ///Extension methods for operator compatibility.
    ///</summary>
    public static class OperatorExtensions
    {
        ///<summary>
        ///Checks if a struct is null-equivalent.
        ///</summary>
        ///<param name="component">The component to check.</param>
        ///<returns>True if component is default/null-equivalent.</returns>
        public static bool IsNull(this TransformComponent component)
        {
            return Equals(component, default(TransformComponent));
        }

        ///<summary>
        ///Checks if a struct is not null-equivalent.
        ///</summary>
        ///<param name="component">The component to check.</param>
        ///<returns>True if component is not default/null-equivalent.</returns>
        public static bool IsNotNull(this TransformComponent component) => !component.IsNull();

        ///<summary>
        ///Converts string to EnemyType for comparison.
        ///</summary>
        ///<param name="str">The string to convert.</param>
        ///<returns>EnemyType value.</returns>
        public static EnemyType ToEnemyType(this string str)
        {
            if (Enum.TryParse<EnemyType>(str, out var enemyType))
                return enemyType;

            return EnemyType.Unknown;
        }

        ///<summary>
        ///Converts Vector3 to Point.
        ///</summary>
        ///<param name="vector">The vector to convert.</param>
        ///<returns>Point value.</returns>
        public static Point ToPoint(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        ///<summary>
        ///Converts Vector3 to PointF.
        ///</summary>
        ///<param name="vector">The vector to convert.</param>
        ///<returns>PointF value.</returns>
        public static PointF ToPointF(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return new PointF(vector.X, vector.Y);
        }

        ///<summary>
        ///Converts Point to Vector3.
        ///</summary>
        ///<param name="point">The point to convert.</param>
        ///<returns>Vector3 value.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 ToVector3(this Point point)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(point.X, point.Y, 0f);
        }

        ///<summary>
        ///Converts PointF to Vector3.
        ///</summary>
        ///<param name="point">The point to convert.</param>
        ///<returns>Vector3 value.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 ToVector3(this PointF point)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(point.X, point.Y, 0f);
        }

        ///<summary>
        ///Subtracts Vector3 from PointF.
        ///</summary>
        ///<param name="point">The PointF.</param>
        ///<param name="vector">The Vector3 to subtract.</param>
        ///<returns>Result PointF.</returns>
        public static PointF Subtract(this PointF point, SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return new PointF(point.X - vector.X, point.Y - vector.Y);
        }

        ///<summary>
        ///Adds Vector3 to PointF.
        ///</summary>
        ///<param name="point">The PointF.</param>
        ///<param name="vector">The Vector3 to add.</param>
        ///<returns>Result PointF.</returns>
        public static PointF Add(this PointF point, SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return new PointF(point.X + vector.X, point.Y + vector.Y);
        }

        ///<summary>
        ///Converts Vector3Int to int (sum of components).
        ///</summary>
        ///<param name="vector">The Vector3Int to convert.</param>
        ///<returns>Sum of components.</returns>
        public static int ToInt(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector)
        {
            return vector.X + vector.Y + vector.Z;
        }

        ///<summary>
        ///Converts int to Vector3Int.
        ///</summary>
        ///<param name="value">The int to convert.</param>
        ///<returns>Vector3Int value.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3Int ToVector3Int(this int value)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3Int(value, value, value);
        }

        ///<summary>
        ///Multiplies Vector3Int by float.
        ///</summary>
        ///<param name="vector">The Vector3Int.</param>
        ///<param name="scalar">The float scalar.</param>
        ///<returns>Result Vector3Int.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3Int Multiply(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector, float scalar)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3Int(
                (int)(vector.X * scalar),
                (int)(vector.Y * scalar),
                (int)(vector.Z * scalar)
            );
        }

        ///<summary>
        ///Adds int to Vector3Int.
        ///</summary>
        ///<param name="vector">The Vector3Int.</param>
        ///<param name="value">The int to add.</param>
        ///<returns>Result Vector3Int.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3Int Add(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector, int value)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3Int(vector.X + value, vector.Y + value, vector.Z + value);
        }

        ///<summary>
        ///Compares int to Vector3Int (sum).
        ///</summary>
        ///<param name="value">The int value.</param>
        ///<param name="vector">The Vector3Int to compare.</param>
        ///<returns>True if int is less than or equal to sum of Vector3Int components.</returns>
        public static bool LessThanOrEqual(this int value, SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector)
        {
            return value <= vector.ToInt();
        }

        ///<summary>
        ///Compares int to Vector3Int (sum).
        ///</summary>
        ///<param name="value">The int value.</param>
        ///<param name="vector">The Vector3Int to compare.</param>
        ///<returns>True if int is less than sum of Vector3Int components.</returns>
        public static bool LessThan(this int value, SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector)
        {
            return value < vector.ToInt();
        }

        ///<summary>
        ///Compares Vector3Int to int (sum).
        ///</summary>
        ///<param name="vector">The Vector3Int to compare.</param>
        ///<param name="value">The int value.</param>
        ///<returns>True if sum of Vector3Int components is less than int.</returns>
        public static bool LessThan(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector, int value)
        {
            return vector.ToInt() < value;
        }

        ///<summary>
        ///Compares Vector3Int to int (sum).
        ///</summary>
        ///<param name="vector">The Vector3Int to compare.</param>
        ///<param name="value">The int value.</param>
        ///<returns>True if sum of Vector3Int components is greater than int.</returns>
        public static bool GreaterThan(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int vector, int value)
        {
            return vector.ToInt() > value;
        }

        ///<summary>
        ///Compares AssetType values.
        ///</summary>
        ///<param name="type1">First AssetType.</param>
        ///<param name="type2">Second AssetType.</param>
        ///<returns>True if equal.</returns>
        public static bool EqualsAssetType(this AssetType type1, AssetType type2)
        {
            return type1.ToString() == type2.ToString();
        }

        ///<summary>
        ///Converts nullable int to Dictionary of string, int with default.
        ///</summary>
        ///<param name="value">The nullable int.</param>
        ///<param name="defaultValue">Default dictionary.</param>
        ///<returns>Dictionary value.</returns>
        public static Dictionary<string, int> ToDictionary(this int? value, Dictionary<string, int> defaultValue)
        {
            if (value.HasValue)
                return new Dictionary<string, int> { ["value"] = value.Value };

            return defaultValue ?? new Dictionary<string, int>();
        }

        ///<summary>
        ///Converts nullable int to List of int with default.
        ///</summary>
        ///<param name="value">The nullable int.</param>
        ///<param name="defaultValue">Default list.</param>
        ///<returns>List value.</returns>
        public static List<int> ToList(this int? value, List<int> defaultValue)
        {
            if (value.HasValue)
                return new List<int> { value.Value };

            return defaultValue ?? new List<int>();
        }

        ///<summary>
        ///Converts string to EngineState.
        ///</summary>
        ///<param name="str">The string to convert.</param>
        ///<returns>EngineState value.</returns>
        public static EngineState ToEngineState(this string str)
        {
            if (Enum.TryParse<EngineState>(str, out var state))
                return state;

            return EngineState.Unknown;
        }

        ///<summary>
        ///Converts Rectangle? to Vector3 with default.
        ///</summary>
        ///<param name="rect">The nullable rectangle.</param>
        ///<param name="defaultValue">Default vector.</param>
        ///<returns>Vector3 value.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 ToVector3(this Rectangle? rect, SASZombieAssaultTD.Engine.VectorMath.Vector3 defaultValue)
        {
            if (rect.HasValue)
                return new SASZombieAssaultTD.Engine.VectorMath.Vector3(rect.Value.X, rect.Value.Y, 0f);

            return defaultValue;
        }

        ///<summary>
        ///Converts Vector3Int? to int with default.
        ///</summary>
        ///<param name="vector">The nullable Vector3Int.</param>
        ///<param name="defaultValue">Default int.</param>
        ///<returns>Int value.</returns>
        public static int ToInt(this SASZombieAssaultTD.Engine.VectorMath.Vector3Int? vector, int defaultValue)
        {
            if (vector.HasValue)
                return vector.Value.ToInt();

            return defaultValue;
        }
    }

    ///<summary>
    ///Enemy type enumeration.
    ///</summary>
    public enum EnemyType
    {
        Unknown,
        Zombie,
        FastZombie,
        HeavyZombie,
        SpecialZombie
    }

    ///<summary>
    ///Asset type enumeration.
    ///</summary>
    ///
    //NOTE: AssetType already defined in Engine/Resources/Manager/AssetType.cs
    //public enum AssetType
    //{
    //   Unknown,
    //   Texture,
    //   Sound,
    //   Model,
    //   Font
    //}

    ///<summary>
    ///Engine state enumeration.
    ///</summary>
    public enum EngineState
    {
        Unknown,
        Initializing,
        Running,
        Paused,
        ShuttingDown
    }
}
