using UnityEngine;

public class PlayerParentTrigger : MonoBehaviour
{
    [SerializeField] private Transform objectToParent;
    [SerializeField] private Transform parentTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objectToParent != null)
        {
            objectToParent.SetParent(parentTarget);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && objectToParent != null)
        {
            objectToParent.SetParent(null);
        }
    }
}