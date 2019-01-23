using System;
using System.IO;
using System.Xml;
using TerraSketch.DataObjects.Export;
using TerraSketch.DataObjects.SaveLoad;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.Presenters
{

    public partial class MasterPresenter
    {

        #region ShowHelp
        
        public ICommandWrapper CommandShowHelp
        {
            get { return new CommandWrapper((object o) => CanExecuteCommandShowHelp(o), (object o) => ExecuteCommandShowHelp(o)); }
        }
        bool CanExecuteCommandShowHelp(object o)
        {
            return true;
        }

        void ExecuteCommandShowHelp(object o)
        {
            MasterView.ShowAboutForm();
        }

        #endregion

        #region export


        private ICommandWrapper _commandExportToTxt = null;
        
        public ICommandWrapper CommandExportToTxt
        {
            get
            {
                if (_commandExportToTxt == null)
                    _commandExportToTxt = new CommandWrapper(
                        CanExecuteCommandExportToTxt,
                        (o) => ExecuteCommandExportToTxt(o)
                        );
                return _commandExportToTxt;
            }
        }
        bool CanExecuteCommandExportToTxt(object o)
        {
            return true;
        }

        void ExecuteCommandExportToTxt(object o)
        {
            if (HeightmapPresenter.HeightmapLayer == null)
            {
                MasterView.ShowErrorOKMessage("No heightmap rendered yet");
                return;
            }

            try
            {


                IExportManager exportMgr = new ExportManager();

                var path = MasterView.GetSaveFilePath();
                var layer = this.HeightmapPresenter.HeightmapLayer;
                exportMgr.ExportToFile(path, layer, MinHeight, MaxHeight);
                MasterView.ShowInfoOKMessage("Exported");
            }
            catch (IOException e)
            {
                MasterView.ShowErrorOKMessage(e.Message);
            }





        }

        #endregion

        #region Load


        private ICommandWrapper _commandLoadProject = null;
        
        public ICommandWrapper CommandLoadProject
        {
            get
            {
                if (_commandLoadProject == null)
                    _commandLoadProject = new CommandWrapper(
                        CanExecuteCommandLoadProject,
                        (o) => ExecuteCommandLoadProject(o)
             );
            return
            _commandLoadProject;
            }
        }
        bool CanExecuteCommandLoadProject(object o)
        {
            return true;
        }

        void ExecuteCommandLoadProject(object o)
        {
            try
            {
                var path = MasterView.GetLoadFilePath();

                var si = SaveLoadManager.Load(path);
                World = si.World;
                FieldPresenter.UnselectField();
                //HeightmapPresenter = si.Layer;
                
                MasterView.RefreshView();
                MasterView.ShowInfoOKMessage("Loaded");
            }
            catch (XmlException xe)
            {
                MasterView.ShowErrorOKMessage("Not loaded due to xml error: " + xe.Message);
            }
            catch (InvalidOperationException ioe)
            {
                MasterView.ShowErrorOKMessage("Not loaded due to Invalid Operation: " + ioe.Message);
            }
            catch (IOException ie)
            {
                MasterView.ShowErrorOKMessage("Not loaded due to IO error: " + ie.Message);
            }
            catch (Exception e)
            {
                MasterView.ShowErrorOKMessage("Not loaded due to: " + e.Message);
            }
        }

        #endregion

        #region Save
        private ICommandWrapper _commandSaveProject = null;
        
        public ICommandWrapper CommandSaveProject
        {
            get
            {
                if (_commandSaveProject == null)
                    _commandSaveProject = new CommandWrapper((object o) => CanExecuteCommandSaveProject(o), (object o) => ExecuteCommandSaveProject(o));
                return _commandSaveProject;

            }
        }

       
        bool CanExecuteCommandSaveProject(object o)
        {
            return true;
        }

        void ExecuteCommandSaveProject(object o)
        {
            try
            {
                var si = new SaveItem();
                si.Layer = HeightmapPresenter.HeightmapLayer;
                si.World = FieldPresenter.World;

                var sPath = MasterView.GetSaveFilePath();
                SaveLoadManager.Save(si, sPath);
                MasterView.ShowInfoOKMessage("Saved");
            }
            catch (XmlException xe)
            {
                MasterView.ShowErrorOKMessage("Not saved due to xml error: " + xe.Message);
            }
            catch (InvalidOperationException ioe)
            {
                MasterView.ShowErrorOKMessage("Not saved due to Invalid Operation: " + ioe.Message);
            }
            catch (IOException ie)
            {
                MasterView.ShowErrorOKMessage("Not saved due to IO error: " + ie.Message);
            }
            catch (Exception e)
            {
                MasterView.ShowErrorOKMessage("Not saved due to: " + e.Message);
            }
        }

        #endregion
    }
}
