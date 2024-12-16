using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject_CarDeliver : MonoBehaviour
{
    public static event Action OnCarDelivery;

    public void InvokeOnCarDelivery() => OnCarDelivery?.Invoke();
}
