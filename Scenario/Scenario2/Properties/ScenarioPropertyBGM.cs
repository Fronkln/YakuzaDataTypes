using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyBGM : ScenarioProperty
    {
        public string BGM;

        internal override void Read(DataReader reader)
        {
            BGM = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(BGM));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
