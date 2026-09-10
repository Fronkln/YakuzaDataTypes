using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyUSM : ScenarioProperty
    {
        public string USM;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);
            USM = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(USM));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
