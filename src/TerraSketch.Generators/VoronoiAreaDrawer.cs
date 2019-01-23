using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{
    
    public class VoronoiAreaDrawer : IVoronoiAreaDrawer
    {
        private readonly GeometryUtils _geometryUtiltiy;

        public VoronoiAreaDrawer()
        {
            _geometryUtiltiy = new GeometryUtils();
        }

        public void PrintToLayer(ILayerMasked layer, IList<IArea> areas,Vector2 translateVector)
        {

            foreach (var area in areas)
            //Parallel.ForEach(areas, (area) =>
                {
                    var translatedArea = translateArea(area, translateVector);
                    drawAreaToLayer(layer, translatedArea);
                }
            //);
            
        }

        private IArea translateArea(IArea area, Vector2 translateVector)
        {
            return
                new Area()
                {

                    Center = area.Center + translateVector,
                    Points = area.Points.Select(r => r + translateVector).ToList(),
                    Segments = area.Segments.Select(seg =>
                        new LineSegment(seg.Point1 + translateVector, seg.Point2 + translateVector)
                    ).ToList()

                };
        }

        private void drawAreaToLayer(ILayerMasked layer, IArea area)
        {
            var pts = area.Points.ToArray();
            var bounds = _geometryUtiltiy.GetBoundsForPoints(pts).RestrictOn(layer.Resolution);

            for (int i = bounds.Top; i < bounds.Bottom; i++)
            {
                for (int j = bounds.Left; j < bounds.Right; j++)
                {
                    if (j == 464 && i == 385)
                    {
                        
                    }
                    if (!_geometryUtiltiy.IsInPolygon(pts, j,i)) continue;
                    var coor = new Vector2(j, i);
                    int indexOfClosestSeg = 0;
                    var distance = float.MaxValue;

                    for (var k = 0; k < area.Segments.Count; k++)
                    {
                        var seg = area.Segments[k];
                        var dist = _geometryUtiltiy.DistanceFromPointToSegmentSquare(
                            ref coor,ref seg);

                        if (dist < distance)
                        {
                            distance = dist;
                            indexOfClosestSeg = k;
                        }
                    }
                    layer[j,i] = _geometryUtiltiy.DistanceFromPointToSegment(coor,area.Segments[indexOfClosestSeg]);
                }
            }
        }
        
        
    }




    public class VerySpecificQuadraticVoronoiAreaDrawer : IVoronoiAreaDrawer
    {
        private readonly GeometryUtils _geometryUtiltiy;

        public VerySpecificQuadraticVoronoiAreaDrawer()
        {
            _geometryUtiltiy = new GeometryUtils();
        }

        public void PrintToLayer(ILayerMasked layer, IList<IArea> areas, Vector2 translateVector)
        {

            foreach (var area in areas)
            //Parallel.ForEach(areas, (area) =>
            {
                var translatedArea = translateArea(area, translateVector);
                drawAreaToLayer(layer, translatedArea);
            }
            //);

        }

        private IArea translateArea(IArea area, Vector2 translateVector)
        {
            return
                new Area()
                {

                    Center = area.Center + translateVector,
                    Points = area.Points.Select(r => r + translateVector).ToList(),
                    Segments = area.Segments.Select(seg =>
                        new LineSegment(seg.Point1 + translateVector, seg.Point2 + translateVector)
                    ).ToList()

                };
        }

        private void drawAreaToLayer(ILayerMasked layer, IArea area)
        {
            var pts = area.Points.ToArray();
            

            for (int i = 0; i < layer.Resolution.Y; i++)
            {
                for (int j = 0; j < layer.Resolution.X; j++)
                {
                    var coor = new Vector2(j, i);
                    //if (!_geometryUtiltiy.IsInPolygon(pts, coor)) continue;

                    var positiveDistance1 = float.MaxValue;
                    var positiveDistance2 = float.MaxValue;
                    var positiveDistance3 = float.MaxValue;
                    //var negativeDistance = float.MaxValue;
                    foreach (var seg in area.Segments)
                    {
                        var dist = _geometryUtiltiy.DistanceFromPointToSegment(coor, seg);
                        
                        if (dist < positiveDistance1)
                        {
                            positiveDistance3 = positiveDistance2;
                            positiveDistance2 = positiveDistance1;
                            positiveDistance1 = dist;
                        }
                        else if (dist < positiveDistance2)
                        {
                            positiveDistance3 = positiveDistance2;
                            positiveDistance2 = dist;
                        }
                        else if (dist < positiveDistance3)
                            positiveDistance3 = dist;
                    }


                    if (positiveDistance1 == float.MaxValue) positiveDistance1 = 0;
                    if (positiveDistance2 == float.MaxValue) positiveDistance2 = 0;
                    if (positiveDistance3 == float.MaxValue)
                        positiveDistance3 = 0;

                    layer[coor] = positiveDistance1 + positiveDistance2 + positiveDistance3;
                }
            }
        }
        
    }
}