using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GameOfLife
{
    public abstract class ModelBase
    {
        private readonly IView _view;
        protected char[,] Chars;

        protected ModelBase(IView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _view.Generation = 0;

            Chars = new char[_view.GridSize.Width, _view.GridSize.Height];
            _view.Colors = InitialColors();
        }

        protected virtual Color[,] InitialColors()
        {
            var colors = new Color[_view.GridSize.Width, _view.GridSize.Height];
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                for (int i = 0; i < colors.GetLength(0); i++)
                {
                    InitialColor(colors, j, i);
                }
            }

            return colors;
        }

        protected virtual void InitialColor(Color[,] colors, int j, int i)
        {
            Chars[i, j] = 'O';
            colors[i, j] = Color.White;
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
                    InsideStep(colors, i, j);
                }
            }

            _view.Colors = colors;
        }

        protected virtual void InsideStep(Color[,] colors, int i, int j)
        {
            var neighbors = NumNeighbors(i, j);
            var state = Chars[i, j];
            if (state == '#')
            {
                if (neighbors < 2 || neighbors > 3)
                {
                    Chars[i, j] = 'O';
                    colors[i, j] = Color.White;
                }
            }
            else if (state == 'O')
            {
                if (neighbors == 3)
                {
                    Chars[i, j] = '#';
                    colors[i, j] = Color.Black;
                }
            }
        }

        protected int NumNeighbors(int i, int j)
        {
            var count = Living(i - 1, j - 1);
            count += Living(i, j - 1);
            count += Living(i + 1, j - 1);
            count += Living(i - 1, j);
            count += Living(i + 1, j);
            count += Living(i - 1, j + 1);
            count += Living(i, j + 1);
            count += Living(i + 1, j + 1);

            return count;
        }

        private int Living(int i, int j)
        {
            if (i < 0 || j < 0) return 0;
            if (i > Chars.GetLength(0) - 1 || j > Chars.GetLength(1) - 1) return 0;

            return Chars[i, j] == '#' ? 1 : 0;
        }

        public void DrawPoint(Point position, Point form)
        {
            int x = position.X - form.X - 12;
            int y = position.Y - form.Y - 32;
            var state = Chars[x, y];

            var colors = _view.Colors;

            if (state == 'O')
            {
                Chars[x, y] = '#';
                colors[x, y] = Color.Black;
            }
            else if (state == '#')
            {
                Chars[x, y] = 'O';
                colors[x, y] = Color.White;
            }

            _view.Colors = colors;
        }
    }
}
