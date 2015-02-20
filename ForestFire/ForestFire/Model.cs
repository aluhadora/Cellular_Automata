using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ForestFire
{
  public class Model
  {
    private Random _random;
    private readonly IView _view;
    private char[,] _chars;

    public Model(IView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        _random = new Random();
        //_view.P = 1d;
        _view.P = _random.Next(0, 100) / 1000d;
        _view.F = _random.Next(0, 10) / 10000d;
        _view.Generation = 0;

        _chars = new char[_view.FireSize.Width, _view.FireSize.Height];
        _view.Colors = InitialTrees();
    }

    private Color[,] InitialTrees()
    {
        var colors = new Color[_view.FireSize.Width,_view.FireSize.Height];
        for (int j = 0; j < colors.GetLength(1); j++)
        {
            for (int i = 0; i < colors.GetLength(0); i++)
            {
                var r = _random.Next(0, 100) / 100d;
                var t = r < _view.P;

                if (t)
                {
                    colors[i, j] = Tree();
                    _chars[i, j] = 'T';
                }
                else
                {
                    colors[i, j] = Color.White;
                    _chars[i, j] = 'O';
                }
            }
        }

        return colors;
    }

    private Color Tree()
    {
        return FireTree(Color.ForestGreen, 78);
    }

    private Color Fire()
    {
        return FireTree(Color.Coral, 34);
    }

    private Color FireTree(Color color, int variance)
    {
        var argb = new byte[4];
        argb[0] = color.A;
        argb[1] = color.R;
        argb[2] = color.G;
        argb[3] = color.B;

        argb[1] = (byte)Math.Max(Math.Min(argb[1] + _random.Next(-variance, variance), 255), 0);
        argb[2] = (byte)Math.Max(Math.Min(argb[2] + _random.Next(-variance, variance), 255), 0);
        argb[3] = (byte)Math.Max(Math.Min(argb[3] + _random.Next(-variance, variance), 255), 0);

        return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
    }

    private const PixelFormat Pxf = PixelFormat.Format32bppArgb;

    public Bitmap FromColors(Color[,] colors)
    {
        var bmp = new Bitmap(colors.GetLength(0), colors.GetLength(1));

        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, Pxf);

        IntPtr ptr = bmpData.Scan0;
        int numBytes = bmp.Width * bmp.Height * 4;
        var rgbValues = new byte[numBytes];

        int counter = 0;
        for (int j = 0; j < bmp.Height; j++)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                rgbValues[counter + 3] = colors[i, j].A;
                rgbValues[counter + 2] = colors[i, j].R;
                rgbValues[counter + 1] = colors[i, j].G;
                rgbValues[counter] = colors[i, j].B;
                counter += 4;
            }
        }

        Marshal.Copy(rgbValues, 0, ptr, numBytes);

        bmp.UnlockBits(bmpData);
        return bmp;
    }

    public Color[,] FromImage(Bitmap bmp)
    {
        var colors = new Color[bmp.Width, bmp.Height];

        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, Pxf);

        IntPtr ptr = bmpData.Scan0;
        int numBytes = bmp.Width * bmp.Height * 4;
        var rgbValues = new byte[numBytes];
        Marshal.Copy(ptr, rgbValues, 0, numBytes);

        int counter = 0;
        for (int j = 0; j < bmp.Height; j++)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
              colors[i, j] = Color.FromArgb(rgbValues[counter + 3], rgbValues[counter + 2], rgbValues[counter + 1], rgbValues[counter]);
              counter += 4;
            }
        }

        bmp.UnlockBits(bmpData);
        return colors;
    }

    public void Step()
    {
        ++_view.Generation;
        var colors = _view.Colors;

        for (int j = 0; j < colors.GetLength(1); j++)
        {
            for (int i = 0; i < colors.GetLength(0); i++)
            {
                var state = _chars[i, j];
                if (state == '#')
                {
                    _chars[i, j] = 'O';
                    colors[i, j] = Color.White;
                }
                else if (state == 'O')
                {
                        
                    var r = _random.NextDouble();
                    if (r < _view.P)
                    {
                        _chars[i, j] = 'T';
                        colors[i, j] = Tree();
                    }
                }
                else if (state == 'T')
                {
                    var r = _random.NextDouble();
                    if (r < _view.F || ShouldFire(i , j))
                    {
                        _chars[i, j] = '#';
                        colors[i, j] = Fire();
                    }
                }
            }
        }

        _view.Colors = colors;
    }

    private bool ShouldFire(int i, int j)
    {
        var b = OnFire(i - 1, j - 1);
        b |= OnFire(i, j - 1);
        b |= OnFire(i + 1, j - 1);
        b |= OnFire(i - 1, j);
        b |= OnFire(i + 1, j);
        b |= OnFire(i - 1, j + 1);
        b |= OnFire(i, j + 1);
        b |= OnFire(i + 1, j + 1);

        return b;
    }

    private bool OnFire(int i, int j)
    {
        if (i < 0 || j < 0) return false;
        if (i > _chars.GetLength(0) - 1 || j > _chars.GetLength(1) - 1) return false;

        return _chars[i, j] == '#';
    }
  }
}
