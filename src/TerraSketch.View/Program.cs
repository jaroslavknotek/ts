using System;
using System.Windows.Forms;

namespace TerraSketch.View
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // used here for parsing floats. 
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Application.AddMessageFilter(KeyMessageFilter.Instance);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MasterView());
        }
    }
}
