using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject freeLookCamera;

    private void Start()
    {
        // Find the player using tag if not assigned
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Parent the player to the parent object (which could be the boat)
            Player.transform.parent = transform.parent;

            // Parent the freelook camera to the boat as well
            if (freeLookCamera != null)
            {
                freeLookCamera.transform.parent = transform.parent;
            }

            Debug.Log("Player and FreeLook camera parented to boat");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Unparent the player
            Player.transform.parent = null;

            // Unparent the freelook camera
            if (freeLookCamera != null)
            {
                freeLookCamera.transform.parent = null;
            }

            Debug.Log("Player and FreeLook camera unparented from boat");
        }
    }
}