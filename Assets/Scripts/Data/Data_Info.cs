using System;
using ListTable = System.Collections.Generic.Dictionary<string, System.Tuple<string, System.Collections.Generic.List<object>>>;
using IDTable = System.Collections.Generic.Dictionary<string,System.Collections.Generic.Dictionary<string, System.Tuple<string, object>>>;

public class Data_Info : Security_Base
{
    [Serializable]
    public class Option_Data
    {
        public ListTable keyData = new();
        public ListTable mouseData = new();
        public ListTable soundData = new();
        public ListTable graphicData = new();
        public ListTable videoData = new();
        public ListTable gameData = new();
    }

    [Serializable]
    public class Play_Data
    {
        public ListTable resolutionList = new();


        public ListTable colorSupportList = new();
        public IDTable monsterAttackType = new();
        public IDTable monsterState = new();
        public IDTable monsterData = new();   
        



        //New
        public IDTable itemData = new();
        public IDTable questData = new();
        public IDTable skillData = new(); // Test
        public IDTable stageData = new();
        public IDTable bookData = new();
        public IDTable buffData = new();
        public IDTable rewardData = new();
        public IDTable combinationData = new();//Color
        public IDTable characterData = new();
        public IDTable stateData = new();
    }

}
