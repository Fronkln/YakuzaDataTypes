using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyPIBArchive : ScenarioProperty
    {
        public string PIBArchive;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);
            PIBArchive = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(PIBArchive));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
