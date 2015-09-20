using System.Drawing;

namespace Ets2Map.Demo
{
    public class DefaultMapPalette : MapPalette
    {
        public DefaultMapPalette()
        {
            Background = Brushes.Black;

            Truck = Brushes.Aqua;

            Highway = Brushes.Red;
            Express = Brushes.Yellow;
            Prefab = Brushes.Yellow;
            Local = Brushes.Orange;

            HighwayGPS = Brushes.DarkRed;
            ExpressGPS = Brushes.GreenYellow;
            PrefabGPS = Brushes.GreenYellow;
            LocalGPS = Brushes.LightCoral;

            Error = Brushes.LightCoral;
        }
    }
}