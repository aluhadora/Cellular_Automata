using System.Drawing;

namespace ForestFire
{
    public interface IView
    {
        double P { get; set; }
        double F { get; set; }
        long Generation { get; set; }
        Color[,] Colors { get; set; }
        Size FireSize { get; }
    }
}
