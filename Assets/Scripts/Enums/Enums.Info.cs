public static partial class Enums
{
    public static string ToCustomString(this SCENE_SCENES _Value) => _Value switch
    {
        //SCENE_SCENES.Grove_Of_Scrap => "Grove_Of_Scrap",
        //SCENE_SCENES.BamserMap_Test => "BamserMap_Test",
        //INFO_OPTION_DATA.SoundData => "soundData",
        //INFO_OPTION_DATA.MouseData => "mouseData",
        //INFO_OPTION_DATA.GraphicData => "graphicData",
        //INFO_OPTION_DATA.GameData => "gameData",
        //INFO_OPTION_DATA.VideoData => "videoData",
        _ => null
    };
    public static string ToCustomString(this INFO_OPTION_DATA _Value) => _Value switch
    {
        INFO_OPTION_DATA.KeyData => "keyData",
        INFO_OPTION_DATA.SoundData => "soundData",
        INFO_OPTION_DATA.MouseData => "mouseData",
        INFO_OPTION_DATA.GraphicData => "graphicData",
        INFO_OPTION_DATA.GameData => "gameData",
        INFO_OPTION_DATA.VideoData => "videoData",
        _ => null
    };
    public static string ToCustomString(this INFO_CONFIG_DATA _Value) => _Value switch
    {
        INFO_CONFIG_DATA.ResolutionList => "resolutionList",
        INFO_CONFIG_DATA.ColorSupportList => "colorSupportList",
        _ => null
    };
    public static string ToCustomString(this INFO_STATIC_DATA _Value) => _Value switch
    {
        INFO_STATIC_DATA.MonsterData => "monsterData",
        INFO_STATIC_DATA.MonsterAttackType => "monsterAttackType",
        INFO_STATIC_DATA.MonsterState => "monsterState",
        INFO_STATIC_DATA.Item => "itemData",
        INFO_STATIC_DATA.Quest => "questData",
        INFO_STATIC_DATA.Skill => "skillData",
        INFO_STATIC_DATA.Stage => "stageData",
        INFO_STATIC_DATA.Book => "bookData",
        INFO_STATIC_DATA.Buff => "buffData",
        INFO_STATIC_DATA.Reward => "rewardData",
        INFO_STATIC_DATA.Combination => "combinationData",
        INFO_STATIC_DATA.Character => "characterData",
        INFO_STATIC_DATA.State => "stateData",
        _ => null
    };
    public enum INFO_OPTION_DATA { KeyData, SoundData, MouseData, GraphicData, GameData, VideoData }
    public enum INFO_CONFIG_DATA 
    {
        ResolutionList,
        ColorSupportList,
        
    }
    public enum INFO_STATIC_DATA 
    {
        MonsterData, 
        MonsterAttackType,
        MonsterState,
        //new
        Item,
        Quest,
        Skill,
        Stage,
        Character,
        State,
        Book,
        Buff,
        Reward,
        Combination,
    }
}