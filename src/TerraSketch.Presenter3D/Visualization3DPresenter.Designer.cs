using System;
using OpenTK;
using Common.DataObjects.Geometry;
using TerraSketch.Layer;
using System.Collections.Generic;

namespace TerraSketch.VisualPresenters
{
    public partial class Visualization3DPresenter
    {

        Vector3[] vertdata;
        Vector3[] coldata;
        Vector2[] texcoorddata;
        Vector3[] normdata;
        int[] indicedata;

        int ibo_elements;
        Dictionary<String, Material> materials = new Dictionary<string, Material>();
        Dictionary<string, int> textures = new Dictionary<string, int>();

        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();
        string activeShader = "default";

        private string shaderPath = "shaders\\";
        private string materialPath = "materials\\";
        private OpenGLWrapper ogw { get; set; }
        private AGfxObject mapGfxObject = null;
        
        private bool IsLoaded { get; set; }

        IVisual3DView view { get; set; }

        private Matrix4 _perspectiveProjection;
        public Matrix4 PerspectiveProjection
        {
            get { return _perspectiveProjection; }
            set { _perspectiveProjection = value; }
        }

        public ICamera Camera { get; set; }

    }
}
