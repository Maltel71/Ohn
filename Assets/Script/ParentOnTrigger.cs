using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    public Transform parentObject;
    public Transform playerTransform;

    // We'll use this to help debug
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

        // Check if we're interacting with any part of the player
        if (playerTransform != null && !hasTriggered)
        {
            if (other.gameObject.GetComponent<CharacterController>() != null ||
                other.transform == playerTransform)
            {
                Debug.Log("Parenting player to: " + parentObject.name);
                playerTransform.SetParent(parentObject);
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.gameObject.name);

        // Check if we're interacting with any part of the player
        if (playerTransform != null && hasTriggered)
        {
            if (other.gameObject.GetComponent<CharacterController>() != null ||
                other.transform == playerTransform)
            {
                Debug.Log("Unparenting player");
                playerTransform.SetParent(null);
                hasTriggered = false;
            }
        }
    }
}