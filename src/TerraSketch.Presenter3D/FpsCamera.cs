using OpenTK;
using Common.MathUtils;

namespace TerraSketch.VisualPresenters
{

    public interface ICamera
    {
        Vector3 Center { get; set; }

        float Fov { get; set; }
        float Far { get; set; }
        float Zoom { get; set; }

        Vector3 Dir { get;}

        Vector3 DefaultCenter { get; set; }

        Matrix4 CreateModelViewMatrix();

        void GoUp();
        void GoDown();
        void GoBack();
        void GoFront();
        void GoRight();
        void GoLeft();
        void Reset();
        void BeginRotation();
        void UpdateRotation(int x, int y, float sens);
        void EndRotation();
    }
    public class FpsCamera : ICamera
    {
        public Vector3 Up { get; set; }
        public Vector3 Aside { get; set; }
        public Vector3 Dir { get; set; }
        public Vector3 Center { get; set; }
        public Vector3 DefaultCenter { get; set; }
        public Matrix4 Rotation { get; set; }
        
        public float Speed { get; set; }
        public float Fov { get; set; }
        public float Far { get; set; }
        public float Zoom { get; set; }
        public float Diameter { get; set; }
        public FpsCamera()
        {
            Speed = 5;
            Center = Vector3.Zero;
            Up = Vector3.UnitZ;
            Dir = Vector3.UnitY;
            Aside = Vector3.UnitX;
            Fov = 1.0f;
            Rotation = Matrix4.Identity;
            Far = 200;
            Diameter = 4;
            Zoom = 1;
        }

        public Matrix4 CreateModelViewMatrix()
        {
            return Matrix4.CreateTranslation(-Center) *
                                Matrix4.CreateScale(Zoom / Diameter) *
                                Rotation;
        }

        private Vector3 speedup(Vector3 dir)
        {
            return dir * Speed;
        }
        public void GoUp()
        {
            Center += speedup(Up);
        }


        public void GoDown()
        {
            Center -= speedup(Up);
        }

        public void GoBack()
        {
            Center -= speedup(Dir);
        }

        public void GoFront()
        {
            Center += speedup(Dir);
        }

        public void GoRight()
        {
            Center -= speedup(Aside);
        }

        public void GoLeft()
        {
            Center += speedup(Aside);
        }

        public void Reset()
        {
            Center = DefaultCenter;

            Up = Vector3.UnitY;
            Dir = Vector3.UnitZ;
            Aside = Vector3.UnitX;

            Rotation = Matrix4.CreateFromAxisAngle(Up, JryMath.Pi);
            previousYaw = JryMath.Pi;
            currentYaw = JryMath.Pi;
            previousPitch = 0;
            currentPitch = 0;
        }
        private float previousYaw,previousPitch, currentYaw, currentPitch;

        public void BeginRotation()
        {
            previousYaw = currentYaw;
            previousPitch = currentPitch;
        }
        public void UpdateRotation(int x, int y, float sens)
        {           
           
            float yShift = sens*.001f * y;
            float xShift = sens * .001f * -x;

            currentPitch = previousPitch - yShift;
            if(currentPitch> JryMath.Pi /2)
                currentPitch = JryMath.Pi / 2 ;
            if (currentPitch <-JryMath.Pi / 2)
                currentPitch = -JryMath.Pi / 2;

            currentYaw = previousYaw + xShift;

            Rotation = Matrix4.CreateRotationY(currentYaw) * Matrix4.CreateRotationX(currentPitch);

            Dir = rotVec(-Vector3.UnitZ, Rotation);
            Up = rotVec(Vector3.UnitY, Rotation);
            Aside = Vector3.Cross(Up,Dir);
            
        }
        public void EndRotation()
        {
            
        }

        private float decideAngle(float x, float y)
        {
            if (x >= 0 && y >= 0)
                return JryMath.Acos(x);
            else if (x >= 0)
                return JryMath.Pi * 2 - JryMath.Acos(x);
            else if (y >= 0)
                return JryMath.Acos(x);
            else
                return JryMath.Pi * 2 - JryMath.Acos(x);
        }

        private Vector3 rotVec(Vector3 v, Matrix4 rot)
        {

            return new Vector3(vmv(v, rot.Row0.Xyz), vmv(v, rot.Row1.Xyz), vmv(v, rot.Row2.Xyz));
        }
        private float vmv(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
    }
}
