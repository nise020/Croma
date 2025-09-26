using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class ColorPalette
{
    public static readonly Color32 emptyColor = new Color32(255, 255, 255, 51);


    public static readonly Dictionary<COLOR_TYPE, Color32> colorMap = new()
    {
        { COLOR_TYPE.scred,       new Color32(0xEE, 0x00, 0x00, 0xFF) },
        { COLOR_TYPE.scorange,    new Color32(0xFF, 0x7F, 0x00, 0xFF) },
        { COLOR_TYPE.scyellow,    new Color32(0xFF, 0xD4, 0x00, 0xFF) },
        { COLOR_TYPE.scgreen,     new Color32(0x00, 0x99, 0x00, 0xFF) },
        { COLOR_TYPE.scblue,      new Color32(0x00, 0x19, 0xF4, 0xFF) },
        { COLOR_TYPE.scpurple,    new Color32(0x46, 0x26, 0x79, 0xFF) },
        { COLOR_TYPE.scbrown,     new Color32(0x55, 0x38, 0x30, 0xFF) },
        { COLOR_TYPE.scskyBlue,   new Color32(0x79, 0xED, 0xFF, 0xFF) },
        { COLOR_TYPE.sclightGreen,new Color32(0xBF, 0xFD, 0x9F, 0xFF) },
        { COLOR_TYPE.scblack,     new Color32(0x00, 0x00, 0x00, 0xFF) },
        { COLOR_TYPE.scwhite,     new Color32(0xFF, 0xFF, 0xFF, 0xFF) },
    };

    public static Color32 GetColor(COLOR_TYPE type)
    {
        if (colorMap.TryGetValue(type, out var color))
            return color;
        
        return emptyColor;
    }
}
