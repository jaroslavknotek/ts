using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using Voronoi.Generator;
using Common.MathUtils.Probability;

namespace TerraSketch.Generators
{
    public class VoronoiAreaGenerator : IVoronoiAreaGenerator
    {
        private readonly IVoronoiGenerator _voronoiGenerator;
        private readonly IRandom0 _random;
        private readonly PropabilityHelperRandom0 _randomHelper;
        public VoronoiAreaGenerator(IVoronoiGenerator voronoi, IRandom0 random)
        {
            _voronoiGenerator = voronoi;
            _random = random;
            _randomHelper = new PropabilityHelperRandom0();
        }

        public IList<IArea> GenerateAreas(IntVector2 size, IntVector2 sizeToGenerate, int countOfCells)
        {
            // those injected points cause, that all other areas going to be closed

            Vector2 delta = (size - sizeToGenerate) * .5f;
            var ex = 1.5f;
            var closingPoints = new List<Vector2>
            {
                //left
                new Vector2(-sizeToGenerate.X*ex, sizeToGenerate.Y*.5f)+delta,
                //right
                new Vector2((1f+ex)*sizeToGenerate.X, sizeToGenerate.Y*.5f)+delta,
                //top
                new Vector2(sizeToGenerate.X*.5f, -sizeToGenerate.Y*ex)+delta,
                //bottom
                new Vector2(sizeToGenerate.X*.5f, (1f+ex)*sizeToGenerate.Y)+delta
            };

            var pts = getRandomPoints(countOfCells, sizeToGenerate.X, sizeToGenerate.Y).Select(r => r + (Vector2)delta).ToList();
            pts.AddRange(closingPoints);

            //filter out injected points
            var areas = _voronoiGenerator.Generate(size, pts)
                .Where(r => !closingPoints.Contains(r.Center))
                .ToList();

            // return new List<IArea>() { areas[0]};

            return areas;
        }
        private IList<Vector2> getRandomPoints(int count, float sizeX, float sizeY)
        {
            var list = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                var x = _randomHelper.NextInRange(_random, 0, sizeX - 1);
                var y = _randomHelper.NextInRange(_random, 0, sizeY - 1);
                var pnt = new Vector2(x, y);
                list.Add(pnt);
            }

            return list;

        }
    }
}