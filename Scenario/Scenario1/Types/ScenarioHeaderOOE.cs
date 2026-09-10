using Yarhl.IO.Serialization.Attributes;

namespace YakuzaDataTypes.Scenario
{
    [Serializable]
    public class ScenarioHeaderOOE
    {
        public SizedPointer Collections { get; set; }
        public SizedPointer Events { get; set; }
        public SizedPointer SequenceGroups { get; set; }
        public SizedPointer UnkDataY4 { get; set; }
    }
}
