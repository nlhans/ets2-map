using System.Drawing;

namespace Ets2Map
{
    public class MapPalette
    {
        /// <summary>
        /// Background of map
        /// </summary>
        public Brush Background;

        /// <summary>
        /// Brush of truck drawn
        /// </summary>
        public Brush Truck;

        /// <summary>
        /// Highway roads
        /// </summary>
        public Brush Highway;

        /// <summary>
        /// Roads in cities
        /// </summary>
        public Brush Express;

        /// <summary>
        /// Local B-roads
        /// </summary>
        public Brush Local;

        /// <summary>
        /// Prefab roads (prefabs are crosspoints, etc.)
        /// </summary>
        public Brush Prefab;

        /// <summary>
        /// Color for highways that are in GPS
        /// </summary>
        public Brush HighwayGPS;

        /// <summary>
        /// Color for express roads (cities) that are in GPS
        /// </summary>
        public Brush ExpressGPS;

        /// <summary>
        /// Color for B-roads that are in GPS
        /// </summary>
        public Brush LocalGPS;

        /// <summary>
        /// Color for prefabs that are in GPS
        /// </summary>
        public Brush PrefabGPS;

        /// <summary>
        /// Brush for error text/roads
        /// </summary>
        public Brush Error;
    }
}