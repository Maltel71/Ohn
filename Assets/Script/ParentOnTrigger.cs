using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    public GameObject Player;

    private void Start()
    {
        // Find the player using tag
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Parent the player to the parent object (which could be the boat)
            Player.transform.parent = transform.parent;
            Debug.Log("Player parented to boat");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Unparent the player
            Player.transform.parent = null;
            Debug.Log("Player unparented from boat");
        }
    }
}