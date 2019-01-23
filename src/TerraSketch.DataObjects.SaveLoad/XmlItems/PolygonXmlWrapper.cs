using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Serialization;
using Common.DataObjects.Geometry;
using TerraSketch.DataObjects.FieldObjects;
using System.Linq;

namespace TerraSketch.DataObjects.SaveLoad
{
    public class PolygonConverter
    {

        public IFieldPolygon ToObject(IFieldPolygon  wrapper)
        {
            FieldPolygon poly = new FieldPolygon();

            foreach (var p in wrapper.Points)
            {
                poly.AddPoint(p);
            }
            return poly;
        }

        public PolygonXmlWrapper ToXmlWrapper(IFieldPolygon param)
        {
            PolygonXmlWrapper pxw = new PolygonXmlWrapper();
            pxw.Points = param.Points.ToList();

            return pxw;
        }
    }


    public class PolygonXmlWrapper : IFieldPolygon
    {


        private List<Vector2> _points;
       
        [XmlIgnore]

        public bool HasAnyNotSelectedPoint
        {
            get
            {
                return false;
            }
        }
        [XmlIgnore]
        public bool HasSelectedEdges
        {
            get
            {
                return false;
            }
        }
        [XmlIgnore]
        public bool HasSelectedEdgesConnected
        {
            get
            {
                return false;
            }
        }

        [XmlIgnore]
        public bool HasSelectedPoints
        {
            get
            {
                return default(bool);
            }
        }

        [XmlElement("Pts")]
        public List<Vector2> Points
        {
            get { return _points; }
            set { _points = value; }
        }
        [XmlIgnore]
        public int PointsCount
        {
            get
            {
                return -1;
            }
        }

        [XmlIgnore]
        public IEnumerable<LineSegment> SelectedEdges
        {
            get
            {
                return default(IEnumerable<LineSegment>);
            }
        }

        [XmlIgnore]
        public IPlanarObjectState State
        {
            get
            {
                return default(IPlanarObjectState);
            }
        }
        [XmlIgnore]
        public IEnumerable<LineSegment> Edges
        {
            get
            {
                return Edges;
            }
        }

        IEnumerable<Vector2> IPolygon.Points
        {
            get
            {
                return Points;
            }
        }

     

        IEnumerable<Vector2> IFieldPolygon.SelectedPoints
        {
            get
            {
                return null;
            }
        }
        [XmlIgnore]
        public int SelectedPointsCount
        {
            get
            {
                return 0;
            }
        }

        public bool AddPoint(Vector2 v)
        {
            throw new InvalidOperationException();
        }

        public bool Deselect(Vector2 v)
        {
            throw new InvalidOperationException();
        }

        public void DeselectAll()
        {
            throw new InvalidOperationException();
        }

        public bool IsNearToSelected(Vector2 point)
        {
            throw new InvalidOperationException();
        }

        public bool MoveSelected(Vector2 v)
        {
            throw new InvalidOperationException();
        }

        public void RemoveLast()
        {
            throw new InvalidOperationException();
        }

        public bool RemoveSelected()
        {
            throw new InvalidOperationException();
        }

        public bool Select(Vector2 v)
        {
            throw new InvalidOperationException();

        }

        public void Unselect()
        {
            throw new InvalidOperationException();

        }

        public void SelectAll()
        {
            throw new InvalidOperationException();

        }

        public void ScaleSelected(float factor, Vector2 center)
        {
            throw new InvalidOperationException();
        }

        public void RotateSelected(float factor, Vector2 center)
        {
            throw new InvalidOperationException();
        }

        public void SplitSelected()
        {
            throw new InvalidCastException();
        }
        public void MergeSelected()
        {
            throw new InvalidCastException();
        }

        public bool IsInPoly(Vector2 v)
        {
            throw new InvalidOperationException();
        }

        public bool CheckForIntersections()
        {
            throw new InvalidOperationException();
        }

        public IFieldPolygon ShallowCopy()
        {
            throw new InvalidOperationException();
        }
    }
    

}
