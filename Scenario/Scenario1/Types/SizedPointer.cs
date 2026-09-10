using Yarhl.IO.Serialization.Attributes;

namespace YakuzaDataTypes.Scenario
{
    [Serializable]
    public class SizedPointer
    {
        public int Pointer { get; set; }
        public int Size { get; set; }
    }
}
