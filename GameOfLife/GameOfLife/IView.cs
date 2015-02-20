using System.Drawing;

namespace GameOfLife
{
    public interface IView
    {
        long Generation { get; set; }
        Color[,] Colors { get; set; }
        Size GridSize { get; }
    }
}
