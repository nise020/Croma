public static partial class Enums
{
    public enum OPTION_PARAMETERS_TYPE { Button, Dropdown, Key, Slider }
    public enum OPTION_DATA_KEYS_KEY
    {
        Key_Move_Left,
        Key_Move_Right,
        Key_Move_Down,
        Key_Jump,
        Key_Attack,
        Key_Interact,
        Key_Dash,
        Key_Skill,
        Key_Steal,
        Key_Slot,
        Key_Inven
    }
    public enum OPTION_DATA_KEYS_SOUND
    {
        Volume_Master,
        Volume_Bgm,
        Volume_Sfx,
        Volume_UI,
        Sound_Channel,
    }
    public enum OPTION_DATA_KEYS_MOUSE
    {
        Mouse_Sensitivity,
        Enable_MouseAcceleration,
        Mouse_Acceleration_Factor,
        Invert_Mouse_X,
        Invert_Mouse_Y,
        Mouse_Smoothing,
    }
    public enum OPTION_DATA_KEYS_GRAPHIC
    {
        Enable_Shadows,
        Shadow_Resolution,
        Lighting_Quality,
        Enable_PostProcessing,
        Enable_Bloom,
        Enable_UI_Blur,
        Antialiasing_Mode,
        Texture_Quality,
        Background_Detail_Level,
    }
    public enum OPTION_DATA_KEYS_GAME { Language, Show_FPS, }
    public enum OPTION_DATA_KEYS_VIDEO { Resolution, Aspect_Ratio, Frame_Limit, Display_Mode, Gamma, Brightness, }
    public enum OPTION_DATA_VALUES_SOUND_CHANNEL { Stereo2_0, Stereo2_1, Surround5_1, Surround7_1, Virtual7_1, Headphone_Virtual, Mono }
    public enum OPTION_DATA_VALUES_ANTI_ALIASING_MODE { Off, FXAA, SMAA, TAA }
    public enum OPTION_DATA_VALUES_LANGUAGE { En, Ko }
    public enum OPTION_DATA_VALUES_ASPECTRATIO { Ratio16_9, Ratio16_10, Ratio21_9, Ratio32_9 }
    public enum OPTION_DATA_VALUES_QUALITY { Low, Medium, High, Ultra }
    public enum OPTION_DATA_VALUES_DISPLAY_MODE { Exclusive_Fullscreen, Fullscreen_Window, Windowed, }
}