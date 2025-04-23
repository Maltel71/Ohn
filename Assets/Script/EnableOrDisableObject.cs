using UnityEngine;
using System.Collections.Generic;

public class ObjectToggleSwitch : MonoBehaviour
{
    public List<GameObject> targetObjects = new List<GameObject>();
    public string playerTag = "Player";
    public bool triggerOnExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnExit && IsPlayer(other))
        {
            ToggleObjects();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerOnExit && IsPlayer(other))
        {
            ToggleObjects();
        }
    }

    private bool IsPlayer(Collider col)
    {
        return col.CompareTag(playerTag) && targetObjects.Count > 0;
    }

    private void ToggleObjects()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}