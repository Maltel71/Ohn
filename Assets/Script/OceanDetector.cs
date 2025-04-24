using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanDetector1 : MonoBehaviour
{
    public Transform reverbDistance; //Array bestående av oceandetection-kuber
    public Transform player; //referens till spelaren
    //public float oceanCheckRadius = 20f; //Avstånd för att hitta oceandetection-kuber
    //public LayerMask oceanMask; //Ett layer innehållande oceandetection-kuber
    //public Collider nearestOcean; //Den oceandetection-kub som är närmast efter sortering
    float oceanDistance; //Avståndet till den närmaste oceandetection-kuben
    float audioDistance; //Resultareande ljudvolym
    AudioSource audioSource; //Referens till ljudkällan
    AudioLowPassFilter loPass; //Referens till lågpass-filter
    public AnimationCurve loPassCurve; //Animationskurva som ställer diskantinnehåll

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        loPass = GetComponent<AudioLowPassFilter>();
        StartCoroutine(OceanTimer());
    }

    private IEnumerator OceanTimer() //Timerfunktion
    {
        OceanCheck();
        yield return new WaitForSeconds(0.5f); //Begränsa antalet volymchecker
        StartCoroutine(OceanTimer()); //kalla på dig själv
    }

    void OceanCheck()
    {
        oceanDistance = Vector3.Distance(player.transform.position,
        reverbDistance.transform.position); //Räkna ut avståndet till aktuell kub
                                            // audioDistance = Mathf.InverseLerp(oceanCheckRadius, 0, oceanDistance); //Sätt en grundvolym
        audioSource.volume = audioDistance; //Sätt volymen till avståndet till oceandetection-kuben
                                            //nearestOcean = orderedByProximity[0]; //Jämför med den närmaste oceandetection-kuben
        loPass.cutoffFrequency = loPassCurve.Evaluate(audioDistance); //Ställ diskanten efter animationskurvans utseende
    }
}