namespace GOoDkat.PixelLogic.Models;

using System;
using System.Collections.Generic;
using Miscellaneous;
using SixLabors.ImageSharp.PixelFormats;
using Point = SharpDX.Point;

class CircuitBoard
{
    private readonly Image highImage;
    private readonly Image lowImage;

    private CircuitBoard(Image image, Image highImage, Image lowImage)
    {
        WireMap = new Wire[image.Width, image.Height];
        Wires = new List<Wire>();
        Image = image;
        this.highImage = highImage;
        this.lowImage = lowImage;
    }

    public Wire[,] WireMap { get; }

    public ICollection<Wire> Wires { get; }

    public Image Image { get; }

    public void ForceWireAt(int x, int y)
    {
        Wire wire = WireMap[x,y];

        wire?.Force(true);
    }

    public void UnforceWireAt(int x, int y)
    {
        Wire wire = WireMap[x, y];

        wire?.UnForce();
    }

    public void ToggleForceWireAt(int x, int y)
    {
        Wire wire = WireMap[x, y];

        if (wire != null)
        {
            if (wire.IsForced)
            {
                wire.UnForce();
            }
            else
            {
                wire.Force(true);
            }
        }
    }


    public bool Update()
    {
        bool result = false;

        foreach (Wire wire in Wires)
        {
            wire.PrepareUpdate();
        }

        foreach (Wire wire in Wires)
        {
            if (wire.Update())
            {
                Image image = wire.IsActive ? highImage : lowImage;

                foreach (Point point in wire.Points)
                {
                    int x = point.X;
                    int y = point.Y;

                    Image[x, y] = image[x, y];
                }

                result = true;
            }
        }

        return result;
    }

    public static CircuitBoard FromImage(Image image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        int width = image.Width;
        int height = image.Height;

        var highImage = new Image(width, height);
        var lowImage = new Image(width, height);
            
        var board = new CircuitBoard(image, highImage, lowImage);

        if (width == 0 || height == 0)
        {
            return board;
        }

        var map = board.WireMap;

        if (image.IsWire(0, 0))
        {
            var t = new Wire(false, new Point(0, 0));
            map[0, 0] = t;
            board.Wires.Add(t);
        }

        for (int x = 1; x < width; x++)
        {
            if (image.IsWire(x, 0))
            {
                var point = new Point(x, 0);

                Wire wire = map[x - 1, 0];

                if (wire != null)
                {
                    map[x, 0] = wire;
                    wire.Points.Add(point);
                }
                else
                {
                    var t = new Wire(false, point);
                    map[x, 0] = t;
                    board.Wires.Add(t);
                }
            }
        }

        for (int y = 1; y < height; y++)
        {
            if (image.IsWire(0, y))
            {
                var point = new Point(0, y);
                Wire wire = map[0, y - 1];

                if (wire != null)
                {
                    map[0, y] = wire;
                    wire.Points.Add(point);
                }
                else
                {
                    var t = new Wire(false, point);
                    map[0, y] = t;
                    board.Wires.Add(t);
                }
            }

            for (int x = 1; x < width; x++)
            {
                if (image.IsWire(x, y))
                {
                    var point = new Point(x, y);
                    Wire wire = map[x, y - 1];

                    if (wire != null)
                    {
                        map[x, y] = wire;
                        wire.Points.Add(point);
                        var wire2 = map[x - 1, y];

                        if (wire2 != null && wire != wire2)
                        {
                            foreach (Point p in wire2.Points)
                            {
                                map[p.X, p.Y] = wire;
                                wire.Points.Add(p);
                            }

                            board.Wires.Remove(wire2);
                        }
                    }
                    else
                    {
                        wire = map[x - 1, y];

                        if (wire != null)
                        {
                            map[x, y] = wire;
                            wire.Points.Add(point);
                        }
                        else
                        {
                            var t = new Wire(false, point);
                            map[x, y] = t;
                            board.Wires.Add(t);
                        }
                    }
                }
            }
        }

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Wire t1 = map[x - 1, y - 1];
                Wire t2 = map[x, y - 1];
                Wire t3 = map[x + 1, y - 1];
                Wire t4 = map[x - 1, y];
                Wire t5 = map[x, y];
                Wire t6 = map[x + 1, y];
                Wire t7 = map[x - 1, y + 1];
                Wire t8 = map[x, y + 1];
                Wire t9 = map[x + 1, y + 1];

                if (t1 == null && t2 != null && t3 == null && t4 != null && t5 == null && t6 != null && t7 == null && t8 != null && t9 == null)
                {
                    if (t2 != t8)
                    {
                        board.MergeWires(t2, t8);
                    }

                    if (t4 != t6)
                    {
                        board.MergeWires(t4, t6);
                    }
                }
            }
        }

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Wire t1 = map[x - 1, y - 1];
                Wire t2 = map[x, y - 1];
                Wire t3 = map[x + 1, y - 1];
                Wire t4 = map[x - 1, y];
                Wire t5 = map[x, y];
                Wire t6 = map[x + 1, y];
                Wire t7 = map[x - 1, y + 1];
                Wire t8 = map[x, y + 1];
                Wire t9 = map[x + 1, y + 1];

                if (t1 == null && t2 != null && t3 == null && t4 == null && t5 == null && t6 == null && t7 != null && t8 != null && t9 != null)
                {
                    t2.SourceWires.Add(t8);
                }
                else if (t1 == null && t2 == null && t3 != null && t4 != null && t5 == null && t6 != null && t7 == null && t8 == null && t9 != null)
                {
                    t4.SourceWires.Add(t6);
                }
                else if (t1 != null && t2 != null && t3 != null && t4 == null && t5 == null && t6 == null && t7 == null && t8 != null && t9 == null)
                {
                    t8.SourceWires.Add(t2);
                }
                else if (t1 != null && t2 == null && t3 == null && t4 != null && t5 == null && t6 != null && t7 != null && t8 == null && t9 == null)
                {
                    t6.SourceWires.Add(t4);
                }


                //if (t1 == null && t2 != null && t3 == null && t4 != null && t5 == null && t6 != null && t7 != null && t8 != null && t9 != null)
                //{
                //    t2.SourceWires.Add(t8);
                //}
                //else if (t1 == null && t2 != null && t3 != null && t4 != null && t5 == null && t6 != null && t7 == null && t8 != null && t9 != null)
                //{
                //    t4.SourceWires.Add(t6);
                //}
                //else if (t1 != null && t2 != null && t3 != null && t4 != null && t5 == null && t6 != null && t7 == null && t8 != null && t9 == null)
                //{
                //    t8.SourceWires.Add(t2);
                //}
                //else if (t1 != null && t2 != null && t3 == null && t4 != null && t5 == null && t6 != null && t7 != null && t8 != null && t9 == null)
                //{
                //    t6.SourceWires.Add(t4);
                //}
            }
        }

        foreach (Wire wire in board.Wires)
        {
            int active = 0;
            foreach (Point point in wire.Points)
            {
                int x = point.X;
                int y = point.Y;
                Bgra32 color = image[x, y];

                byte r = color.R;
                byte g = color.G;
                byte b = color.B;
                byte a = color.A;

                highImage[x, y] = new Bgra32(r, g, b);
                lowImage[x, y] = new Bgra32((byte)(r / 2), (byte)(g / 2), (byte)(b / 2));

                if (a >= 128)
                {
                    active++;
                }
                else
                {
                    active--;
                }
            }

            wire.IsActive = active > 0;

            Image i = wire.IsActive ? board.highImage : board.lowImage;

            foreach (Point point in wire.Points)
            {
                int x = point.X;
                int y = point.Y;
                image[x, y] = i[x, y];
            }
        }

        return board;
    }

    public Image ToImage()
    {
        Image image = Image.Clone();

        foreach (Wire wire in Wires)
        {
            byte alpha = wire.IsActive ? (byte)255 : (byte)127;

            foreach (Point point in wire.Points)
            {
                int x = point.X;
                int y = point.Y;

                Bgra32 color = highImage[x, y];

                image[x, y] = new Bgra32(color.R, color.G, color.B, alpha);
            }
        }

        return image;
    }

    private void MergeWires(Wire wire1, Wire wire2)
    {
        foreach (Point p in wire2.Points)
        {
            WireMap[p.X, p.Y] = wire1;
            wire1.Points.Add(p);
        }

        Wires.Remove(wire2);
    }
}