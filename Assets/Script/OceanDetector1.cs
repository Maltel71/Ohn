using UnityEngine;
using UnityEngine.Audio;

public class ReverbDistanceController : MonoBehaviour
{
    public Transform player;
    public AudioMixer audioMixer;
    public string volumeParameterName = "ReverbVolumeParameter";
    public float maxDistance = 20f;
    public float minDistance = 1f;
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, -80, 1, 0);

    private float updateTimer;
    private const float UPDATE_INTERVAL = 0.2f;

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

        float distance = Vector3.Distance(player.position, transform.position);
        float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        audioMixer.SetFloat(volumeParameterName, volumeCurve.Evaluate(normalizedDistance));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}