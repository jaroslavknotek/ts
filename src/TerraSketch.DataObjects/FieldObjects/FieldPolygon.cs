using Common.DataObjects.Geometry;
using Common.MathUtils;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System;

namespace TerraSketch.DataObjects.FieldObjects
{
    public enum IPlanarObjectState
    {
        Finished, SelectedFinished
    }


    public class FieldPolygon : IFieldPolygon
    {
        /// <summary>
        /// When new point A is inserted and there is a point B that is closer than DISTANCE value, 
        /// then the new one is considered to be signal for polygon to close and finish
        /// </summary>
        private readonly int DISTANCE = UISettings.UIDistance;
        private readonly GeometryUtils geoUtils = new GeometryUtils();


        public int PointsCount { get { return _points == null ? 0 : _points.Count; } }
        public int SelectedPointsCount { get { return _selectedPoints == null ? 0 : _selectedPoints.Count; } }

        private IList<Vector2> _points = new List<Vector2>();
        public IEnumerable<Vector2> Points
        {
            get
            {
                return _points;
            }
        }

        private IList<LineSegmentWithIndices> _edges = new List<LineSegmentWithIndices>();
        public IEnumerable<LineSegment> Edges { get { return _edges.Select(r => r.Segment); } }

        public IPlanarObjectState State { get; private set; }



        private IList<Vector2> _selectedPoints = new List<Vector2>();
        public IEnumerable<Vector2> SelectedPoints { get { return _selectedPoints; } }
        private IList<LineSegmentWithIndices> _selectedEdges = new List<LineSegmentWithIndices>();
        public IEnumerable<LineSegment> SelectedEdges { get { return _selectedEdges.Select(r => r.Segment); } }
        public bool HasSelectedPoints { get { return SelectedPoints != null && SelectedPoints.Any(); } }
        public bool HasAnyNotSelectedPoint { get { return _selectedPoints == null || _selectedPoints.Count < _points.Count; } }
        public bool HasSelectedEdges
        {
            get
            {

                return SelectedEdges != null && SelectedEdges.Any();
            }
        }
        private bool _hasSelectedEdgesConnected = false;
        public bool HasSelectedEdgesConnected { get { return _hasSelectedEdgesConnected; } }

        public bool AddPoint(Vector2 v)
        {
            _points.Add(v);
            recalcEdges();
            return true;
        }

        public bool CheckForIntersections()
        {
            foreach (var edge in Edges)
            {
                if (geoUtils.HasIntersection(Edges, edge))
                    return true;
            }

            return false;
        }


        public bool RemoveSelected()
        {
            if (!HasSelectedPoints) return false;

            foreach (var item in SelectedPoints)
            {
                _points.Remove(item);
            }

            this.DeselectAll();
            recalcEdges();

            return true;
        }


        // recalulates position of all edges
        // update selectedEdges endpoints
        // finds wheter it has contitinous path of selected edges
        private void recalcEdges()
        {

            _selectedEdges = new List<LineSegmentWithIndices>();
            _edges = new List<LineSegmentWithIndices>();
            if (_points.Count < 2) return;
            int j = _points.Count - 1;
            var pt = _points[j];
            for (int i = 0; i < _points.Count; i++)
            {
                var newPt = _points[i];
                var edg = new LineSegmentWithIndices(pt, j, newPt, i);
                _edges.Add(edg);
                var ptIsSelected = IsSelected(pt);
                var newPtIsSelected = IsSelected(newPt);
                if (ptIsSelected && newPtIsSelected)
                {
                    _selectedEdges.Add(edg);
                }
                pt = newPt;
                j = i;
            }

            int breakCount = 0;
            for (int i = 0; i < _selectedEdges.Count; i++)
            {
                var ia = i % _selectedEdges.Count;
                var ib = (i + 1) % _selectedEdges.Count;
                var a = _selectedEdges[ia];
                var b = _selectedEdges[ib];

                if (a.IndexB != b.IndexA)
                    breakCount++;

            }
            _hasSelectedEdgesConnected = breakCount <= 1;

        }

        public bool IsInPoly(Vector2 v)
        {


            return geoUtils.IsInPolygon(Points.ToArray(), v);
        }

        public bool Select(Vector2 v)
        {

            for (int i = 0; i < PointsCount; i++)
            {
                if ((_points[i] - v).Length() < DISTANCE)
                {
                    _selectedPoints.Add(_points[i]);
                    //slow
                    recalcEdges();
                    return true;
                }
            }


            return false;
        }

        public bool Deselect(Vector2 v)
        {
            for (int i = 0; i < PointsCount; i++)
            {
                var pt = _selectedPoints[i];
                if ((pt - v).Length() < DISTANCE)
                {
                    _selectedPoints.RemoveAt(i);
                    //slow
                    recalcEdges();
                    return true;
                }
            }
            return false;
        }

        public void SelectAll()
        {
            //creates copy
            _selectedPoints = this._points.ToList();
            _selectedEdges = _edges.ToList();
        }

        public void DeselectAll()
        {
            _selectedEdges = new List<LineSegmentWithIndices>();
            _selectedPoints = new List<Vector2>();
        }


        public bool MoveSelected(Vector2 shiftVector)
        {
            if (!HasSelectedPoints) return false;

            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                var index = _selectedPoints.IndexOf(pt);
                if (index != -1)
                {
                    _points[i] = pt + shiftVector;
                    _selectedPoints[index] = _points[i];
                }
            }
            recalcEdges();
            return true;
        }

        public void ScaleSelected(float factor, Vector2 center)
        {
            if (!HasSelectedPoints) return;

            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                var index = _selectedPoints.IndexOf(pt);
                if (index != -1)
                {

                    var scaleVector = pt - center;

                    _points[i] = center + scaleVector * factor;

                    _selectedPoints[index] = _points[i];
                }
            }
            recalcEdges();
        }


        public void MergeSelected()
        {
            if (SelectedPointsCount < 2) return;
            int count = _points.Count;
            int firstEncountered = -1;
            var gathered = new List<Vector2>();
            for (int i = 0; i < _points.Count; i++)
            {

                var pt = _points[i];
                if (IsSelected(pt))
                {
                    if (firstEncountered == -1)
                        firstEncountered = i;
                    gathered.Add(pt);
                }
            }

            var center = geoUtils.GetCenter(gathered);

            foreach (var item in _selectedPoints)
            {
                if (_points.Contains(item))
                    _points.Remove(item);
            }
            _points.Insert(firstEncountered, center);
            _selectedPoints = new List<Vector2>();
            recalcEdges();
        }
        public void SplitSelected()
        {
            if (SelectedPointsCount < 2) return;
            int count = _points.Count;

            var ptLast = _points[_points.Count - 1];
            var ptFirst = _points[0];
            if (IsSelected(ptLast) && IsSelected(ptFirst))
                _points.Add(geoUtils.GetCenter(new[] { ptLast, ptFirst }));

            for (int i = count - 1; i > 0; i--)
            {
                ptFirst = _points[i - 1];
                ptLast = _points[i];

                if (IsSelected(ptLast) && IsSelected(ptFirst))
                {
                    var newPt = geoUtils.GetCenter(new[] { ptLast, ptFirst });
                    _points.Insert(i, newPt);
                }
            }

            recalcEdges();
        }

        public void RotateSelected(float factor, Vector2 center)
        {
            if (!HasSelectedPoints) return;

            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                var index = _selectedPoints.IndexOf(pt);
                if (index != -1)
                {

                    pt = pt - center;
                    var p = new Vector2(JryMath.Cos(factor) * pt.X - JryMath.Sin(factor) * pt.Y,
                                        JryMath.Sin(factor) * pt.X + JryMath.Cos(factor) * pt.Y);
                    _points[i] = p + center;
                    _selectedPoints[index] = _points[i];
                }
            }
            recalcEdges();
        }

        public bool IsNearToSelected(Vector2 v)
        {
            // TODO analyze
            if (_selectedPoints == null) return false;
            for (int i = 0; i < _selectedPoints.Count; i++)
            {
                if ((_selectedPoints[i] - v).Length() < DISTANCE)
                    return true;
            }
            return false;
        }

        private bool IsSelected(Vector2 v)
        {

            return _selectedPoints != null && _selectedPoints.Contains(v);

        }

        public IFieldPolygon ShallowCopy()
        {
            return new FieldPolygon()
            {
                // segments are struct so they are copied
                _edges = this._edges.Select(r => r).ToList(),
                _points = this._points.Select(r => r).ToList(),
                _hasSelectedEdgesConnected = this._hasSelectedEdgesConnected,
                State = this.State,
                _selectedEdges = this._selectedEdges.Select(r => r).ToList()
               , _selectedPoints = this._selectedPoints.Select(r => r).ToList()
            };

        }

        private struct LineSegmentWithIndices
        {
            public int IndexA { get; private set; }
            public int IndexB { get; private set; }
            public LineSegment Segment { get; private set; }
            public LineSegmentWithIndices(Vector2 a, int indexA, Vector2 b, int indexB) : this()
            {
                IndexA = indexA;
                IndexB = indexB;
                Segment = new LineSegment(a, b);
            }

        }
    }
}
