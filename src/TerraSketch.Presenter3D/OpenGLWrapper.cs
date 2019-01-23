using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TerraSketch.VisualPresenters
{
    /***************************************************************************************
*    Title: OpenGlWrapper
*    Author: Josef Pelikan
*    Date: unknown
*    Code version: unknown
*    Availability: http://cgg.mff.cuni.cz/~pepca/grcis/index.en.php - project - 038 trackball
*
***************************************************************************************/
    public class OpenGLWrapper
    {
        public bool IsNoError()
        {
            return GL.GetError() != ErrorCode.NoError;
        }

        public void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }

        public void MatrixMode(MatrixMode projection)
        {
            GL.MatrixMode(projection);
        }

        public void ClearColor(float r, float g,float b, float a)
        {
            GL.ClearColor(r,g,b,a);
        }

        public void GenBuffers(int v, uint[] vboId)
        {
            GL.GenBuffers(v, vboId);
        }
        public void GenBuffers(int v, out int vboId)
        {
            GL.GenBuffers(v,out vboId);
        }

        public void LoadMatrix(ref Matrix4 p)
        {
            GL.LoadMatrix(ref p);
        }

        public void Clear(ClearBufferMask clearBufferMask)
        {
            GL.Clear(clearBufferMask);
        }

        public void ShadeModel(ShadingModel shadingModel)
        {
            GL.ShadeModel(shadingModel);
        }

        public void PointSize(float v)
        {
            GL.PointSize(v);
        }

        public void PolygonMode(MaterialFace materialFace, PolygonMode polygonMode)
        {
            GL.PolygonMode(materialFace, polygonMode);
        }

        public void Disable(EnableCap cullFace)
        {
            GL.Disable(cullFace);
        }

        public void Enable(EnableCap cullFace)
        {
            GL.Enable(cullFace);
        }

        public void Begin(PrimitiveType quads)
        {
            GL.Begin(quads);
        }

        public void End()
        {
            GL.End();
        }

        public void Vertex3(float v1, float br, float v2)
        {
            GL.Vertex3(v1, br, v2);
        }

        public void Color3(float v1, float v2, float v3)
        {
            GL.Color3(v1, v2, v3);
        }

        public void BindBuffer(BufferTarget elementArrayBuffer, uint ibo_elements)
        {
            GL.BindBuffer(elementArrayBuffer, ibo_elements);
        }
        public void BindBuffer(BufferTarget elementArrayBuffer, int ibo_elements)
        {
            GL.BindBuffer(elementArrayBuffer, ibo_elements);
        }

        public void UseProgram(int programID)
        {
            GL.UseProgram(programID);
        }

        public void BufferData(BufferTarget target, IntPtr intPtr, int[] indicedata, BufferUsageHint staticDraw)
        {
            GL.BufferData(target, intPtr, indicedata, staticDraw);
        }

        public void BufferData(BufferTarget arrayBuffer, IntPtr intPtr, Vector3[] vertdata, BufferUsageHint staticDraw)
        {
            GL.BufferData<Vector3>(arrayBuffer, intPtr, vertdata, staticDraw);
        }
        public void BufferData(BufferTarget arrayBuffer, IntPtr intPtr, Vector2[] vertdata, BufferUsageHint staticDraw)
        {
            GL.BufferData<Vector2>(arrayBuffer, intPtr, vertdata, staticDraw);
        }

        internal void VertexAttribPointer(int index, int size, VertexAttribPointerType vapType, bool normalized, int stride, int offset)
        {
            GL.VertexAttribPointer(index, size, vapType, normalized, stride, offset);
        }
    }
}
