using OpenTK;
using System;
using System.Collections.Generic;
using TerraSketch.Layer;

namespace TerraSketch.VisualPresenters
{

    class MapGfxObject : AGfxObject
    {
        Vector3[] vertices = new Vector3[0];
        Vector3[] colors = new Vector3[0];
        Vector2[] texturecoords = new Vector2[0];
        int[] indices;

        public override int VertCount { get { return vertices.Length; } }

        public override int IndiceCount { get { return indices.Length; } }
        public override int ColorDataCount { get { return colors.Length; } }

        private MapGfxObject() { }

        public override Vector3[] GetVerts() { return vertices; }

        public override int[] GetIndices(int offset = 0) {
            if (offset != 0) throw new NotImplementedException("not implemented. I don't know what to do when offset != 0.");
            return indices; }

        public override Vector3[] GetColorData() { return colors; }
        public override Vector2[] GetTextureCoords() { return texturecoords; }



        /// <summary>
        /// Calculates the model matrix from transforms
        /// </summary>
        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        public static MapGfxObject LoadFromLayer(ILayer l, int step, float min, float max)
        {
            #region map
            float heightCoef = 1f;
            float scale = ((max - min) * heightCoef);

            int shiftX = 0;
            int shiftY = 0;
            float shittZ = 0;
            // TODO optimize for arrays
            shittZ = min * heightCoef;
            int yMax = l.Resolution.Y  - 1;
            int xMax = l.Resolution.X  - 1;
            int vCount = yMax * xMax;
            MapGfxObject map = new MapGfxObject();
            // HACK I - add one
            var verts = new List<Vector3>() {  };            
            var cols = new List<Vector3>() {   };            
            var texs = new List<Vector2>() {   };            
            var inc = new List<int>() { 0};
            var gray = new Vector3(.66f, .66f, .66f);




            for (int x = 0; x < xMax+1; x++)
            {
                var topHeight = getVal(l[x, 0], scale, 0);
                var top = new Vector3(x, topHeight, 0);
                verts.Add(top);
                texs.Add(new Vector2(x, 0));
                cols.Add(gray);
            }

            for (int y = 1; y < yMax+1; y++)
            {
                var firstInARowHeight = getVal(l[0, y], scale, 0);
                var firstInARow = new Vector3(0, firstInARowHeight, y);
                verts.Add(firstInARow);
                texs.Add(new Vector2(0, y));
                cols.Add(gray);

                for (int x = 1; x < xMax+1; x++)
                {
                    var currentHeight = getVal(l[x,y], scale, 0);

                    var tr = new Vector3(x, currentHeight, y);       // Top Left Of The Quad (Top)
                    verts.Add(tr);
                    texs.Add(new Vector2(x, y));
                    cols.Add(gray);
                    
                    var tlIndex = Index(x - 1, y - 1, xMax+1);
                    var trIndex = Index(x , y - 1, xMax+1);//current
                    var blIndex = Index(x - 1, y , xMax+1);
                    var brIndex = Index(x , y , xMax+1);

                    inc.Add(tlIndex);
                    inc.Add(brIndex);
                    inc.Add(blIndex);

                    inc.Add(tlIndex);
                    inc.Add(trIndex);
                    inc.Add(brIndex);


                }
            }
           
            #region old
            //for (int y = 0; y < yMax; y++)
            //{
            //    for (int x = 0; x < xMax; x++)
            //    {
            //        var trHeight = getVal(l[(x + 1) , (y) ], scale, 0) + shittZ;
            //        var tlHeight = getVal(l[(x) , (y) ], scale, 0) + shittZ;
            //        var blHeight = getVal(l[(x) , (y + 1) ], scale, 0) + shittZ;
            //        var brHeight = getVal(l[(x + 1) , (y + 1) ], scale, 0) + shittZ;
            //        float xCor = x  + shiftX;
            //        float yCor = y  + shiftY;


            //        var tl = new Vector3(xCor, tlHeight, yCor);       // Top Left Of The Quad (Top)
            //        var br = new Vector3(xCor + step, brHeight, yCor + step); // Bottom Right Of The Quad (Top)
            //        var tr = new Vector3(xCor + step, trHeight, yCor);        // Top Right Of The Quad (Top)
            //        var bl = new Vector3(xCor, blHeight, yCor + step);        // Bottom Left Of The Quad (Top)

            //        int iCount = verts.Count;
            //        verts.Add(tl);
            //        verts.Add(tr);
            //        verts.Add(br);
            //        verts.Add(bl);

            //        inc.Add(iCount);
            //        inc.Add(iCount +1);
            //        inc.Add(iCount + 2);

            //        inc.Add(iCount +3);
            //        inc.Add(iCount );
            //        inc.Add(iCount + 2);

            //        texs.Add(new Vector2(x, y));
            //        texs.Add(new Vector2(x + step, y + step));
            //        texs.Add(new Vector2(x + step, y));
            //        texs.Add(new Vector2(x, y+ step));

            //        cols.Add(gray);
            //        cols.Add(gray);
            //        cols.Add(gray);
            //        cols.Add(gray);
            //    }
            //}
            //// HACK II
            //verts.Add(new Vector3());
            //verts.Add(new Vector3());

            //texs.Add(new Vector2());
            //texs.Add(new Vector2());

            //inc.Add(0);
            //inc.Add(0);

            //cols.Add(new Vector3());
            //cols.Add(new Vector3()); 
            #endregion

            map.vertices = verts.ToArray();
            map.colors = cols.ToArray();
            map.indices = inc.ToArray();
            map.texturecoords = texs.ToArray();
            //map.CalculateNormals();
            #endregion
            return map;
        }

        private static int Index(int x, int y, int yMax)
        {
            return (y*yMax)+ x;
        }

        private static float getVal(float? sourceValue, float scale, int defaultValue)
        {
            if (sourceValue.HasValue)
                return sourceValue.Value * scale;
            return defaultValue;
        }

    }
}

