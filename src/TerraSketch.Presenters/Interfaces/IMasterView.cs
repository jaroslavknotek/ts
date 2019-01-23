using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraSketch.Presenters
{
  public  interface IMasterView :IRefreshableView
  {
      new void RefreshView();
        void ShowAboutForm();
        void SelectHeightmapView();
        void SelectVisualView();
        FileInfo GetLoadFilePath();



       FileInfo GetSaveFilePath();
        void ShowInfoOKMessage(string v);
        void ShowErrorOKMessage(string v);
        bool ShowInfoYesNoMessage(string v1, string v2);
    }
}
