using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class SnapPoint : MonoBehaviour
{
    public SnapPointType PointType;

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (boxCollider != null)
            boxCollider.enabled = false;

        if (meshRenderer != null)
            meshRenderer.enabled = false;

    }
    private void OnValidate()
    {
        gameObject.name = "Snap Point - " + PointType.ToString(); 
    }

}
