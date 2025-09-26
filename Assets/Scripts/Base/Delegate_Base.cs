using System;
using System.Collections.Generic;
using UnityEngine;

using static Enums;

//public class Delegate_Base : MonoBehaviour
//{
//    public static readonly IReadOnlyDictionary<UI_OPTION_TYPE, Action<OptionContent_Base>> keyActionMap =
//        new Dictionary<UI_OPTION_TYPE, Action<OptionContent_Base>>
//        {
//            { UI_OPTION_TYPE.Key,      KeyOptionDelegates.KeyAction },
//            { UI_OPTION_TYPE.Slider,   KeyOptionDelegates.SliderAction },
//            { UI_OPTION_TYPE.Dropdown, KeyOptionDelegates.DropdownAction }
//        };

//    public static readonly IReadOnlyDictionary<UI_OPTION_TYPE, Action<OptionContent_Base>> valueActionMap =
//        new Dictionary<UI_OPTION_TYPE, Action<OptionContent_Base>>
//        {
//            { UI_OPTION_TYPE.Key,      ValueOptionDelegates.KeyAction },
//            { UI_OPTION_TYPE.Button,   ValueOptionDelegates.ButtonAction },
//            { UI_OPTION_TYPE.Slider,   ValueOptionDelegates.SliderAction },
//            { UI_OPTION_TYPE.Dropdown, ValueOptionDelegates.DropdownAction }
//        };
//    private static class KeyOptionDelegates
//    {
//        public static readonly Action<OptionContent_Base> KeyAction = KeyMethod;
//        public static readonly Action<OptionContent_Base> SliderAction = SliderMethod;
//        public static readonly Action<OptionContent_Base> DropdownAction = DropdownMethod;

//        private static void KeyMethod(OptionContent_Base c) => c.Type_Key_Set();
//        private static void SliderMethod(OptionContent_Base c) => c.Type_Slider_Set();
//        private static void DropdownMethod(OptionContent_Base c) => c.Type_Dropdown_Set();
//    }
//    private static class ValueOptionDelegates
//    {
//        public static readonly Action<OptionContent_Base> KeyAction = KeyMethod;
//        public static readonly Action<OptionContent_Base> SliderAction = SliderMethod;
//        public static readonly Action<OptionContent_Base> ButtonAction = ButtonMethod;
//        public static readonly Action<OptionContent_Base> DropdownAction = DropdownMethod;

//        private static void KeyMethod(OptionContent_Base c) => c.Type_Key_Init();
//        private static void ButtonMethod(OptionContent_Base c) => c.Type_Button_Init();
//        private static void SliderMethod(OptionContent_Base c) => c.Type_Slider_Init();
//        private static void DropdownMethod(OptionContent_Base c) => c.Type_Dropdown_Init();
//    }
//    public static class TypeParser
//    {
//        public static readonly Dictionary<string, Func<string, object>> Parsers = new()
//    {
//        { "int", value => int.TryParse(value, out var v) ? v : 0 },
//        { "ushort", value => ushort.TryParse(value, out var v) ? v : (short)0 },
//        { "float", value => float.TryParse(value, out var v) ? v : 0f },
//        { "bool", value => bool.TryParse(value, out var v) ? v : false },
//        { "string", value => value },
//        { "Link", value => value },
//        { "undefined", value => value },
//        { "byte", value => byte.TryParse(value, out var v) ? v : (byte)0 },
//        { "KeyCode", value => Enum.TryParse<KeyCode>(value, out var v) ? v : KeyCode.None },
//        { "OPTION_DATA_VALUES_DISPLAY_MODE", value => Enum.TryParse<OPTION_DATA_VALUES_DISPLAY_MODE>(value, out var v) ? v : OPTION_DATA_VALUES_DISPLAY_MODE.Fullscreen_Window },
//        { "OPTION_DATA_VALUES_QUALITY", value => Enum.TryParse<OPTION_DATA_VALUES_QUALITY>(value, out var v) ? v : OPTION_DATA_VALUES_QUALITY.Low },
//        { "OPTION_DATA_VALUES_ASPECTRATIO", value => Enum.TryParse<OPTION_DATA_VALUES_ASPECTRATIO>(value, out var v) ? v : OPTION_DATA_VALUES_ASPECTRATIO.Ratio16_9 },
//        { "OPTION_DATA_VALUES_LANGUAGE", value => Enum.TryParse<OPTION_DATA_VALUES_LANGUAGE>(value, out var v) ? v : OPTION_DATA_VALUES_LANGUAGE.En },
//        { "OPTION_DATA_VALUES_ANTI_ALIASING_MODE", value => Enum.TryParse<OPTION_DATA_VALUES_ANTI_ALIASING_MODE>(value, out var v) ? v : OPTION_DATA_VALUES_ANTI_ALIASING_MODE.Off },
//        { "OPTION_DATA_VALUES_SOUND_CHANNEL", value => Enum.TryParse<OPTION_DATA_VALUES_SOUND_CHANNEL>(value, out var v) ? v : OPTION_DATA_VALUES_SOUND_CHANNEL.Mono },
//        { "CONFIG_COLOR_TYPE", value => Enum.TryParse<CONFIG_COLOR_TYPE>(value, out var v) ? v : CONFIG_COLOR_TYPE.Color },
//        { "MONSTER_MOVE_TYPE", value => Enum.TryParse<MONSTER_MOVE_TYPE>(value, out var v) ? v : MONSTER_MOVE_TYPE.Ground },
//        { "MONSTER_INFO_SIZE_DATA", value => Enum.TryParse<MONSTER_INFO_SIZE_DATA>(value, out var v) ? v : MONSTER_INFO_SIZE_DATA.Small },
//    };
//        public static object Parse(string typeName, string value) => Parsers.TryGetValue(typeName, out var parser) ? parser(value) : value;
//    }
//}
