using UnityEngine;
using static Enums;

public class ColorTarget : MonoBehaviour
{
    //public Monster_Base monster;

    [SerializeField] private COLOR_TYPE colors;

    public COLOR_TYPE GetColor()
    {
        //return (COLOR_TYPE)monster.Infos[Enums.MONSTER_DATA.Color];
        return colors;
    }
}
