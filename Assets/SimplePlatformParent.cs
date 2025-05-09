using UnityEngine;

public class SimplePlatformParent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<MinimalThirdPersonController>();
            if (playerController != null)
            {
                playerController.SetPlatform(transform.parent);
                Debug.Log("Player on platform: " + transform.parent.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<MinimalThirdPersonController>();
            if (playerController != null)
            {
                playerController.ClearPlatform();
                Debug.Log("Player left platform");
            }
        }
    }
}