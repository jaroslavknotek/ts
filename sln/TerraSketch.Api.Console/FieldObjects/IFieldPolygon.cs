using Common.DataObjects.Geometry;
using System.Collections.Generic;
using System.Numerics;


namespace TerraSketch.DataObjects.FieldObjects
{

    public interface IFieldPolygon : IPolygon
    {
        IEnumerable<LineSegment> Edges { get; }
        IEnumerable<LineSegment> SelectedEdges { get; }
        IEnumerable<Vector2> SelectedPoints { get; }

        bool HasSelectedPoints { get; }
        bool HasSelectedEdges { get; }
        IPlanarObjectState State { get; }
        bool HasAnyNotSelectedPoint { get; }

        IFieldPolygon ShallowCopy();

        int SelectedPointsCount { get; }
        bool HasSelectedEdgesConnected { get; }

        bool CheckForIntersections();
        bool RemoveSelected();
        bool Select(Vector2 v);
        bool IsInPoly(Vector2 v);
        bool Deselect(Vector2 v);
        void DeselectAll();
        bool MoveSelected(Vector2 shiftVector);
        void ScaleSelected(float factor, Vector2 center);
        void RotateSelected(float factor, Vector2 center);
        bool IsNearToSelected(Vector2 point);
        void SelectAll();
        void SplitSelected();
        void MergeSelected();
    }
}
