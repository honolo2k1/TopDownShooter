using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection Check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckCollider;
    [SerializeField] private Transform intersectionCheckParent;

    [ContextMenu("Set static to Env layer")]
    private void AdjustLayerForStaticObject()
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            if (childTransform.gameObject.isStatic)
            {
                childTransform.gameObject.layer = LayerMask.NameToLayer("Environment");
            }
        }
    }
    private void Start()
    {
        if (intersectionCheckCollider.Length <= 0)
        { 
            intersectionCheckCollider = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }
    }

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();

        foreach (var collider in intersectionCheckCollider)
        {
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer);

            foreach (var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>();
                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo(entrancePoint, targetSnapPoint);
        SnapTo(entrancePoint, targetSnapPoint);
    }
    public void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        transform.rotation = targetSnapPoint.transform.rotation;

        transform.Rotate(0, 180, 0);
        transform.Rotate(0, - rotationOffset, 0);
    }
    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var offset = transform.position - ownSnapPoint.transform.position;

        var newPosition = targetSnapPoint.transform.position + offset;

        transform.position = newPosition;
    }

    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);

    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();
        // Collect all snap points of the spesified type
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.PointType == pointType)
                filteredSnapPoints.Add(snapPoint);
        }
        // If there are matching snap points, choose one at random
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }
        // Return null if no matching snap points are found
        return null;
    }

    public Enemy[] Enemies() => GetComponentsInChildren<Enemy>(true);
}

