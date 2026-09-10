using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioStateOE
    {
        public static ScenarioStateOE Read(string path)
        {
            var state = new ScenarioStateOE();

            byte[] fileBuffer = File.ReadAllBytes(path);

            var readStream = DataStreamFactory.FromArray(fileBuffer, 0, fileBuffer.Length);
            var reader = new DataReader(readStream) { DefaultEncoding = Encoding.GetEncoding(932), Endianness = EndiannessMode.BigEndian };

            //TODO:

            return state;
        }
    }
}
