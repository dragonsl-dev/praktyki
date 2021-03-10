using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public float offsetY;
    public void Highlight(Player target)
    {
        var newPos = target.transform.position;
        newPos.y += offsetY;
        transform.position = newPos;
    }
}
