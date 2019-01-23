
namespace TerraSketch.Presenters
{
    public interface IHeightmapView:IRefreshableView
    {
       new void RefreshView();
        void ShowInfoOKMessage(string v);
        void ShowErrorOKMessage(string v);
        void ClearView();
        bool IsHeightmapViewSelected();
    }
}
