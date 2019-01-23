using OpenTK;

namespace TerraSketch.VisualPresenters
{
    /***************************************************************************************
*    Title: AGfxObject
*    Author: Josef Pelikan
*    Date: unknown
*    Code version: unknown
*    Availability: http://cgg.mff.cuni.cz/~pepca/grcis/index.en.php - project - 038 trackball
*
***************************************************************************************/
    public abstract class AGfxObject
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public virtual int VertCount { get; set; }
        public virtual int IndiceCount { get; set; }
        public virtual int ColorDataCount { get; set; }
        public virtual int NormalCount { get { return Normals.Length; } }

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        Vector3[] Normals = new Vector3[0];

        public abstract Vector3[] GetVerts();
        public abstract int[] GetIndices(int offset = 0);
        public abstract Vector3[] GetColorData();
        public abstract void CalculateModelMatrix();

        public bool IsTextured = false;
        public int TextureID;
        public int TextureCoordsCount;
        public abstract Vector2[] GetTextureCoords();

        public virtual Vector3[] GetNormals()
        {
            return Normals;
        }

        public void CalculateNormals()
        {

            int errorLine = 0;
            try
            {


                Vector3[] normals = new Vector3[VertCount];
                Vector3[] verts = GetVerts();
                int[] inds = GetIndices();

                //// Compute normals for each face
                for (int i = 0; i < IndiceCount-2; i ++)
                {
                    errorLine = i;
                    var in1 = inds[i];
                    var in2 = inds[i + 1];
                    var in3 = inds[i + 2];
                    Vector3 v1 = verts[in1];
                    Vector3 v2 = verts[in2];
                    Vector3 v3 = verts[in3];

                //    // The normal is the cross-product of two sides of the triangle
                    normals[in1] += Vector3.Cross(v2 - v1, v3 - v1);
                    normals[in2] += Vector3.Cross(v2 - v1, v3 - v1);
                    normals[in3] += Vector3.Cross(v2 - v1, v3 - v1);
                }

                for (int i = 0; i < VertCount; i++)
                {
                    normals[i] = normals[i].Normalized();
                }


                #region old
                //Vector3[] normals = new Vector3[VertCount];
                //Vector3[] verts = GetVerts();
                //int[] inds = GetIndices();

                //// Compute normals for each face
                //for (int i = 0; i < IndiceCount; i += 3)
                //{
                //    errorLine = i;
                //    var in1 = inds[i];
                //    var in2 = inds[i + 1];
                //    var in3 = inds[i + 2];
                //    Vector3 v1 = verts[in1];
                //    Vector3 v2 = verts[in2];
                //    Vector3 v3 = verts[in3];

                //    // The normal is the cross-product of two sides of the triangle
                //    normals[in1] += Vector3.Cross(v2 - v1, v3 - v1);
                //    normals[in2] += Vector3.Cross(v2 - v1, v3 - v1);
                //    normals[in3] += Vector3.Cross(v2 - v1, v3 - v1);
                //}

                //for (int i = 0; i < NormalCount; i++)
                //{
                //    normals[i] = normals[i].Normalized();
                //} 
                #endregion

                Normals = normals;
            }
            catch (System.Exception)
            {
                var x = errorLine;
                x++;
                x--;
                throw;
            }
        }
    }
}
