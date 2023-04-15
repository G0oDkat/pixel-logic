namespace GOoDkat.PixelLogic.Models;

using System.Collections.Generic;
using SharpDX;

internal class Wire
{
    public Wire(bool isActive, params Point[] points)
    {
        IsActive = isActive;
        Points = new List<Point>(points);
        SourceWires = new List<Wire>();
    }

    public ICollection<Point> Points { get; }

    public ICollection<Wire> SourceWires { get; }

    public bool IsActive { get; set; }

    public bool WasActive { get; private set; }

    public bool IsForced { get; private set; }

    public bool IsForcedActive { get; private set; }

    public void Force(bool isActive)
    {
        IsForced = true;
        IsForcedActive = isActive;
    }

    public void UnForce()
    {
        IsForced = false;
    }

    public void PrepareUpdate()
    {
        WasActive = IsActive;
    }

    public bool Update()
    {
        if (IsForced)
        {
            IsActive = IsForcedActive;
        }
        else
        {
            bool isActive = false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Wire wire in SourceWires)
            {
                isActive = isActive || !wire.WasActive;
            }

            IsActive = isActive;
        }
            
        return WasActive != IsActive;
    }
}