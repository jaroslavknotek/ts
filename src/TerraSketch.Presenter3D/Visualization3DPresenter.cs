using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TerraSketch.Layer;

namespace TerraSketch.VisualPresenters
{

    public partial class Visualization3DPresenter
    {
        private const float fow = 1500.0f;
        private int _width, _height;
        private LayerConfig _layer;

        public Visualization3DPresenter(IVisual3DView view)
        {
            this.view = view;
            ogw = new OpenGLWrapper();

            var defCenter = new OpenTK.Vector3(0, 5, 0);
            Camera = new FpsCamera();
            Camera.DefaultCenter = defCenter;
            Camera.Reset();
        }


        /***************************************************************************************
        *    Title: LoadOpenGl
        *    Author: Josef Pelikan
        *    Date: unknown
        *    Code version: unknown
        *    Availability: http://cgg.mff.cuni.cz/~pepca/grcis/index.en.php - project - 038 trackball
        *
        ***************************************************************************************/

        public void LoadOpenGL(int width, int height)
        {
            UpdateHeightWidth(width, height);
            // OpenGL init code:
            ogw.Enable(EnableCap.DepthTest);
            ogw.ShadeModel(ShadingModel.Flat);
            ogw.MatrixMode(MatrixMode.Projection);
            
            ogw.PointSize(5f);

            initProgram();
            IsLoaded = true;
        }
        void initProgram()
        {
            ogw.GenBuffers(1, out ibo_elements);

            // Load shaders from file
            shaders.Add("default", new ShaderProgram(shaderPath + "vs.glsl", shaderPath + "fs.glsl", true));
            shaders.Add("textured", new ShaderProgram(shaderPath + "vs_tex.glsl", shaderPath + "fs_tex.glsl", true));
            shaders.Add("normal", new ShaderProgram(shaderPath + "vs_norm.glsl", shaderPath + "fs_norm.glsl", true));

            activeShader = "normal";


            loadMaterials(materialPath + "opentk.mtl");


        }


        public void UpdateHeightWidth(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public void UpdateViewport()
        {
            ogw.Viewport(0, 0, _width, _height);
            var p = Matrix4.CreatePerspectiveFieldOfView(Camera.Fov, _width /(float)_height, 0.1f, Camera.Far);
            ogw.MatrixMode(MatrixMode.Projection);
            ogw.LoadMatrix(ref p);
            PerspectiveProjection = p;
        }


        /***************************************************************************************
       *    Title: Render inners
       *    Author: Josef Pelikan
       *    Date: unknown
       *    Code version: unknown
       *    Availability: http://cgg.mff.cuni.cz/~pepca/grcis/index.en.php - project - 038 trackball
       *
       ***************************************************************************************/

        public void Render(ILayer l, int step, float min, float max, long version)
        {
            if (!IsLoaded) return;
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            // update 
            updateLayer(l, step, min, max,version);
            OnUpdateFrame();

            shaders[activeShader].EnableVertexAttribArrays();

            var rot = Matrix4.Identity;
            int iCount = 0;
            if (mapGfxObject != null)
            {
                mapGfxObject.CalculateModelMatrix();
                mapGfxObject.ViewProjectionMatrix = Camera.CreateModelViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, _width / (float)_height, 1.0f,fow);
                mapGfxObject.ModelViewProjectionMatrix = mapGfxObject.ModelMatrix * mapGfxObject.ViewProjectionMatrix;

                iCount = this.mapGfxObject.IndiceCount;
                rot = this.mapGfxObject.ModelViewProjectionMatrix;                
            }
            
            GL.UniformMatrix4(shaders[activeShader].GetUniform("modelview"), false, ref rot);
            GL.DrawElements(BeginMode.Triangles, iCount, DrawElementsType.UnsignedInt, sizeof(uint));
            shaders[activeShader].DisableVertexAttribArrays();
        }

        /// <summary>
        /// Loads all materials in a file, along with their required maps.
        /// Materials will not overwrite existing materials with the same name.
        /// </summary>
        /// <param name="filename">MTL file to load from</param>
        private void loadMaterials(string filename)
        {
            var file = Material.LoadFromFile(filename);
            foreach (var mat in file)
            {
                if (!materials.ContainsKey(mat.Key))
                {
                    materials.Add(mat.Key, mat.Value);
                }
            }

            //// Load textures
            //foreach (Material mat in materials.Values)
            //{
            //    if (File.Exists(mat.AmbientMap) && !textures.ContainsKey(mat.AmbientMap))
            //    {
            //        textures.Add(mat.AmbientMap, loadImage(mat.AmbientMap));
            //    }

            //    if (File.Exists(mat.DiffuseMap) && !textures.ContainsKey(mat.DiffuseMap))
            //    {
            //        textures.Add(mat.DiffuseMap, loadImage(mat.DiffuseMap));
            //    }

            //    if (File.Exists(mat.SpecularMap) && !textures.ContainsKey(mat.SpecularMap))
            //    {
            //        textures.Add(mat.SpecularMap, loadImage(mat.SpecularMap));
            //    }

            //    if (File.Exists(mat.NormalMap) && !textures.ContainsKey(mat.NormalMap))
            //    {
            //        textures.Add(mat.NormalMap, loadImage(mat.NormalMap));
            //    }

            //    if (File.Exists(mat.OpacityMap) && !textures.ContainsKey(mat.OpacityMap))
            //    {
            //        textures.Add(mat.OpacityMap, loadImage(mat.OpacityMap));
            //    }
            //}
        }


        /***************************************************************************************
       *    Title: OnUpdateFrame
       *    Author: Josef Pelikan
       *    Date: unknown
       *    Code version: unknown
       *    Availability: http://cgg.mff.cuni.cz/~pepca/grcis/index.en.php - project - 038 trackball
       *
       ***************************************************************************************/
        protected void OnUpdateFrame()
        {
            if (mapGfxObject == null) return;
            // Assemble vertex and indice data for all volumes

            vertdata = mapGfxObject.GetVerts();
            indicedata = mapGfxObject.GetIndices();
            coldata = mapGfxObject.GetColorData();
            texcoorddata = mapGfxObject.GetTextureCoords();
            normdata = mapGfxObject.GetNormals();



            var vPosBuffer = shaders[activeShader].GetBuffer("vPosition");
            var vPosAtt = shaders[activeShader].GetAttribute("vPosition");
            ogw.BindBuffer(BufferTarget.ArrayBuffer, vPosBuffer);

            ogw.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            ogw.VertexAttribPointer(vPosAtt, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Buffer vertex color if shader supports it
            if (shaders[activeShader].GetAttribute("vColor") != -1)
            {
                ogw.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vColor"));
                ogw.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(coldata.Length * Vector3.SizeInBytes), coldata, BufferUsageHint.StaticDraw);
                ogw.VertexAttribPointer(shaders[activeShader].GetAttribute("vColor"), 3, VertexAttribPointerType.Float, true, 0, 0);
            }


            // Buffer texture coordinates if shader supports it
            if (shaders[activeShader].GetAttribute("texcoord") != -1)
            {
                ogw.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("texcoord"));
                ogw.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(texcoorddata.Length * Vector2.SizeInBytes), texcoorddata, BufferUsageHint.StaticDraw);
                ogw.VertexAttribPointer(shaders[activeShader].GetAttribute("texcoord"), 2, VertexAttribPointerType.Float, true, 0, 0);
            }

            if (shaders[activeShader].GetAttribute("vNormal") != -1)
            {
                ogw.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vNormal"));
                ogw.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normdata.Length * Vector3.SizeInBytes), normdata, BufferUsageHint.StaticDraw);
                ogw.VertexAttribPointer(shaders[activeShader].GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, true, 0, 0);
            }


            ogw.UseProgram(shaders[activeShader].ProgramID);

            ogw.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Buffer index data
            ogw.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            ogw.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicedata.Length * sizeof(int)), indicedata, BufferUsageHint.StaticDraw);



        }

        
        public void ResetCamera()
        {
            Camera.Reset();
        }


        private void updateLayer(ILayer l, int step, float min, float max, long version)
        {
            var lc = new LayerConfig(l, step, min, max, version);
            if (l == null || _layer.Equals(lc))
            {
                return;
            }

            _layer = lc;
            mapGfxObject = MapGfxObject.LoadFromLayer(l, step, min, max);
            mapGfxObject.CalculateNormals();
        }


        private struct LayerConfig
        {
            public int Size { get; private set; }
            public ILayer Layer { get; private set; }
            public float Min { get; private set; }
            public float Max { get; private set; }
            public long Version { get; private set; }
            public LayerConfig(ILayer l, int size, float min, float max, long version) : this()
            {
                Size = size;
                Layer = l;
                Min = min;
                Max = max;
                Version = version;
            }
        }
    }

}

