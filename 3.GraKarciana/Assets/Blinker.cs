using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Blinker : MonoBehaviour
{
    public Image _Image;
    public Color Color1;
    public Color Color2;
    public float Interval;

    void Start()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            _Image.color = Color1;
            yield return new WaitForSeconds(Interval);

            _Image.color = Color2;
            yield return new WaitForSeconds(Interval);
        }
    }
}
