using System;
using System.ComponentModel;

public partial class SROptions
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NumberRangeAttribute : Attribute
    {
        public readonly double Max;
        public readonly double Min;

        public NumberRangeAttribute(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IncrementAttribute : Attribute
    {
        public readonly double Increment;

        public IncrementAttribute(double increment)
        {
            Increment = increment;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class SortAttribute : Attribute
    {
        public readonly int SortPriority;

        public SortAttribute(int priority)
        {
            SortPriority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public readonly string Name;

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }

    [Category("MakeFriends")]
    public bool MakeFriendsUseDifficulty { get; set; }

    [Category("MakeFriends")]
    public EA4S.MakeFriends.MakeFriendsVariation MakeFriendsDifficulty { get; set; }

    private bool showProjection = false;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public bool ShowProjection
    {
        get { return showProjection; }
        set { showProjection = value; }
    }

    private float elasticity = 4.5f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float Elasticity
    {
        get { return elasticity; }
        set { elasticity = value; }
    }
}

#if NETFX_CORE

namespace System.ComponentModel
{

	[AttributeUsage(AttributeTargets.All)]
	public sealed class CategoryAttribute : Attribute
	{

		public readonly string Category;

		public CategoryAttribute(string category)
		{
			Category = category;
		}

	}

}

#endif
