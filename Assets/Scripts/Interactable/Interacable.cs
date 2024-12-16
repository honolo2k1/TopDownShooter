using UnityEngine;

public class Interacable : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    protected PlayerWeaponController weaponController;
    [SerializeField] protected MeshRenderer mesh;
    [SerializeField] protected Material defaultMaterial;

    private void Start()
    {
        if (mesh == null)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }
        defaultMaterial = mesh.sharedMaterial;
    }
    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = mesh.sharedMaterial;
    }
    public virtual void Interaction()
    {
        Debug.Log("Interacable with" + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
        {
            mesh.material = highlightMaterial;
        }
        else
        {
            mesh.material = defaultMaterial;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
        {
            weaponController = other.GetComponent<PlayerWeaponController>();
        }
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction == null) return;

        playerInteraction.GetInteracables().Add(this);
        playerInteraction.UpdateClosestInteractable();

    }
    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction == null) return;

        playerInteraction.GetInteracables().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
