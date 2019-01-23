using System;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.MathUtils.Probability;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Voronoi.Generator;

namespace Voronoi.Tests
{
    [TestClass]
    public class SegmentDividerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var octave = 1;
            var persistance = .5f;
            var amplitude =20;

            var mockRand = new Mock<IRandom2>();

            mockRand.Setup(r => r.NextD(It.IsAny<int>(), It.IsAny<int>()))
                .Returns( 1f);

            var sg = new SegmentDivider(mockRand.Object,amplitude,persistance);
            var inputLine = new LineSegment(new Vector2(1, 1), new Vector2(65, 65));

            var exPoint = new Vector2(40,25);
            var expected1 = new LineSegment(new Vector2(1, 1), exPoint);
            var expected2 = new LineSegment(exPoint, new Vector2(65, 65));


            // act
            var actualLines = sg.Subdivide(inputLine.Point1, inputLine.Point2);
            for (int i = 0; i < actualLines.Count; i++)
            {
                var al = actualLines[i];
                var p1 = new Vector2((int) al.Point1.X, (int) al.Point1.Y);
                var p2 = new Vector2((int)al.Point2.X, (int)al.Point2.Y);
                actualLines[i]=new LineSegment(p1,p2);
            }
            // assert
            Assert.AreEqual(actualLines[0], expected1);
            Assert.AreEqual(actualLines[1], expected2);

        }

        [TestMethod]
        public void Divider_Pass()
        {
            var octave = 2;
            var persistance = .5f;
            var amplitude = 20;

            var mockRand = new Mock<IRandom2>();

            mockRand.Setup(r => r.NextD(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1f);

            var sg = new SegmentDivider(mockRand.Object, amplitude, persistance);
            var inputLine = new LineSegment(new Vector2(1, 1), new Vector2(65, 65));

            var exPointh = new Vector2(40, 25);
            var exPointq = new Vector2(23, 9);
            var exPoint3q = new Vector2(56, 42);
            var expected1 = new LineSegment(new Vector2(1, 1), exPointq);

            var expected2 = new LineSegment(exPointq, exPointh);
            var expected3 = new LineSegment(exPointh, exPoint3q);
            var expected4 = new LineSegment(exPoint3q, new Vector2(65,65));

            // act
            var actualLines = sg.Subdivide(inputLine.Point1, inputLine.Point2);
            for (int i = 0; i < actualLines.Count; i++)
            {
                var al = actualLines[i];
                var p1 = new Vector2((int)al.Point1.X, (int)al.Point1.Y);
                var p2 = new Vector2((int)al.Point2.X, (int)al.Point2.Y);
                actualLines[i] = new LineSegment(p1, p2);
            }
            // assert
            Assert.AreEqual(actualLines[0], expected1);
            Assert.AreEqual(actualLines[1], expected2);
            Assert.AreEqual(actualLines[2], expected3);
            Assert.AreEqual(actualLines[3], expected4);

        }
    }
}
