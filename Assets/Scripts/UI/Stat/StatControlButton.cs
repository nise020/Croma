using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static Enums;

public enum STAT_POINT { Point1, Point10 }

public class StatControlButton : SlotBase
{
    private StatTab statTab = null;
    [SerializeField] private CHARACTER_STATUS type;
    [SerializeField] private STAT_POINT value;
    private int statValue;

    private int scbSoundID = 20012;

    public void init(StatTab _ui)
    {
        statTab = _ui;
        statValue = (value == STAT_POINT.Point1 ? 1 : 10); 
    }

    public override void OnPointerClick(PointerEventData eventData) 
    {
        base.OnPointerClick(eventData);

        if (slotType == SLOTTYPE.StatPlus && statTab != null)
        {
            statTab.StatPlus(type, statValue);
        }
    }
}
