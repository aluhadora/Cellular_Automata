using System.Drawing;

namespace GameOfLife
{
    public class GameOfLifeModel : ModelBase
    {
        public GameOfLifeModel(IView view) : base(view)
        {
        }

        protected override void InsideStep(Color[,] colors, int i, int j)
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
    }
}
