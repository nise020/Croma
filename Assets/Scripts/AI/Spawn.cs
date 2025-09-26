using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Spawn : MonoBehaviour
{
    public CHARACTER_ID SpownMonsterType = CHARACTER_ID.Default;
    private List<Vector3> MovePointList { get; set; } = new();
    private Vector3 MovePos1 { get; set; } = Vector3.zero;
    private Vector3 MovePos2 { get; set; } = Vector3.zero;

    public List<Vector3> LoadMovePos()
    {
        var movePointPos = GetComponentsInChildren<Transform>();
        List<Vector3> movePointList = new();

        foreach (Transform t in movePointPos)
        {
            if (t != transform) movePointList.Add(t.position);
        }
        return movePointList;
    }
}
