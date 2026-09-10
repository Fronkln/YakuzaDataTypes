using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyAuth : ScenarioProperty
    {
        public string Auth;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);
            Auth = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(Auth));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
