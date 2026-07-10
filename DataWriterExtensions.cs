using Yarhl.IO;

public static class DataWriterExtensions
{   
    public static void WritePXDVector3(this DataWriter writer, PXDVector3 vec)
    {
        writer.Write(vec.x);
        writer.Write(vec.y);
        writer.Write(vec.z);
    }
}
