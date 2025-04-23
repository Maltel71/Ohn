using UnityEngine;

public class ObjectToggleSwitch : MonoBehaviour
{
    public GameObject targetObject;
    public string playerTag = "Player";
    public bool triggerOnExit = false;

    private void OnTriggerEnter(Collider other)
    {
       // if (!triggerOnExit && IsPlayer(other))
        {
            ToggleObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
       // if (triggerOnExit && IsPlayer(other))
        {
            ToggleObject();
        }
    }

    private bool IsPlayer(Collider col)
    {
        return col.CompareTag(playerTag) && targetObject != null;
    }

    private void ToggleObject()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
}