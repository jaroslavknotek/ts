using System;
using System.Drawing;
using System.Windows.Forms;
using TerraSketch.Presenters;
using System.Numerics;
using TerraSketch.VisualPresenters;
using System.IO;
using System.Collections.Generic;
namespace TerraSketch.View
{
    
    public partial class MasterView : Form, IFieldView, IMasterView, IHeightmapView, IVisual3DView
    {
        private CommandBinder binder = new CommandBinder();
        
        public MasterPresenter MasterPreseneter { get; private set; }


        public MasterView()
        {
            
            DataObjects.UISettings.UIDistance = 5;

            InitializeComponent();



            MasterPreseneter = new MasterPresenter(this);

            var fp = new FieldPresenter( this, MasterPreseneter);
            var hp = new HeightMapPresenter( this,MasterPreseneter);
            var vp = new Visualization3DPresenter(this);
            
            MasterPreseneter.SetupPresenters(fp, hp, vp);

            initMainView();
            init3DView();
            initHeightmapView();
            initFieldView();
            
            pbFieldCanvas.Focus();
        }

        

        private void initMainView()
        {
            this.masterPresenterBindingSource.DataSource = new List<MasterPresenter>() { MasterPreseneter };

            bindMasterControls();
        }




        public FileInfo GetLoadFilePath()
        {
            loadDialog.InitialDirectory = "c:\\";
            loadDialog.Filter = "All files (*.*)|*.*";

            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                    return new FileInfo(loadDialog.FileName);
            }

            throw new Exception("Load does not work");
        }

        public FileInfo GetSaveFilePath()
        {
            saveDialog.InitialDirectory = "c:\\";
            saveDialog.Filter = "All files (*.*)|*.*";            

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                return new FileInfo(saveDialog.FileName);
            }

            throw new Exception("Save does not work");
        }

        public Vector2 GetCurrentMousePos()
        {
            var p = pbFieldCanvas.PointToClient(MousePosition);
            var loc = GetVectorFromPoint(p);
            return loc;
        }
        public Vector2 GetVectorFromPoint(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        void IRefreshableView.RefreshView()
        {
            pbFieldCanvas.Refresh();

            rendered = null;
            renderHeightMapWithBrush(pbHeightmapCanvas.CreateGraphics(), HeightMapPresenter.CurrentMousePosition, HeightMapPresenter.BrushSize);
            pbFieldCanvas.Focus();
        }
        


        void IMasterView.RefreshView()
        {
            ((IFieldView)this).RefreshView();
            ((IHeightmapView)this).RefreshView();
        }

        
        private void bindMasterControls()
        {
            binder.Bind(MasterPreseneter.CommandLoadProject, tsmiLoadProject);
            binder.Bind(MasterPreseneter.CommandSaveProject, tsmiSave);
            binder.Bind(MasterPreseneter.CommandExportToTxt, tsmiExportToTxtFile);
            binder.Bind(MasterPreseneter.CommandShowHelp, tsmiAbout);
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (selectNextControl(sender))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private bool selectNextControl(object sender)
        {
            var ctr = sender as Control;
            if (ctr == null) return false;
            SelectNextControl(ctr, true, true, true, true);
            return true;
        }

        private void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            selectNextControl(sender);
        }


        public void ShowAboutForm()
        {
            FrmAbout fa = new FrmAbout();
            fa.ShowDialog(this);
        }

        void IFieldView.RefreshView()
        {
            pbFieldCanvas.Refresh();
        }
        public void ShowInfoOKMessage(string v)
        {
            MessageBox.Show(v,"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowErrorOKMessage(string v)
        {
            MessageBox.Show(v, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        
       
        public bool ShowInfoYesNoMessage(string caption, string text)
        {
            return MessageBox.Show(text,caption, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes;
        }

        bool IFieldView.IsFieldViewSelected()
        {
            return tcTabControl.SelectedTab == tpFieldTab;
        }

        bool IHeightmapView.IsHeightmapViewSelected()
        {
            return tcTabControl.SelectedTab == tpHeightmapTab;
        }

        private void focus_pbFieldCanvas(object sender, MouseEventArgs e)
        {
            // don't use this method!
            pbFieldCanvas.Focus();
        }

    }
}