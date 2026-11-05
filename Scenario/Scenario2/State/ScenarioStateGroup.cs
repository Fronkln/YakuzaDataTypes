namespace YakuzaDataTypes.Scenario
{
    public class ScenarioStateGroup
    {
        public string Name = "";
        public List<ScenarioStateCategory> Categories = new List<ScenarioStateCategory>();
    
        public override string ToString()
        {
            return Name;
        }
    }
}
