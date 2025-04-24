using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class ReverbDistanceController : MonoBehaviour
{
    public Transform player;
    public AudioMixer audioMixer;
    public string volumeParameterName = "ReverbVolumeParameter";
    public float maxDistance = 20f;
    public float minDistance = 3f;
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, -80, 1, 0);

    // Static collection to manage all reverb controllers in the scene
    private static List<ReverbDistanceController> allControllers = new List<ReverbDistanceController>();

    private float updateTimer;
    private const float UPDATE_INTERVAL = 0.2f;

    void OnEnable()
    {
        allControllers.Add(this);
    }

    void OnDisable()
    {
        allControllers.Remove(this);
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer < UPDATE_INTERVAL) return;
        updateTimer = 0;

        if (player == null || audioMixer == null) return;

        // Only the closest controller should affect the reverb
        if (IsClosestController())
        {
            float distance = Vector3.Distance(player.position, transform.position);
            float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            audioMixer.SetFloat(volumeParameterName, volumeCurve.Evaluate(normalizedDistance));
        }
    }

    // Check if this controller is the closest to the player
    private bool IsClosestController()
    {
        if (allControllers.Count <= 1) return true;

        float myDistance = Vector3.Distance(player.position, transform.position);

        foreach (var controller in allControllers)
        {
            if (controller == this) continue;

            float otherDistance = Vector3.Distance(player.position, controller.transform.position);
            if (otherDistance < myDistance)
                return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}