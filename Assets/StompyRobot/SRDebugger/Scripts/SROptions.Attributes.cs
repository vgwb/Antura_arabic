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

    private float highStrengthThreshold = 50f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float HighStrengthThreshold
    {
        get { return highStrengthThreshold; }
        set { highStrengthThreshold = value; }
    }

    private float highStrengthBruteForce = 100f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float HighStrengthBruteForce
    {
        get { return highStrengthBruteForce; }
        set { highStrengthBruteForce = value; }
    }

    private float highStrengthYDelta = 0.75f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float HighStrengthYDelta
    {
        get { return highStrengthYDelta; }
        set { highStrengthYDelta = value; }
    }

    private float medStrengthThreshold = 30f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float MedStrengthThreshold
    {
        get { return medStrengthThreshold; }
        set { medStrengthThreshold = value; }
    }

    private float medStrengthYDelta = 0.5f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float MedStrengthYDelta
    {
        get { return medStrengthYDelta; }
        set { medStrengthYDelta = value; }
    }

    private float lowStrengthThreshold = 0f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float LowStrengthThreshold
    {
        get { return lowStrengthThreshold; }
        set { lowStrengthThreshold = value; }
    }

    private float lowStrengthYDelta = -10f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float LowStrengthYDelta
    {
        get { return lowStrengthYDelta; }
        set { lowStrengthYDelta = value; }
    }

    private float highStrengthXFactor = 1f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float HighStrengthXFactor
    {
        get { return highStrengthXFactor; }
        set { highStrengthXFactor = value; }
    }

    private float medStrengthXFactor = 0.4f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float MedStrengthXFactor
    {
        get { return medStrengthXFactor; }
        set { medStrengthXFactor = value; }
    }

    private float lowStrengthXFactor = 0.7f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float LowStrengthXFactor
    {
        get { return lowStrengthXFactor; }
        set { lowStrengthXFactor = value; }
    }

    private float touchRecordingYThreshold = 0.1f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float TouchRecordingYThreshold
    {
        get { return touchRecordingYThreshold; }
        set { touchRecordingYThreshold = value; }
    }

    private float targetGravityFactor = 0.33f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float TargetGravityFactor
    {
        get { return targetGravityFactor; }
        set { targetGravityFactor = value; }
    }

    private float yVelocity_Low = 20f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float YVelocity_Low
    {
        get { return yVelocity_Low; }
        set { yVelocity_Low = value; }
    }

    private float yVelocity_Med = 40f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float YVelocity_Med
    {
        get { return yVelocity_Med; }
        set { yVelocity_Med = value; }
    }

    private float yVelocity_High = 60f;

    // Options will be grouped by category
    [Category("ThrowBalls")]
    public float YVelocity_High
    {
        get { return yVelocity_High; }
        set { yVelocity_High = value; }
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
