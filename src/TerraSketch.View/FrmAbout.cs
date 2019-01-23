using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerraSketch.Presenters;

namespace TerraSketch.View
{
    
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
            tbAboutInfo.Text =
                "Credits for icons: https://www.elegantthemes.com/ " + Environment.NewLine +
            " for icon pack http://www.flaticon.com/packs/elegant-font/3" + Environment.NewLine +
                "Credits for 3D gfx: http://neokabuto.blogspot.cz/p/tutorials.html " + Environment.NewLine +
            " and Josef Pelikan's framework" + Environment.NewLine;
        }
    }
}
