using System.Drawing;

namespace Ets2Map.Demo
{
    public class SimpleMapPalette : MapPalette
    {
        public SimpleMapPalette()
        {
            Background = Brushes.Black;

            Truck = Brushes.Aqua;

            Highway = Brushes.Yellow;
            Express = Brushes.Yellow;
            Prefab = Brushes.Yellow;
            Local = Brushes.Yellow;

            HighwayGPS = Brushes.OrangeRed;
            ExpressGPS = Brushes.OrangeRed;
            PrefabGPS = Brushes.OrangeRed;
            LocalGPS = Brushes.OrangeRed;

            Error = Brushes.LightCoral;
        }
    }
}