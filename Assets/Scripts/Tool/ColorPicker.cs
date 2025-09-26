using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class ColorPicker : MonoBehaviour
{


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(GetColor());
        }
    }

    IEnumerator GetColor()
    {

        yield break;
    }

}
