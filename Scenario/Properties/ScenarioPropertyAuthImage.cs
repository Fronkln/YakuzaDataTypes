using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyAuthImage : ScenarioProperty
    {
        public string Image;

        internal override void Read(DataReader reader)
        {
            Image = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(Image));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
