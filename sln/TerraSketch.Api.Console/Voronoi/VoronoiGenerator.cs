using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using Voronoi.Generator.FortuneObjects;

namespace Voronoi.Generator
{
    public class VoronoiGenerator : IVoronoiGenerator
    {
        private readonly IVoronoiConverter _converter;

        public VoronoiGenerator(IVoronoiConverter converter)
        {
            _converter = converter;
        }
        /// <summary>
        /// Genereates using user input
        /// </summary>
        /// <param name="size"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public IList<IArea> Generate(IntVector2 size, IList<Vector2> input)
        {
            var f = new Fortune();
            //Points = input ;
            var segs = f.GetSegments(input, minSize: 0, maxSize: JryMath.Max(size.X, size.Y));
            return _converter.ConvertSegmentToAreas(segs, size);
            //return segs;
        }


    }

    public interface IVoronoiGenerator
    {
        IList<IArea> Generate(IntVector2 size, IList<Vector2> input);
    }
}
