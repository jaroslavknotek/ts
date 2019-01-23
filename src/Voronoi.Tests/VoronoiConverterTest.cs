using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voronoi.Generator;
using Voronoi.Generator.FortuneObjects;

namespace Voronoi.Tests
{
    [TestClass]
    public class VoronoiConverterTest
    {
        private readonly VoronoiConverter _voronoiConverter;

        public VoronoiConverterTest()
        {
            var sg = new SegmentNoDivider();
            _voronoiConverter = new VoronoiConverter(sg);
        }
        [TestMethod]
        public void VoronoiTest()
        {
            var size = new IntVector2(100, 150);
            // Arrange
            Vector2 a = new Vector2(48, 102),
                b = new Vector2(95, 60),
                c = new Vector2(135, 105),
                d = new Vector2(101, 164),

                tl = new Vector2(0, 0),
                tr = new Vector2(200, 0),
                bl = new Vector2(0, 200),
                br = new Vector2(200, 200),

                t = new Vector2(105, 0),
                r = new Vector2(200, 89),
                l = new Vector2(0, 105),
                bt = new Vector2(107, 200),


                center1 = new Vector2(102, 97),
                center2 = new Vector2(47, 54),
                center3 = new Vector2(154, 53),
                center4 = new Vector2(147, 146),
                center5 = new Vector2(56, 157),

                fake = new Vector2(-100, -100)
                ;


            //frame
            Segment
                f1 = new Segment(tl, fake, center2) { End = t },
                f2 = new Segment(tr, center3, fake) { End = t },
                f3 = new Segment(tr, fake, center3) { End = r },
                f4 = new Segment(r, fake, center4) { End = br },
                f5 = new Segment(br, fake, center4) { End = bt },
                f6 = new Segment(bl, center5, fake) { End = bt },
                f7 = new Segment(tl, center2, fake) { End = l },
                f8 = new Segment(l, center5, fake) { End = bl },

                //core
                core1 = new Segment(a, center2, center1) { End = b },
                core2 = new Segment(a, center1, center5) { End = d },
                core3 = new Segment(b, center3, center1) { End = c },
                core4 = new Segment(d, center1, center4) { End = c },

                //others

                o1 = new Segment(a, center5, center2) { End = l },
                o2 = new Segment(t, center3, center2) { End = b },
                o3 = new Segment(c, center4, center3) { End = r },
                o4 = new Segment(d, center4, center5) { End = bt };

            var allSegments = new List<Segment>()
                {
                    f1,f2,f3,f4,f5,f6,f7,f8,core1,core2 ,core3 ,core4 ,o1,o2 ,o3 ,o4
                };

            // aranged test data

            //expectedData
            Vector2 exa = a,
                exb = b,
                exc = new Vector2(100, 105),
                exd = new Vector2(100, 150),

                extl = tl,
                extr = new Vector2(100, 0),
                exbl = new Vector2(0, 150),
                exbr = new Vector2(100, 150),

                ext = new Vector2(100, 0),
                exr = new Vector2(100, 89),
                exl = l,
                exbt = new Vector2(100, 150),


                excenter1 = center1, //new Vector2(100, 97),
                excenter2 = center2,
                excenter3 = center3,//new Vector2(150, 53),
                excenter4 = center4,// new Vector2(100, 146),
                excenter5 = center5// new Vector2(56, 150)
                ;


            var cArea = new Area()
            {
                Center = excenter1,

                Points = new List<Vector2>()
                {
                    exa,
                    exb,
                    exc,
                    exd
                },
                Segments = new List<LineSegment>()
                {
                    new LineSegment(exa, exb),
                    new LineSegment(exb, exc),
                    new LineSegment(exc, exd),
                    new LineSegment(exd, exa)
                }

            };

            var tlArea = new Area()
            {
                Center = excenter2,

                Points = new List<Vector2>()
                {
                   extl,
                   ext,
                   exb,
                   exa,
                   exl
                },
                Segments = new List<LineSegment>()
                {
                    new LineSegment(extl, ext),
                    new LineSegment(ext, exb),
                    new LineSegment(exb, exa),
                    new LineSegment(exa, exl),
                    new LineSegment(exl, extl)
                }

            };

            var trArea = new Area()
            {
                Center = excenter3,

                Points = new List<Vector2>()
                {
                   extr,
                   exr,
                   exc,
                   exb,
                   //ext
                },
                Segments = new List<LineSegment>()
                {
                    new LineSegment(extr, exr),
                    new LineSegment(exr, exc),
                    new LineSegment(exc, exb),
                    new LineSegment(exb, ext),
                //    new LineSegment(ext, extr)
                }
            };

            var brArea = new Area()
            {
                Center = excenter4,

                Points = new List<Vector2>()
                {
                   exr,
               //    exbr,
                //   exbt,
                   exd,
                   exc
                },
                Segments = new List<LineSegment>()
                {
                    new LineSegment(exr, exbr),
                   // new LineSegment(exbr, exbt),
                 //   new LineSegment(exbt, exd),
                    new LineSegment(exd, exc),
                    new LineSegment(exc, exr)
                }
            };
            var blArea = new Area()
            {
                Center = excenter5,

                Points = new List<Vector2>()
                {
                   exbl,
                   exl,
                   exa,
               //    exd,
                   exbt
                },
                Segments = new List<LineSegment>()
                {

                    new LineSegment( exbl,exbt),
                    new LineSegment( exbt,exa),
                    new LineSegment(exa, exl),
                    new LineSegment( exl,exbl),

                }
            };

            var expectedAreas = new List<Area>()
            {
             tlArea,
                trArea,
                brArea,
                blArea,
                cArea
            };

            var actual = _voronoiConverter.ConvertSegmentToAreas(allSegments, size).Skip(1).ToList();

            for (int i = 0; i < actual.Count; i++)
            {
                var actualArea = actual[i];
                var expectedArea = expectedAreas[i];

                Assert.AreEqual(expectedArea.Center, actualArea.Center);

                CollectionAssert.AreEquivalent(expectedArea.Points.ToList(), actualArea.Points.ToList());
                CollectionAssert.AreEquivalent(expectedArea.Segments.ToList(), actualArea.Segments.ToList());
            }


        }
    }
}
