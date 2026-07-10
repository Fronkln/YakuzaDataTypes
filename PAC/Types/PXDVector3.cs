public struct PXDVector3
{
    public float x; 
    public float y; 
    public float z;

    public PXDVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static PXDVector3 FromUnityVector(float x, float y, float z)
    {
        return new PXDVector3(-x, y, z);
    }
}
