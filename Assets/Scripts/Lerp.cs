using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public float value;
    public float maxValue;

    [Range(0f, 1f)]
    public float step;

    private void Update()
    {
        value = Mathf.Lerp(value, maxValue, step * Time.deltaTime);
    }
}
