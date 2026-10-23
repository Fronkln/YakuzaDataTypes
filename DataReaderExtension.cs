using Yarhl.IO;

public static class DataReaderExtension
{
    /// <summary>
    /// Simply push the reader stream forward by N bytes.
    /// 
    /// This is different to SkipPadding(), which pads the current stream position out to a multiple of the argument.
    /// For example, if a stream were at 0xec0 and called SkipAhead(4) it would go to 0xec4,
    /// but if it called SkipPadding it would stay at 0xec0 because that is divisible by 4.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="bytes"></param>
    public static void SkipAhead(this DataReader reader, int bytes)
    {
        reader.Stream.Seek(bytes, SeekMode.Current);
    }

    /// <summary>
    /// Read PXDVector3 (Vectgr3 that will have its X flipped when converted to Unity)
    /// </summary>
    public static PXDVector3 ReadPXDVector3(this DataReader reader)
    {
        return new PXDVector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static string ReadStringPointer(this DataReader reader)
    {
        int pointer = reader.ReadInt32();

        if (pointer <= 0)
            return null;
        else
        {
            string str = null;

            reader.Stream.RunInPosition(delegate
            {
                str = reader.ReadString();
            }, pointer);

            return str;
        }
    }

    public static string ReadStringPointer(this DataReader reader, int address)
    {
        string str = null;

        reader.Stream.RunInPosition(delegate
        {
            str = reader.ReadStringPointer();
        }, address);

        return str;
    }


    public static string ReadString(this DataReader reader, long pointer)
    {
        if (pointer <= 0)
            return null;
        else
        {
            string str = null;

            reader.Stream.RunInPosition(delegate
            {
                str = reader.ReadString();
            }, pointer);

            return str;
        }
    }
}