namespace PixelLogic.Models
{
    using System.Collections.Generic;
    using System.Linq;
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

        public bool IsActive { get; private set; }

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
            IsActive = IsForced ? IsForcedActive : SourceWires.Aggregate(false, (w, w2) => w || !w2.WasActive);

            return WasActive != IsActive;
        }
    }
}