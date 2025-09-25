using System;
using System.Windows;
using System.Windows.Controls;

namespace CsmLib.Wpf.Controls;

/// <summary>
/// A <see cref="System.Windows.Controls.Primitives.UniformGrid"/> that supports orientation.
/// </summary>
public class UniformGrid : System.Windows.Controls.Primitives.UniformGrid
{
    private int rows;
    private int cols;

    /// <summary>Identifies the <see cref="Orientation"/> property.</summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(UniformGrid),
            new FrameworkPropertyMetadata(
                Orientation.Horizontal,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Identifies the <see cref="FirstRow"/> property.</summary>
    public static readonly DependencyProperty FirstRowProperty =
        DependencyProperty.Register(
            nameof(FirstRow),
            typeof(int),
            typeof(UniformGrid),
            new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.AffectsMeasure),
            validateValueCallback: static o => (int)o >= 0);

    /// <summary>
    /// Gets or sets the value for the dimension in which child content should be arranged.
    /// </summary>
    /// <remarks>
    /// Children fill rows first when <see cref="Orientation"/> is <see cref="Orientation.Horizontal"/> (the default behavior).
    /// Children fill columns first when <see cref="Orientation"/> is <see cref="Orientation.Vertical"/>.
    /// </remarks>
    public Orientation Orientation
    {
        get => (Orientation)this.GetValue(OrientationProperty);
        set => this.SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of leading blank cells in the first column of the grid.
    /// </summary>
    /// <remarks>
    /// This property only takes effect when <see cref="Orientation"/> is <see cref="Orientation.Vertical"/>.
    /// </remarks>
    public int FirstRow
    {
        get => (int)this.GetValue(FirstRowProperty);
        set => this.SetValue(FirstRowProperty, value);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (this.Orientation != Orientation.Vertical)
            return base.MeasureOverride(constraint);

        // NOTE(tad): All logic is repurposed from base class call above to ensure similar behavior.
        this.cols = this.Columns;
        this.rows = this.Rows;

        if (this.FirstRow >= this.rows)
            this.FirstRow = 0;

        if ((this.rows == 0) || (this.cols == 0))
        {
            int nonCollapsedCount = 0;

            for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
            {
                if (this.InternalChildren[i].Visibility != Visibility.Collapsed)
                {
                    nonCollapsedCount++;
                }
            }

            if (nonCollapsedCount == 0)
            {
                nonCollapsedCount = 1;
            }

            if (this.cols == 0)
            {
                if (this.rows > 0)
                {
                    this.cols = (nonCollapsedCount + this.FirstRow + (this.rows - 1)) / this.rows;
                }
                else
                {
                    this.cols = (int)Math.Sqrt(nonCollapsedCount);
                    if ((this.cols * this.cols) < nonCollapsedCount)
                    {
                        this.cols++;
                    }
                    this.rows = this.cols;
                }
            }
            else if (this.rows == 0)
            {
                this.rows = (nonCollapsedCount + (this.cols - 1)) / this.cols;
            }
        }

        Size childConstraint = new(constraint.Width / this.cols, constraint.Height / this.rows);
        double maxChildDesiredWidth = 0.0;
        double maxChildDesiredHeight = 0.0;

        for (int i = 0, count = this.InternalChildren.Count; i < count; ++i)
        {
            var child = this.InternalChildren[i];
            child.Measure(childConstraint);

            Size childDesiredSize = child.DesiredSize;

            if (maxChildDesiredWidth < childDesiredSize.Width)
                maxChildDesiredWidth = childDesiredSize.Width;

            if (maxChildDesiredHeight < childDesiredSize.Height)
                maxChildDesiredHeight = childDesiredSize.Height;
        }

        return new Size(maxChildDesiredWidth * this.cols, maxChildDesiredHeight * this.rows);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        if (this.Orientation != Orientation.Vertical)
            return base.ArrangeOverride(arrangeSize);

        // NOTE(tad): All logic is repurposed from base class call above to ensure similar behavior.
        Rect childBounds = new(0, 0, arrangeSize.Width / this.cols, arrangeSize.Height / this.rows);
        double yStep = childBounds.Height;
        double yBound = arrangeSize.Height - 1.0;

        childBounds.Y += childBounds.Height * this.FirstRow;

        foreach (UIElement child in this.InternalChildren)
        {
            child.Arrange(childBounds);

            if (child.Visibility != Visibility.Collapsed)
            {
                childBounds.Y += yStep;

                if (childBounds.Y >= yBound)
                {
                    childBounds.X += childBounds.Width;
                    childBounds.Y = 0;
                }
            }
        }

        return arrangeSize;
    }
}
