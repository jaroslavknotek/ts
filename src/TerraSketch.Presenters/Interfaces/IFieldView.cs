using System.Drawing;
using System.Numerics;
namespace TerraSketch.Presenters
{
    public interface IFieldView :IRefreshableView
    {
      new  void RefreshView();
        Vector2 GetCurrentMousePos();

        bool IsFieldViewSelected();
        void ShowProgressBar();
        void CloseProgressBar();
        void UpdateProgressBar(string text);
    }
}
