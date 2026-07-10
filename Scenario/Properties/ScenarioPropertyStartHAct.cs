using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyStartHAct : ScenarioProperty
    {
        public string HAct;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);
            HAct = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(HAct));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
