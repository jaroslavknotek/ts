using System.Collections.Generic;

namespace TerraSketch.DataObjects.FieldObjects
{
    public interface IFieldList : IEnumerable<IField>
    {
        //int Count { get; }
        //int Min { get;  }
        //int Max { get;}

        void Add(IField field);
        
        bool Remove(IField item);
        bool CanBeSentToBack(IField field);
        void SendToBack(IField field);
        void BringToFront(IField field);
        bool CanBeBroughtToFront(IField field);
        void SendToBottom(IField field);
        void BringToTop(IField field);
    }
}