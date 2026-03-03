using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interacable> interacables = new List<Interacable>();

    private Interacable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.Controls.Character.Interaction.performed += context => InteractWithClosest();
    }
    private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
        interacables.Remove(closestInteractable);

        UpdateClosestInteractable();
    }
    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;

        float closestDistance = float.MaxValue;

        foreach (Interacable interacable in interacables)
        {
            float distance = Vector3.Distance(transform.position, interacable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interacable;
            }
        }
        closestInteractable?.HighlightActive(true);
    }

    public List<Interacable> GetInteracables() => interacables;
}
