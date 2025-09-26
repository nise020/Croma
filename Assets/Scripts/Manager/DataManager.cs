using Cysharp.Threading.Tasks;
using UnityEditor;

public class DataManager : Manager_Base
{
    public CharacterTable Character_Table = new CharacterTable();
    public StatTable Stat_Table = new StatTable();
    public ItemTable Item_Table = new ItemTable();
    public BookTable Book_Table = new BookTable();
    public RewardTable Reward_Table = new RewardTable();
    public StageTable Stage_Table = new StageTable();
    public QuestTable Quest_Table = new QuestTable();
    public CombinationTable Combination_Table = new CombinationTable();
    public BuffTable Buff_Table = new BuffTable();
    public SkillTable Skill_Table = new SkillTable();
    public LevelTable Level_Table = new LevelTable();
    public SoundTable Sound_Table = new SoundTable();
    public LanguageTable Language_Table = new LanguageTable();
    //public object CharacterDictionary { get; internal set; }

    public enum TableType
    {
        Item,
        Quest,
        Skill,
        Stage,
        Book,
        Buff,
        Reward,
        Combination,
        Character,
        Stat,
        Level,
        Sound,
        Language,
    }

    public void Init()
    {
#if UNITY_EDITOR
        Character_Table.Init_Csv(TableType.Character.ToString(), 1, 0);
        Stat_Table.Init_Csv(TableType.Stat.ToString(), 1, 0);
        Buff_Table.Init_Csv(TableType.Buff.ToString(), 1, 0);
        Skill_Table.Init_Csv(TableType.Skill.ToString(), 1, 0);
        Item_Table.Init_Csv(TableType.Item.ToString(), 1, 0);
        Book_Table.Init_Csv(TableType.Book.ToString(), 1, 0);
        Reward_Table.Init_Csv(TableType.Reward.ToString(), 1, 0);
        Quest_Table.Init_Csv(TableType.Quest.ToString(), 1, 0);
        Combination_Table.Init_Csv(TableType.Combination.ToString(), 1, 0);
        Stage_Table.Init_Csv(TableType.Stage.ToString(), 1, 0);
        Level_Table.Init_Csv(TableType.Level.ToString(), 1, 0);
        Sound_Table.Init_Csv(TableType.Sound.ToString(), 1, 0);
        Language_Table.Init_Csv(TableType.Language.ToString(), 1, 0);
#else
        //Character.Init_Binary(TableType.Character.ToString());
        //State.Init_Binary(TableType.Stat.ToString());
#endif
    }

    public void Save()
    {
        Character_Table.Save_Binary(TableType.Character.ToString());
        Stat_Table.Save_Binary(TableType.Stat.ToString());

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public async override UniTask Initialize(string _str)
    {
        await base.Initialize(_str);
        Init();
    }
   

}
