using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ets2Map
{
    public class MapRenderer
    {
        public Ets2Mapper Map { get; private set; }
        public MapPalette Palette { get; set; }
        public Ets2NavigationRoute Route { get; private set; }

        private Rectangle _clip;

        private float totalX;
        private float totalZ;

        private float centerX;
        private float centerZ;

        public MapRenderer(Ets2Mapper map, MapPalette palette)
        {
            Map = map;
            Palette = palette;
        }

        public void Render(Graphics g, Rectangle clip, float baseScale, Ets2Point point)
        {
            _clip = clip;

            g.FillRectangle(Palette.Background, new Rectangle(0,0,clip.X+clip.Width, clip.Y+clip.Height));
            g.SmoothingMode = baseScale < 1000 ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;

            var errorFont = new Font("Arial", 10.0f, FontStyle.Bold);

            if (Map == null)
            {
                g.DrawString("Map object not initialized", errorFont, Palette.Error, 5, 5);
                return;
            }
            if (Palette == null)
            {
                g.DrawString("Palette object not initialized", errorFont, Palette.Error, 5, 5);
                return;
            }
            if (Map.Nodes == null)
            {
                g.DrawString("Map has not started parsing", errorFont, Palette.Error, 5, 5);
                return;
            }
            if (Map.Loading)
            {
                g.DrawString("Map has not completed parsing", errorFont, Palette.Error, 5, 5);
                return;
            }
            if (!Map.Nodes.Any())
            {
                g.DrawString("Map has no data", errorFont, Palette.Error, 5, 5);
                return;
            }

            centerX = point.X;
            centerZ = point.Z;

            if (clip.Width > clip.Height)
            {
                totalX = baseScale;
                totalZ = (int)(baseScale * (float)clip.Height / clip.Width);
            }
            else
            {
                totalZ = baseScale;
                totalX = (int)(baseScale * (float)clip.Width / clip.Height);
            }

            var startX = clip.X + centerX - totalX;
            var endX = clip.X + centerX + totalX;
            var startZ = clip.Y + centerZ - totalZ;
            var endZ = clip.Y + centerZ + totalZ;

            var scaleX = clip.Width / (endX - startX);
            var scaleZ = clip.Height / (endZ - startZ);

            if (float.IsInfinity(scaleX) || float.IsNaN(scaleX))
                scaleX = clip.Width;
            if (float.IsInfinity(scaleZ) || float.IsNaN(scaleZ))
                scaleZ = clip.Height;

            var nodesNearby =
                Map.Nodes.Values.Where(
                    x => x.X >= startX - 1500 && x.X <= endX + 1500 && x.Z >= startZ - 1500 && x.Z <= endZ + 1500);
            var itemsNearby = nodesNearby.SelectMany(x => x.GetItems()).Where(x => x.HideUI == false).ToList();

            var prefabs = itemsNearby.Where(x => x.Type == Ets2ItemType.Prefab);

            var nodesToFollow = prefabs.SelectMany(x => x.NodesList.Values).Distinct();

            // Gather all prefabs, and issue a drawing command
            foreach (var node in nodesToFollow)
            {
                if (node == null)
                    continue;

                // Nodes from prefab are always like:
                // Prefab = Forward
                // Road=backward
                var road = node.ForwardItem != null && node.ForwardItem.Type == Ets2ItemType.Prefab
                    ? node.BackwardItem
                    : node.ForwardItem;
                var roadStart = road;
                var fw = node.ForwardItem != null && node.ForwardItem.Type == Ets2ItemType.Road;

                if (road == null)
                {
                    // DEAD END
                    continue;
                }

                var roadChain = new List<Ets2Item>();

                // Start drawing at start road
                if (fw)
                {
                    do
                    {
                        roadChain.Add(road);
                        road = road.EndNode == null ? null : road.EndNode.ForwardItem;
                    } while (road != null && road.Type == Ets2ItemType.Road);
                }
                else
                {
                    do
                    {
                        roadChain.Add(road);
                        road = road.StartNode == null ? null : road.StartNode.BackwardItem;
                    } while (road != null && road.Type == Ets2ItemType.Road);
                }

                if (!fw)
                    roadChain.Reverse();

                foreach (var n in roadChain.Where(x => x.HideUI == false))
                {
                    n.GenerateRoadPolygon(64);
                }

                // Generate drawing parameters
                var isHighway = roadStart != null && roadStart.RoadLook != null && roadStart.RoadLook.IsHighway;
                var isExpress = roadStart != null && roadStart.RoadLook != null && roadStart.RoadLook.IsExpress;
                var isLocal = roadStart != null && roadStart.RoadLook != null && roadStart.RoadLook.IsLocal;
                var roadWidth = (roadStart.RoadLook != null ? roadStart.RoadLook.GetTotalWidth() : 10.0f) * scaleX;
                var roadInGps = Route != null && !Route.Loading && Route.Roads != null && Route.Roads.Contains(roadStart);

                var pen = default(Pen);

                if (isHighway)
                {
                    pen = new Pen(roadInGps ? Palette.HighwayGPS : Palette.Highway, roadWidth);
                }
                else if (isExpress)
                {
                    pen = new Pen(roadInGps ? Palette.ExpressGPS : Palette.Express, roadWidth);
                }
                else if (isLocal)
                {
                    pen = new Pen(roadInGps ? Palette.LocalGPS : Palette.Local, roadWidth);
                }
                else
                {
                    pen = new Pen(Palette.Error, roadWidth);
                }


                var roadPoly =
                    roadChain.Where(x => x.HideUI == false)
                        .SelectMany(x => x.RoadPolygons)
                        .Select(x => new PointF((x.X - startX) * scaleX, (x.Z - startZ) * scaleZ));

                if (roadPoly.Any())
                {
                    g.DrawLines(pen, roadPoly.ToArray());
                }
            }

            // Cities?
            var cityFont = new Font("Arial", 10.0f);
            foreach (var cities in itemsNearby.Where(x => x.Type == Ets2ItemType.City && x.StartNode != null))
            {
                var ctX = cities.StartNode.X;
                var ctZ = cities.StartNode.Z;

                var mapX = (ctX - startX) * scaleX;
                var mapY = (ctZ - startZ) * scaleZ;
                //
                g.DrawString(cities.City, cityFont, Brushes.White, mapX, mapY);
            }

            // Draw all prefab curves
            foreach (var prefabItem in prefabs.Where(x => x.Prefab != null && x.HideUI == false).Distinct())
            {
                var inGps = Route != null && Route.Prefabs != null && Route.Prefabs.Any(x => x.Item1 == prefabItem);

                if (prefabItem.Prefab.Company != null)
                {
                    // TODO: Draw companies 
                }
                else
                {
                    var originNode = prefabItem.NodesList.FirstOrDefault().Value;
                    if (originNode != null)
                    {
                        foreach (
                            var poly in
                                prefabItem.Prefab.GeneratePolygonCurves(originNode, prefabItem.Origin))
                        {
                            var offsetPoly = poly.Select(x => new PointF((x.X - startX) * scaleX, (x.Z - startZ) * scaleZ)).ToArray();

                            var p = new Pen(inGps?Palette.PrefabGPS : Palette.Prefab, 1.0f);
                            g.DrawLines(p, offsetPoly);
                        }
                    }
                }
            }

            var rotation = point.Heading;

            var truckLength = 10;

            g.DrawLine(new Pen(Brushes.Cyan, 5.0f), scaleX * totalX + (float)Math.Sin(rotation) * truckLength * 2,
                scaleZ * totalZ + (float)Math.Cos(rotation) * truckLength * 2, scaleX * totalX, scaleZ * totalZ);
        }

        public Ets2Point CalculatePointFromMap(int cx, int cy)
        {
            var x = 2 * (-0.5f + cx / (float)_clip.Width) * totalX + centerX;
            var z = 2 * (-0.5f + cy / (float)_clip.Height) * totalZ + centerZ;

            return new Ets2Point(x, 0, z, 0);
        }

        public void SetNavigation(Ets2NavigationRoute route)
        {
            if (route.Loading == false)
            {
                Route = route;
            }
        }
    }
}
