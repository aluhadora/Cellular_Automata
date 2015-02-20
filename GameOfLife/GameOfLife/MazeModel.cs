using System;
using System.Drawing;

namespace GameOfLife
{
    public class MazeModel : ModelBase
    {
        private readonly Random _random;

        public MazeModel(IView view)
            : base(view)
        {
            _random = new Random();
        }

        protected override void InsideStep(Color[,] colors, int i, int j)
        {
            var neighbors = NumNeighbors(i, j);
            var state = Chars[i, j];
            if (state == '#')
            {
                if (neighbors < 1 || neighbors > 5)
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


        protected override void InitialColor(Color[,] colors, int j, int i)
        {
            var r = _random.NextDouble();

            if (r < .5)
            {
                colors[i, j] = Color.White;
                Chars[i, j] = 'O';
            }
            else
            {
                colors[i, j] = Color.Black;
                Chars[i, j] = '#';
            }
        }
    }
}
