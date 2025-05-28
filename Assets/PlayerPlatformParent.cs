using UnityEngine;

public class PlayerPlatformParent : MonoBehaviour
{
    public Transform player;
    public Transform parentObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")

        {
            if (player = null) player = other.transform;
            player.parent = parentObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        player.parent = null;

    }

}