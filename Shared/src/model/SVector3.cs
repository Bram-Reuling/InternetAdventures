namespace Shared
{
    // Special server vector class to transfer vectors
    public class SVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SVector3()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public SVector3(float pX, float pY)
        {
            X = pX;
            Y = pY;
            Z = 0;
        }

        public SVector3(float pX, float pY, float pZ)
        {
            X = pX;
            Y = pY;
            Z = pZ;
        }
    }
}