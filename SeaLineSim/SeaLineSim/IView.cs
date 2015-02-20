using System.Drawing;

namespace SeaLineSim
{
    public interface IView
    {
        long Generation { get; set; }
        Color[,] Colors { get; set; }
        Size GridSize { get; }
    }
}
