using System;

public static partial class Enums
{
    public static T StringToEnum<T>(string _Value) where T : struct, Enum
    {
        if(Enum.TryParse<T>(_Value, out var result)) return result;
        return default;
    }
    public static string EnumToCustomString<T>(this T _Value) where T : Enum
    {
        if (TryGetCustomString(_Value, out string result)) return result;
        return Enum.GetName(typeof(T), _Value);
    }
    public static bool TryEnumOptionToString(object _Value, out string result)
    {
        result = string.Empty;
        Type[] enumTypes =
        {
        typeof(OPTION_DATA_VALUES_SOUND_CHANNEL),
        typeof(OPTION_DATA_VALUES_ANTI_ALIASING_MODE),
        typeof(OPTION_DATA_VALUES_LANGUAGE),
        typeof(OPTION_DATA_VALUES_ASPECTRATIO),
        typeof(OPTION_DATA_VALUES_QUALITY),
        typeof(OPTION_DATA_VALUES_DISPLAY_MODE)
        };

        foreach (Type enumType in enumTypes)
        {
            foreach (Enum item in Enum.GetValues(enumType))
            {
                result = item.ToString();
                if (result == _Value.ToString()) return true;
            }
        }

        return false;
    }
    private static bool TryGetCustomString<T>(T _EnumValue, out string result) where T : Enum
    {
        result = null;
        switch (_EnumValue)
        {
            case INFO_OPTION_DATA value: return (result = value.ToCustomString()) != null;
            case INFO_CONFIG_DATA value: return (result = value.ToCustomString()) != null;
            case INFO_STATIC_DATA value: return (result = value.ToCustomString()) != null;
            case SCENE_SCENES value: return (result = value.ToCustomString()) != null;
            default: return false;
        }
    }
}