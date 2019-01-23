using System.Collections.Generic;
using System.Windows.Forms;

namespace TerraSketch.View
{
    public class KeyMessageFilter : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        //private const int WM_LBUTTONDBLCLK = 0x203;
        //private const int WM_LBUTTONDOWN = 0x201;
        //private const int WM_LBUTTONUP = 0x202;
        //private const int WM_MBUTTONDBLCLK = 0x209;
        //private const int WM_MBUTTONDOWN = 0x207;
        //private const int WM_MBUTTONUP = 0x208;

        //ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private static KeyMessageFilter _instance;

        public static KeyMessageFilter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KeyMessageFilter();
                }
                return _instance;
            }
        }



        private IDictionary<Keys, bool> keyTable = new Dictionary<Keys, bool>();

        private IDictionary<Keys, bool> KeyTable
        {
            get
            {
                return keyTable;
            }
        }

        public bool IsKeyPressed(Keys k)
        {
            bool pressed ;
            bool read = KeyTable.TryGetValue(k, out pressed);

            if (read)
                return pressed;

            return false;
        }

        #region reg
        
        public bool PreFilterMessage(ref Message mes)
        {
            var m = mes;

            if (m.Msg == WM_KEYDOWN)
            {
               // Console.WriteLine( (Keys)m.WParam);
                KeyTable[(Keys)m.WParam] = true;
            }
            else if (m.Msg == WM_KEYUP)
                KeyTable[(Keys)m.WParam] = false;
           
            return false;
        }
    }

    #endregion

    public enum Mouse
    {
        LeftDoubleclick = 0x203,
        LeftDown = 0x201,
        RightDown = 0x204,
        //LeftUp = 0x202,
    }

    struct MouseAction
    {
        public Mouse MouseType
        {
            get;
            private set;
        }
        public MouseAction(Mouse m):this()
        {
            MouseType = m;
        }
    }
}
