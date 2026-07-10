using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.PAC
{
    public class PAC
    {
        public List<PACEntityY5> Entities = new List<PACEntityY5>();

        public static PAC Read(string path, Game game)
        {
            return PACImporter.Read(path, game);
        }

        public static PAC Read(byte[] buf, Game game)
        {
            return PACImporter.Read(buf, game);
        }
    }
}
