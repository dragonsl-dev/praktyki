using UnityEngine;
using System.Collections;
using TMPro;
public class LimitInputFieldRange : MonoBehaviour
{

    public TMP_InputField _InputField;
    public float Max;
    public float Min;
    private void Update()
    {
        var value = System.Convert.ToInt32(_InputField.text);
        //value = Mathf.Clamp(value, Min, Max);

    }
}
