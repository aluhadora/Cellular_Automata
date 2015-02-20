using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace SeaLineSim
{
    public class Model
    {
        private readonly IView _view;
        protected Cell[,] Cells;
        private readonly Random _random;

        public Model(IView view)
        {
            _view = view;
            _random = new Random();
        }

        public void Initialize()
        {
            _view.Generation = 0;

            Cells = new Cell[_view.GridSize.Width, _view.GridSize.Height];
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

            var x = _random.Next(0, colors.GetLength(0));
            var y = _random.Next(0, colors.GetLength(1));

            Cells[x,y].State = State.Land;
            Cells[x, y].Depth = 20;
            colors[x, y] = AddToColor(Color.SandyBrown, -100);

            return colors;
        }

        protected virtual void InitialColor(Color[,] colors, int j, int i)
        {
            Cells[i, j] = new Cell {State = State.Water, Depth = 0, X = i, Y = j};
            colors[i, j] = Color.SkyBlue;
        }

        private Color AddToColor(Color color, int variance)
        {
            var argb = new byte[4];
            argb[0] = color.A;
            argb[1] = color.R;
            argb[2] = color.G;
            argb[3] = color.B;

            argb[1] = (byte)Math.Max(Math.Min(argb[1] + variance, 255), 0);
            argb[2] = (byte)Math.Max(Math.Min(argb[2] + variance, 255), 0);
            argb[3] = (byte)Math.Max(Math.Min(argb[3] + variance, 255), 0);

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
                    InsideStep(colors, i, j);
                }
            }

            _view.Colors = colors;
        }

        protected virtual void InsideStep(Color[,] colors, int i, int j)
        {
            var touchingLand = TouchingLand(i, j);
            var cell = Cells[i, j];
            if (!touchingLand) return;

            var nonEmptyNeighbors = Neighbors(i, j).Where(x => x.State == State.Land).ToList();

            if (cell.State == State.Water)
            {
                if (cell.Depth == -1) return;
                
                if (nonEmptyNeighbors.Any())
                {
                    var averageHeight = nonEmptyNeighbors.Average(x => x.Depth);
                    int height = _random.Next((int)averageHeight - 1 , (int)averageHeight + 1);
                    cell.Depth = height;

                    if (height > 0)
                    {
                        cell.State = State.Land;
                        colors[i, j] = AddToColor(Color.SandyBrown, height*-5);
                    }
                    else
                    {
                        cell.Depth = -1;
                    }
                }
            }
            else
            {
                if (cell.Depth > 1)
                {
                    if (_view.Generation < 100 || nonEmptyNeighbors.Count > 1) return;
                    cell.State = State.Water;
                    colors[i, j] = Color.SkyBlue;
                    cell.Depth = -1;
                }
                else
                {
                    cell.State = State.Water;
                    colors[i, j] = Color.SkyBlue;
                    cell.Depth = -1;
                }

             }
        }

        private IEnumerable<Cell> Neighbors(int i, int j)
        {
            var b = Neighbor(i - 1, j - 1);
            b.AddRange(Neighbor(i, j - 1));
            b.AddRange(Neighbor(i + 1, j - 1));
            b.AddRange(Neighbor(i - 1, j));
            b.AddRange(Neighbor(i + 1, j));
            b.AddRange(Neighbor(i - 1, j + 1));
            b.AddRange(Neighbor(i, j + 1));
            b.AddRange(Neighbor(i + 1, j + 1));

            return b;
        }

        private List<Cell> Neighbor(int i, int j)
        {
            var list = new List<Cell>();

            if (i < 0 || j < 0) return list;
            if (i > Cells.GetLength(0) - 1 || j > Cells.GetLength(1) - 1) return list;

            list.Add(Cells[i, j]);
            return list;
        }

        private bool TouchingLand(int i, int j)
        {
            var b = IsLand(i - 1, j - 1);
            b |= IsLand(i, j - 1);
            b |= IsLand(i + 1, j - 1);
            b |= IsLand(i - 1, j);
            b |= IsLand(i + 1, j);
            b |= IsLand(i - 1, j + 1);
            b |= IsLand(i, j + 1);
            b |= IsLand(i + 1, j + 1);

            return b;
        }

        private bool IsLand(int i, int j)
        {
            if (i < 0 || j < 0) return false;
            if (i > Cells.GetLength(0) - 1 || j > Cells.GetLength(1) - 1) return false;

            return Cells[i, j].State == State.Land;
        }

        public void DrawPoint(Point position, Point form)
        {
            //int x = position.X - form.X - 12;
            //int y = position.Y - form.Y - 32;
            //var state = Cells[x, y];

            //var colors = _view.Colors;

            //if (state == 'O')
            //{
            //    Cells[x, y] = '#';
            //    colors[x, y] = Color.Black;
            //}
            //else if (state == '#')
            //{
            //    Cells[x, y] = 'O';
            //    colors[x, y] = Color.White;
            //}

            //_view.Colors = colors;
        }
    }
}
