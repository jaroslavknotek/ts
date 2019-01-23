using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.DataObjects.Utils;
using Common.MathUtils;
using Common.MathUtils.Probability;
using Voronoi.Generator.FortuneObjects;

namespace Voronoi.Generator
{
    public class VoronoiConverter :IVoronoiConverter
    {
        private readonly AreaFactory _areaFactory = new AreaFactory();
        
        private readonly ISegmendDivider _segmentDivider;
        public VoronoiConverter(ISegmendDivider segmentDivider)
        {
            _segmentDivider = segmentDivider;
        }
        public IList<IArea> ConvertSegmentToAreas(IEnumerable<Segment> segments, IntVector2 size)
        {
            if (!segments.Any()) return new List<IArea>();


            var areasDictinary = new Dictionary<Vector2, IList<Segment>>();
            foreach (var segment in segments)
            {
                addSite(areasDictinary, segment, segment.LeftSite);
                addSite(areasDictinary, segment, segment.RightSite);
            }

            return areasDictinary.Select(ra =>
                {
                    var areaSegments = ra.Value;
                    var finalArea = new List<LineSegment>();
                    foreach (var seg in areaSegments)
                    {
                        finalArea.AddRange(_segmentDivider.Subdivide(seg.Start.Value, seg.End.Value));
                    }

                    var are = _areaFactory.instantiate(ra.Key,finalArea);
                    return are;

                }
            ).ToList();

        }



        private void addSite(Dictionary<Vector2, IList<Segment>> areasDictinary, Segment segment, Vector2? segmentPoint)
        {
            if (!segmentPoint.HasValue) return;
            IList<Segment> siteSegments;
            if (areasDictinary.TryGetValue(segmentPoint.Value, out siteSegments))
                siteSegments.Add(segment);

            else
                areasDictinary.Add(segmentPoint.Value, new List<Segment>() { segment });

        }

    }

    /// <summary>
    /// used to finalize vornoi result which is list of segments
    /// </summary>
    public interface IVoronoiConverter
    {
        IList<IArea> ConvertSegmentToAreas(IEnumerable<Segment> segments, IntVector2 size);
    }

    public interface ISegmendDivider
    {
        IList<LineSegment> Subdivide(Vector2 x, Vector2 y);
    }

    public class SegmentNoDivider : ISegmendDivider
    {
        public IList<LineSegment> Subdivide(Vector2 x,Vector2 y)
        {
            return new List<LineSegment>() { new LineSegment(x,y) };
        }
    }

    public class SegmentDivider : ISegmendDivider
    {
        private readonly IRandom2 _random;
        private readonly float _persistance;
        
        private readonly int _amplitude;

        public SegmentDivider(IRandom2 random, int amplitude, float persistance)
        {
            _random = random;
        
            _amplitude = amplitude;
            _persistance = persistance;
        }

        public IList<LineSegment> Subdivide(Vector2 x,Vector2 y)
        {
            float per = 1;
            var points = new List<Vector2> { x,y};

            var oc = getOctaveAccordingToLength(x, y);

            for (int o = 0; o < oc; o++)
            {

                var newPts = new List<Vector2>();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var cur = points[i];
                    var next = points[i + 1];

                    var dif = next - cur;
                    var perp = Vector2.Normalize(new Vector2(dif.Y, -dif.X));

                    // TODO deterministicaly sort point 
                    var a = cur.X < next.X ? cur :next ;
                    var b = cur.X >= next.X ? cur : next;
                    var curSeed = _random.NextD((int)a.X, (int)a.Y) * 1000;
                    var nextSeed = _random.NextD((int)b.X, (int)b.Y) * 1000;

                    float moverRand = (_random.NextD((int)curSeed, (int)nextSeed)-.5f * _amplitude * per*2);
                    Vector2 mover = perp * moverRand  ;
                    var newPoint = cur + dif*.5f + mover;

                    newPts.Add(cur);
                    newPts.Add(newPoint);
                }

                newPts.Add(points[points.Count - 1]);
                points = newPts;

                per *= _persistance;
            }

            var lines = new List<LineSegment>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                var cur = points[i];
                var next = points[i + 1];

                lines.Add(new LineSegment(cur, next));
            }
            return lines;
        }

        private static int getOctaveAccordingToLength(Vector2 x, Vector2 y)
        {
            return (int)((x - y).Length() *.01f )+1;
        }
    }
}

