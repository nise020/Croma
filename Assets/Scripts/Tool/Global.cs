using UnityEngine;
public class Global : MonoBehaviour
{
    public static Color HexToColor(string _Hex)
    {
        _Hex = _Hex.Replace("#", "");
        if (_Hex.Length != 6)
        {
            Debug.LogError("Invalid Hex value.");
            return Color.white;
        }
        byte r = byte.Parse(_Hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(_Hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(_Hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }
}
