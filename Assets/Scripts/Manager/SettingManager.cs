using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using static Data_Info;
using static Enums;
using static Enums.INFO_OPTION_DATA;
using KEY = Enums.OPTION_DATA_KEYS_KEY;
using SOUND = Enums.OPTION_DATA_KEYS_SOUND;
using MOUSE = Enums.OPTION_DATA_KEYS_MOUSE;
using VIDEO = Enums.OPTION_DATA_KEYS_VIDEO;
using GRPAHIC = Enums.OPTION_DATA_KEYS_GRAPHIC;
using GAME = Enums.OPTION_DATA_KEYS_GAME;
using ListTable = System.Collections.Generic.Dictionary<string, System.Tuple<string, System.Collections.Generic.List<object>>>;
using IDTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Tuple<string, object>>>;
//public class SettingManager : Manager_Base
//{
//    public Dictionary<INFO_OPTION_DATA, Dictionary<Enum, object>> OptionData { get; private set; } = new();
//    public Dictionary<Enum, List<string>> ResolutionList { get; private set; } = new();
//    public Dictionary<string, List<Color>> ColorSupportList { get; private set; } = new();
//    public Dictionary<string, Dictionary<CHARACTER_DATA, object>> CharacterData { get; private set; } = new();
//    public Dictionary<string, Dictionary<CHARACTER_STATUS, object>> CharacterState { get; private set; } = new();
//    public Dictionary<string, Dictionary<MONSTER_ATTACK_TYPE, object>> MonsterAttackType { get; private set; } = new();

//    // 해보는중..
//    public Dictionary<string, List<Tuple<QUEST_DATA, object>>> QuestData { get; private set; } = new();


//    private readonly Func<string, string, int, object> Get = (field, key, idx) => Shared.Instance.DataManager.Get<Option_Data>(field, key, idx);

//    Data_Manager shared;

//    public async override UniTask Initialize(string _Str)
//    {
//        await base.Initialize(_Str);
//        shared = Shared.Instance.DataManager;

//        //await InitKeyDatas(1);
//        //await InitSoundDatas(1);
//        //await InitMouseDatas(1);
//        //await InitGraphicDatas(1);
//        //await InitGameDatas(1);
//        //await InitVideoDatas(1);

//        //await InitResolutionList();
//        //await InitColorSupportList();

//        //await InitMonsterAttackType();
//        await InitCheracter();
//        await InitState();

//        await InitQuestData();

//        await SaveOption();
//        Initialized(_Str);
//    }
//    public UniTask SaveOption()
//    {
//        object v = OptionData;
//        Shared.Instance.DataManager.Save_Json(ref v);
//        return UniTask.CompletedTask;
//    }
//    public UniTask GetMin<T>(ref T _Val, INFO_OPTION_DATA _Data, Enum _Key)
//    {
//        _Val = (T)(shared.Get<Option_Data>(Enums.EnumToCustomString(_Data), Enums.EnumToCustomString(_Key), 2) ?? _Val);
//        return UniTask.CompletedTask;
//    }
//    public UniTask GetMax<T>(ref T _Val, INFO_OPTION_DATA _Data, Enum _Key)
//    {
//        _Val = (T)(shared.Get<Option_Data>(Enums.EnumToCustomString(_Data), Enums.EnumToCustomString(_Key), 3) ?? _Val);
//        return UniTask.CompletedTask;
//    }
//    public UniTask InitKeyDatas(int _Idx)
//    {
//        OptionData.Add(KeyData, new());
//        var field = Enums.EnumToCustomString(KeyData);

//        OptionData[KeyData].Add(KEY.Key_Move_Left, Get(field, Enums.EnumToCustomString(KEY.Key_Move_Left), _Idx) ?? KeyCode.A);
//        OptionData[KeyData].Add(KEY.Key_Move_Right, Get(field, Enums.EnumToCustomString(KEY.Key_Move_Right), _Idx) ?? KeyCode.D);
//        OptionData[KeyData].Add(KEY.Key_Move_Down, Get(field, Enums.EnumToCustomString(KEY.Key_Move_Down), _Idx) ?? KeyCode.S);
//        OptionData[KeyData].Add(KEY.Key_Jump, Get(field, Enums.EnumToCustomString(KEY.Key_Jump), _Idx) ?? KeyCode.W);
//        OptionData[KeyData].Add(KEY.Key_Interact, Get(field, Enums.EnumToCustomString(KEY.Key_Interact), _Idx) ?? KeyCode.E);
//        OptionData[KeyData].Add(KEY.Key_Skill, Get(field, Enums.EnumToCustomString(KEY.Key_Skill), _Idx) ?? KeyCode.F);
//        OptionData[KeyData].Add(KEY.Key_Slot, Get(field, Enums.EnumToCustomString(KEY.Key_Slot), _Idx) ?? KeyCode.R);
//        OptionData[KeyData].Add(KEY.Key_Inven, Get(field, Enums.EnumToCustomString(KEY.Key_Inven), _Idx) ?? KeyCode.I);
//        OptionData[KeyData].Add(KEY.Key_Dash, Get(field, Enums.EnumToCustomString(KEY.Key_Dash), _Idx) ?? KeyCode.Space);
//        OptionData[KeyData].Add(KEY.Key_Attack, Get(field, Enums.EnumToCustomString(KEY.Key_Attack), _Idx) ?? KeyCode.Mouse0);
//        OptionData[KeyData].Add(KEY.Key_Steal, Get(field, Enums.EnumToCustomString(KEY.Key_Steal), _Idx) ?? KeyCode.LeftShift);
//        return UniTask.CompletedTask;
//    }

//    public UniTask InitSoundDatas(int _Idx)
//    {
//        OptionData.Add(SoundData, new());
//        var field = Enums.EnumToCustomString(SoundData);

//        OptionData[SoundData].Add(SOUND.Volume_Master, Get(field, Enums.EnumToCustomString(SOUND.Volume_Master), _Idx) ?? (byte)100);
//        OptionData[SoundData].Add(SOUND.Volume_Bgm, Get(field, Enums.EnumToCustomString(SOUND.Volume_Bgm), _Idx) ?? (byte)100);
//        OptionData[SoundData].Add(SOUND.Volume_Sfx, Get(field, Enums.EnumToCustomString(SOUND.Volume_Sfx), _Idx) ?? (byte)100);
//        OptionData[SoundData].Add(SOUND.Volume_UI, Get(field, Enums.EnumToCustomString(SOUND.Volume_UI), _Idx) ?? (byte)100);
//        OptionData[SoundData].Add(SOUND.Sound_Channel, Get(field, Enums.EnumToCustomString(SOUND.Sound_Channel), _Idx) ?? OPTION_DATA_VALUES_SOUND_CHANNEL.Mono);
//        return UniTask.CompletedTask;
//    }

//    public UniTask InitMouseDatas(int _Idx)
//    {
//        OptionData.Add(MouseData, new());
//        var field = Enums.EnumToCustomString(MouseData);

//        OptionData[MouseData].Add(MOUSE.Mouse_Sensitivity, Get(field, Enums.EnumToCustomString(MOUSE.Mouse_Sensitivity), _Idx) ?? (byte)1);
//        OptionData[MouseData].Add(MOUSE.Mouse_Acceleration_Factor, Get(field, Enums.EnumToCustomString(MOUSE.Mouse_Acceleration_Factor), _Idx) ?? (byte)1);
//        OptionData[MouseData].Add(MOUSE.Enable_MouseAcceleration, Get(field, Enums.EnumToCustomString(MOUSE.Enable_MouseAcceleration), _Idx) ?? true);
//        OptionData[MouseData].Add(MOUSE.Invert_Mouse_X, Get(field, Enums.EnumToCustomString(MOUSE.Invert_Mouse_X), _Idx) ?? false);
//        OptionData[MouseData].Add(MOUSE.Invert_Mouse_Y, Get(field, Enums.EnumToCustomString(MOUSE.Invert_Mouse_Y), _Idx) ?? false);
//        OptionData[MouseData].Add(MOUSE.Mouse_Smoothing, Get(field, Enums.EnumToCustomString(MOUSE.Mouse_Smoothing), _Idx) ?? false);
//        return UniTask.CompletedTask;
//    }

//    public UniTask InitGraphicDatas(int _Idx)
//    {
//        OptionData.Add(GraphicData, new());
//        var field = Enums.EnumToCustomString(GraphicData);

//        OptionData[GraphicData].Add(GRPAHIC.Enable_Shadows, Get(field, Enums.EnumToCustomString(GRPAHIC.Enable_Shadows), _Idx) ?? OPTION_DATA_VALUES_QUALITY.Low);
//        OptionData[GraphicData].Add(GRPAHIC.Shadow_Resolution, Get(field, Enums.EnumToCustomString(GRPAHIC.Shadow_Resolution), _Idx) ?? OPTION_DATA_VALUES_QUALITY.Low);
//        OptionData[GraphicData].Add(GRPAHIC.Lighting_Quality, Get(field, Enums.EnumToCustomString(GRPAHIC.Lighting_Quality), _Idx) ?? OPTION_DATA_VALUES_QUALITY.Low);
//        OptionData[GraphicData].Add(GRPAHIC.Background_Detail_Level, Get(field, Enums.EnumToCustomString(GRPAHIC.Background_Detail_Level), _Idx) ?? OPTION_DATA_VALUES_QUALITY.Low);
//        OptionData[GraphicData].Add(GRPAHIC.Texture_Quality, Get(field, Enums.EnumToCustomString(GRPAHIC.Texture_Quality), _Idx) ?? OPTION_DATA_VALUES_QUALITY.High);
//        OptionData[GraphicData].Add(GRPAHIC.Antialiasing_Mode, Get(field, Enums.EnumToCustomString(GRPAHIC.Antialiasing_Mode), _Idx) ?? OPTION_DATA_VALUES_ANTI_ALIASING_MODE.Off);
//        OptionData[GraphicData].Add(GRPAHIC.Enable_PostProcessing, Get(field, Enums.EnumToCustomString(GRPAHIC.Enable_PostProcessing), _Idx) ?? true);
//        OptionData[GraphicData].Add(GRPAHIC.Enable_Bloom, Get(field, Enums.EnumToCustomString(GRPAHIC.Enable_Bloom), _Idx) ?? true);
//        OptionData[GraphicData].Add(GRPAHIC.Enable_UI_Blur, Get(field, Enums.EnumToCustomString(GRPAHIC.Enable_UI_Blur), _Idx) ?? true);
//        return UniTask.CompletedTask;
//    }

//    public UniTask InitGameDatas(int _Idx)
//    {
//        OptionData.Add(GameData, new());
//        var field = Enums.EnumToCustomString(GameData);

//        OptionData[GameData].Add(GAME.Language, Get(field, Enums.EnumToCustomString(GAME.Language), _Idx) ?? OPTION_DATA_VALUES_LANGUAGE.En);
//        OptionData[GameData].Add(GAME.Show_FPS, Get(field, Enums.EnumToCustomString(GAME.Show_FPS), _Idx) ?? false);
//        return UniTask.CompletedTask;
//    }

//    public UniTask InitVideoDatas(int _Idx)
//    {
//        OptionData.Add(VideoData, new());
//        var field = Enums.EnumToCustomString(VideoData);

//        OptionData[VideoData].Add(VIDEO.Resolution, Get(field, Enums.EnumToCustomString(VIDEO.Resolution), _Idx) ?? "1920x1080");
//        OptionData[VideoData].Add(VIDEO.Aspect_Ratio, Get(field, Enums.EnumToCustomString(VIDEO.Aspect_Ratio), _Idx) ?? OPTION_DATA_VALUES_ASPECTRATIO.Ratio16_9);
//        OptionData[VideoData].Add(VIDEO.Display_Mode, Get(field, Enums.EnumToCustomString(VIDEO.Display_Mode), _Idx) ?? OPTION_DATA_VALUES_DISPLAY_MODE.Fullscreen_Window);
//        OptionData[VideoData].Add(VIDEO.Frame_Limit, Get(field, Enums.EnumToCustomString(VIDEO.Frame_Limit), _Idx) ?? (ushort)60);
//        OptionData[VideoData].Add(VIDEO.Gamma, Get(field, Enums.EnumToCustomString(VIDEO.Gamma), _Idx) ?? (byte)50);
//        OptionData[VideoData].Add(VIDEO.Brightness, Get(field, Enums.EnumToCustomString(VIDEO.Brightness), _Idx) ?? (byte)50);
//        return UniTask.CompletedTask;
//    }
//    public UniTask InitResolutionList()
//    {
//        foreach (OPTION_DATA_VALUES_ASPECTRATIO ratio in Enum.GetValues(typeof(OPTION_DATA_VALUES_ASPECTRATIO)))
//        {
//            var field = Enums.EnumToCustomString(INFO_CONFIG_DATA.ResolutionList);
//            var strList = shared.Get<Play_Data>(field, Enums.EnumToCustomString(ratio));
//            if (strList is List<object> objList) ResolutionList.Add(ratio, objList.ConvertAll(x => x?.ToString() ?? string.Empty));
//        }
//        return UniTask.CompletedTask;
//    }
//    public UniTask InitColorSupportList()
//    {
//        var field = Enums.EnumToCustomString(INFO_CONFIG_DATA.ColorSupportList);
//        var list = shared.Get<Play_Data>(field) as ListTable;
//        foreach (var item in list)
//        {
//            List<Color> colors = new();
//            ColorSupportList.Add(item.Key, colors);
//            foreach (var color in item.Value.Item2) colors.Add(Global.HexToColor((string)color));
//        }
//        return UniTask.CompletedTask;
//    }

//    // 테스트 해보는 중..
//    public UniTask InitQuestData()
//    {
//        var field = Enums.EnumToCustomString(INFO_STATIC_DATA.Quest);
//        var list = shared.Get<Play_Data>(field) as IDTable;

//        foreach (var item in list.Keys)
//        {
//            QuestData.Add(item, new());
//            foreach (var value in list[item])
//            {
//                var key = StringToEnum<QUEST_DATA>(value.Key);
//                QuestData[item].Add(Tuple.Create(key, value.Value.Item2));
//            }
//        }
//        return UniTask.CompletedTask;
//    }


//    public UniTask InitMonsterAttackType()
//    {
//        var field = Enums.EnumToCustomString(INFO_STATIC_DATA.MonsterAttackType);
//        var list = shared.Get<Play_Data>(field) as IDTable;
//        foreach (var item in list.Keys)
//        {
//            MonsterAttackType.Add(item, new());
//            foreach (var value in list[item])
//            {
//                var key = StringToEnum<MONSTER_ATTACK_TYPE>(value.Key);
//                MonsterAttackType[item][key] = value.Value.Item2;
//            }
//        }
//        return UniTask.CompletedTask;
//    }
//    public UniTask InitState()
//    {
//        var field = Enums.EnumToCustomString(INFO_STATIC_DATA.MonsterState);
//        var list = shared.Get<Play_Data>(field) as IDTable;
//        foreach (var item in list.Keys)
//        {
//            CharacterState.Add(item, new());
//            foreach (var value in list[item])
//            {
//                var key = StringToEnum<MONSTER_STATE>(value.Key);
//                CharacterState[item][key] = value.Value.Item2;
//            }
//        }
//        return UniTask.CompletedTask;
//    }
//    //public UniTask InitCheracter()
//    //{
//    //    var field = Enums.EnumToCustomString(INFO_STATIC_DATA.Character);
//    //    var list = shared.Get<Play_Data>(field) as IDTable;
//    //    foreach (var item in list.Keys)
//    //    {
//    //        CharacterData.Add(item, new());
//    //        foreach (var value in list[item])
//    //        {
//    //            var key = StringToEnum<CHARACTER_DATA>(value.Key);
//    //            string valueStr = value.Value.Item2.ToString();
//    //            switch (key)
//    //            {
//    //                case CHARACTER_DATA.Id:
//    //                    {
//    //                        var copiedList = new Dictionary<MONSTER_ATTACK_TYPE, int>(CharacterData[valueStr]);
//    //                        CharacterData[item][key] = copiedList;
//    //                        break;
//    //                    }
//    //                case MONSTER_DATA.State:
//    //                    {
//    //                        var copiedList = new Dictionary<MONSTER_STATE, int>(CharacterState[valueStr]);
//    //                        CharacterData[item][key] = copiedList;
//    //                        break;
//    //                    }
//    //                case MONSTER_DATA.Color:
//    //                    object color = Global.HexToColor(value.Value.Item2.ToString());
//    //                    CharacterData[item][key] = color;
//    //                    break;
//    //                default:
//    //                    CharacterData[item][key] = value.Value.Item2;
//    //                    break;
//    //            }
//    //        }
//    //    }
//    //    MonsterAttackType.Clear();
//    //    CharacterState.Clear();
//    //    return UniTask.CompletedTask;
//    //}

//    public UniTask InitCheracter()
//    {
//        // CSV에서 파싱된 IDTable을 불러옴
//        var field = Enums.EnumToCustomString(INFO_STATIC_DATA.Character);
//        var list = shared.Get<Play_Data>(field) as IDTable;

//        foreach (var item in list.Keys) // item: 캐릭터 ID
//        {
//            // 해당 캐릭터의 데이터를 저장할 딕셔너리 초기화
//            CharacterData[item] = new Dictionary<CHARACTER_DATA, object>();

//            foreach (var value in list[item])
//            {
//                var key = StringToEnum<CHARACTER_DATA>(value.Key);
//                string valueStr = value.Value.Item2.ToString();

//                switch (key)
//                {
//                    case CHARACTER_DATA.Id:
//                        {
//                            if (CharacterData.TryGetValue(valueStr, out var atkList))
//                            {
//                                // ID가 MonsterAttackType의 키일 경우 => 복사해서 저장
//                                foreach (var atk in atkList)
//                                {
//                                    // 원하는 방식으로 키 추가 가능
//                                    CharacterData[item][key] = atk.Value; // 하나만 저장? 구조 점검 필요
//                                }
//                            }
//                            break;
//                        }

//                    case CHARACTER_DATA.StateId:
//                        {
//                            if (CharacterState.TryGetValue(valueStr, out var stateList))
//                            {
//                                foreach (var state in stateList)
//                                {
//                                    CharacterData[item][key] = state.Value;
//                                }
//                            }
//                            break;
//                        }

//                    default:
//                        {
//                            // 일반 숫자 필드는 int로 변환해서 저장
//                            if (int.TryParse(value.Value.Item2.ToString(), out int intValue))
//                            {
//                                CharacterData[item][key] = intValue;
//                            }
//                            else
//                            {
//                                Debug.LogWarning($"값 {value.Value.Item2} 를 int로 변환할 수 없습니다.");
//                            }
//                            break;
//                        }
//                }
//            }
//        }

//        MonsterAttackType.Clear();
//        CharacterState.Clear();
//        return UniTask.CompletedTask;
//    }
//}
