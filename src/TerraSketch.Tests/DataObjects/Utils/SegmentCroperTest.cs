using System;
using System.Collections.Generic;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.DataObjects.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerraSketch.Tests.DataObjects.Utils
{
    [TestClass]
    public class SegmentCroperTest
    {
        private readonly SegmentCroper _segmentCroper
            = new SegmentCroper(new GeometryUtils());

        private IArea _area1;
        [TestInitialize]
        public void Init()
        {



            var a1 = new Vector2(2, 8);
            var b1 = new Vector2(15, 25);
            var c1 = new Vector2(30, 6);
            _area1 = new Area()
            {
                Center = new Vector2(10, 10),
                Segments = new List<LineSegment>()
                {
                    new LineSegment(a1,b1 ),
                    new LineSegment(c1,b1 ),
                    new LineSegment(a1,c1 )
                },
                Points = new List<Vector2>()
                {
                    a1,b1,c1
                }
            };
        }

        [TestMethod]
        public void SegmentCroper_Test()
        {
            // arrange

            var segments = new List<LineSegment>()
            {
                new LineSegment(new Vector2(16,15),new Vector2(16,2) ),
                new LineSegment(new Vector2(16,15),new Vector2(26,25) ),
                new LineSegment(new Vector2(16,15),new Vector2(8,23) )
            };
            //act
            var vec = _segmentCroper.CropSegmentsByArea(segments, _area1);

            //assert

            //first
            Assert.IsTrue(
                segmentEquals(
                new LineSegment(new Vector2(16, 7), new Vector2(16, 15)),
                vec[0])
                );
            // second
            Assert.IsTrue(
                segmentEquals(
                new LineSegment(new Vector2(20, 19), new Vector2(16, 15)),
                vec[1])
                );

            // third
            Assert.IsTrue(
                segmentEquals(
                new LineSegment(new Vector2(11, 20), new Vector2(16, 15)),
                vec[2])
                );
        }








        private const float epsilon = .2f;
        private bool segmentEquals(LineSegment lineSegment, LineSegment lineSegment1)
        {
            return vectorEquals(lineSegment.Point1, lineSegment1.Point1)
                   && vectorEquals(lineSegment.Point2, lineSegment1.Point2);
        }

        private bool vectorEquals(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) < epsilon &&
                   Math.Abs(a.Y - b.Y) < epsilon;
        }
    }
}
