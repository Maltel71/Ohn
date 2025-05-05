using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    // Reference to the object that will become the parent (e.g., a boat)
    public Transform parentObject;

    // Optional tag to check (if you only want specific objects to be parented)
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the specified tag
        if (other.CompareTag(playerTag))
        {
            // Parent the player to the parent object
            other.transform.SetParent(parentObject);

            // Optional: Log for debugging
            Debug.Log("Player parented to " + parentObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger has the specified tag
        if (other.CompareTag(playerTag))
        {
            // Unparent the player (set parent to null)
            other.transform.SetParent(null);

            // Optional: Log for debugging
            Debug.Log("Player unparented from " + parentObject.name);
        }
    }
}