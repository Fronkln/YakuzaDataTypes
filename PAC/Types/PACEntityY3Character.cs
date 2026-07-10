using System.Collections;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.PAC
{
    public class PACEntityY3Character : BasePACEntity
    {
        public string AIChip;
        public string Model;
        public string IdleAnimation;

        internal override void ProcessEntityData(DataReader reader, string[] stringTable)
        {
            base.ProcessEntityData(reader, stringTable);

            int aiChipIdx = reader.ReadInt32();
            int modelIdx = reader.ReadInt32();
            int idleAnimationIdx = reader.ReadInt32();

            AIChip = stringTable[aiChipIdx];
            Model = stringTable[modelIdx];
            IdleAnimation = stringTable[idleAnimationIdx];
        }
    }

}
