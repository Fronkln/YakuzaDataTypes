using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes
{
    internal static class Extensions
    {
        public static byte[] ToArray(this DataStream stream)
        {
            long pos = stream.Position;
            stream.Position = 0;

            byte[] buf = new byte[stream.Length];
            stream.Read(buf, 0, buf.Length);

            stream.Position = pos;

            return buf;
        }

        public static string ToLength(this string self, int length)
        {
            if (self == null)
                return null;

            if (self.Length == length)
                return self;

            if (self.Length > length)
                return self.Substring(0, length);

            StringBuilder str = new StringBuilder();
            str.Append(self);

            while (str.Length != length)
                str.Append('\0');

            return str.ToString();
        }

        public static int Align(this DataWriter writer, int alignment)
        {
            int mod = (int)writer.Stream.Position % alignment;

            if (mod == 0)
                return 0;

            int neededBytes = alignment - mod;

            writer.WriteTimes(0, neededBytes);
            return neededBytes;
        }

    }
}
