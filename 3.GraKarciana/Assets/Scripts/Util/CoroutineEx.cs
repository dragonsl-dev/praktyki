using UnityEngine;
using System.Collections;

public class CoroutineEx<T> : MonoBehaviour
{
    private IEnumerator _target;
    public T result;
    public Coroutine Coroutine { get; private set; }

    public CoroutineEx(MonoBehaviour owner_, IEnumerator target_)
    {
        _target = target_;
        Coroutine = owner_.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            result = (T)_target.Current;
            yield return result;
        }
    }

}
