using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private bool hasKey = false;
    public bool HasKey
    {
        get { return hasKey; }
        private set { hasKey = value; }
    }

    public static KeyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PickupKey()
    {
        HasKey = true;
    }
}