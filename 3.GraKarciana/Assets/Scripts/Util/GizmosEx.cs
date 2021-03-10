using UnityEngine;
using System.Collections;

public class GizmosEx : MonoBehaviour
{
    public static void DrawYAligment(Vector3 position, float scale)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, new Vector3(scale, scale, scale));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(position, new Vector3(scale, 0.0001f, scale));
    }
}
