using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YakuzaDataTypes.Scenario
{
    public struct ScenarioSetting
    {
        public int MissionID;
        public string Name;
        public int PlayerPosID;
        public int StageID;

        public List<ScenarioProperty> Properties;
    }
}
