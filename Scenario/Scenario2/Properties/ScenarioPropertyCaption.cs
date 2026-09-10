using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyCaption : ScenarioProperty
    {
        public string Caption;

        internal override void Read(DataReader reader)
        {
            Caption = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(Caption));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
