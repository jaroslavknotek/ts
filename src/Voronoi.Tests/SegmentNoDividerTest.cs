using System;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voronoi.Generator;

namespace Voronoi.Tests
{
    [TestClass]
    public class SegmentNoDividerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // arrange
            var sg = new SegmentNoDivider();
            var expected = new LineSegment(new Vector2(25,321),new Vector2(322,657) );

            // act
            var actualLine = sg.Subdivide(expected.Point1, expected.Point2).First();

            // assert
            Assert.AreEqual(expected,actualLine);
        }
    }
}
