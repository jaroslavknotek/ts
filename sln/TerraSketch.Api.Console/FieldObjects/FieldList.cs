using System;
using System.Collections;
using System.Collections.Generic;

namespace TerraSketch.DataObjects.FieldObjects
{
    public class ZOrderChangedEventArgs : EventArgs
    {
        public int PreviousValue { get; private set; }
        public int NewValue { get; private set; }
        public ZOrderChangedEventArgs(int prevValue, int newValue)
        {
            PreviousValue = prevValue;
            NewValue = newValue;
        }
    }

    /// <summary>
    /// Stores fields and makes sure that each layer has unique z-order
    /// </summary>
    public class FieldList : IFieldList//,IList<IField>
    {
        private SortedList<int, IField> fields = new SortedList<int, IField>();

        public int Count
        {
            get
            {
                return fields.Count;
            }
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return fields.Values.GetEnumerator();
        }

        public void Add(IField field)
        {

            var cur = Count > 0 ? fields.Values[Count - 1].ZOrder + 1 : 0;
            field.ZOrder = cur;
            insert(field);

        }

        private void insert(IField field)
        {
            field.ZOrderChanged += Field_ZOrderChanged;
            fields.Add(field.ZOrder, field);
        }

        private void Field_ZOrderChanged(object sender, ZOrderChangedEventArgs e)
        {
            // field.Zorder == e.newValue
            var field = sender as IField;
            if (field == null || e.PreviousValue == e.NewValue) return;
            // zorder should be THE key.

            fields.Remove(e.PreviousValue);

            var toBePosition = fields.Keys.IndexOf(e.NewValue);

            if (toBePosition == -1)
            {
                fields.Add(field.ZOrder, field);
                return;
            }
            // 


            IField replacedItem = null;
            IField replacingItem = field;
            field.ZOrderChanged -= this.Field_ZOrderChanged;

            Func<int, int> opp;
            // now i have to move previous item on a position
            if (e.NewValue < e.PreviousValue)
                opp = (i) => i + 1;
            else
                opp = (i) => i - 1;

            var prevPos = field.ZOrder;
            var pos = toBePosition;

            while (pos != -1)
            {
                replacedItem = fields.Values[pos];
                replacedItem.ZOrderChanged -= this.Field_ZOrderChanged;

                //cleaning new space
                fields.Remove(replacingItem.ZOrder);
                // inserting on a new place
                replacingItem.ZOrderChanged += this.Field_ZOrderChanged;
                fields.Add(replacingItem.ZOrder, replacingItem);

                // setting new id that tepends on a direction.
                replacedItem.ZOrder = opp(replacedItem.ZOrder);//Everything should be handled recursively.
                pos = fields.Keys.IndexOf(replacedItem.ZOrder);

                replacingItem = replacedItem;
            }

            replacingItem.ZOrderChanged += this.Field_ZOrderChanged;
            fields.Add(replacingItem.ZOrder, replacingItem);


        }

        public bool Remove(IField item)
        {
            item.ZOrderChanged -= Field_ZOrderChanged;


            // it must be there
            return fields.Remove(item.ZOrder);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        public bool CanBeSentToBack(IField field)
        {
            return Count > 1 && fields.Values[0] != field;
        }

        public bool CanBeBroughtToFront(IField field)
        {
            return Count > 1 && fields.Values[Count - 1] != field;

        }

        public void SendToBack(IField field)
        {
            int i = getIndexOfField(field);
            var newPosition = fields.Keys[i - 1];
            field.ZOrder = newPosition;
        }

        public void BringToFront(IField field)
        {
            int i = getIndexOfField(field);

            var newPosition = fields.Keys[i + 1];
            field.ZOrder = newPosition;
        }



        public void SendToBottom(IField field)
        {
            int i = getIndexOfField(field);
            var newPosition = fields.Keys[0];
            field.ZOrder = newPosition - 1;
        }

        public void BringToTop(IField field)
        {
            int i = getIndexOfField(field);
            var newPosition = fields.Keys[Count - 1];
            field.ZOrder = newPosition + 1;
        }

        private int getIndexOfField(IField field)
        {
            var i = fields.Values.IndexOf(field);
            if (i == -1) throw new InvalidOperationException("Field is not present in FieldList");
            return i;
        }
    }
}
