namespace YakuzaDataTypes.MSG
{
    public enum MsgPropTypeY0 : short
    {
        Start = 0x100,
        DialogueSettings = 0x101,
        ScenarioProgress = 0x105,
        Choice = 0x112,
        ChoiceBranching = 0x113,
        MoneyBranch = 0x11B,
        SetSpeaker = 0x200,
        SpeechPause = 0x209,
        UnknownPropWithString1 = 0x301,
        PlayActionSetAnimation = 0x309,
        PlayAnimationRelated = 0x30A,
        SetVisibility = 0x311,
        PlayAnimation = 0x313,
        SetIdleAnimation = 0x314,
        TeleportToMsgCoordinate = 0x315,
        FaceEntity = 0x316,
        FaceExpression = 0x318,
        Reactor = 0x31A,
        MoneyChange = 0x328,
        TalkVoice = 0x332,
        PlayBgm = 0x333,
        PlayDialogueSound = 0x335,
        SetTalkCameraType = 0x31E,
        CameraParam = 0x31F,
    }
}
