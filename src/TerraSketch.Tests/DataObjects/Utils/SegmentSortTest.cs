using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.DataObjects.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerraSketch.Tests.DataObjects.Utils
{
    [TestClass]
    public class SegmentSortTest
    {
        private readonly ISegmentSort _segmentSort
            = new SegmentSort();

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void SegmentSort_Test1()
        {
            // Arrange
            Vector2 a = new Vector2(1, 1),
                b = new Vector2(5, 1),
                c = new Vector2(5, 5),
                d = new Vector2(1, 10),
                e = new Vector2(1, 5);
            var expected = new List<LineSegment>() {
                new LineSegment(e,a),
                new LineSegment(a,b),
                new LineSegment(b,c),
                new LineSegment(c,d),
                new LineSegment(d,e)
            };
            var segsToBeSorted = new List<LineSegment>()
            {
                new LineSegment(e,a),
                new LineSegment(b,a),
                new LineSegment(c,d),
                new LineSegment(b,c),
                new LineSegment(d,e)
            };

            //Act
            var sorted = _segmentSort.Sort(segsToBeSorted).ToList();


            //Assert

            CollectionAssert.AreEquivalent(expected, sorted);
        }

        [TestMethod]
        public void SegmentSort_Test2()
        {
            // Arrange
            Vector2 a = new Vector2(48,102),
                b = new Vector2(95,60),
                c = new Vector2(100,150),
                e = new Vector2(100,105);
            var expected = new List<LineSegment>() {

                new LineSegment(a,b),
                new LineSegment(b,e),
                new LineSegment(e,c),
                new LineSegment(c,a)
            };
            var segsToBeSorted = new List<LineSegment>()
            {
                new LineSegment(a,b),
                new LineSegment(a,c),
                new LineSegment(b,e),
                new LineSegment(e,c)
            };

            //Act
            var sorted = _segmentSort.Sort(segsToBeSorted).ToList();


            //Assert

            CollectionAssert.AreEquivalent(expected, sorted);
        }

        [TestMethod]
        public void SegmentSort_TestPoint()
        {
            // Arrange
            Vector2 a = new Vector2(1, 1),
                b = new Vector2(5, 1),
                c = new Vector2(5, 5),
                d = new Vector2(1, 10),
                e = new Vector2(1, 5);
            var expected = new List<Vector2>() {
                e,a,b,c,d
            };
            var segsToBeSorted = new List<LineSegment>()
            {
                new LineSegment(e,a),
                new LineSegment(b,a),
                new LineSegment(c,d),
                new LineSegment(b,c),
                new LineSegment(d,e)
            };

            //Act
            var sorted = _segmentSort.Sort(segsToBeSorted).Select(r=>r.Point1).ToList();


            //Assert

            CollectionAssert.AreEquivalent(expected, sorted);
        }
    }
}
