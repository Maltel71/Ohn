using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanDetector1 : MonoBehaviour
{
    public Transform reverbDistance; //Array best�ende av oceandetection-kuber
    public Transform player; //referens till spelaren
    //public float oceanCheckRadius = 20f; //Avst�nd f�r att hitta oceandetection-kuber
    //public LayerMask oceanMask; //Ett layer inneh�llande oceandetection-kuber
    //public Collider nearestOcean; //Den oceandetection-kub som �r n�rmast efter sortering
    float oceanDistance; //Avst�ndet till den n�rmaste oceandetection-kuben
    float audioDistance; //Resultareande ljudvolym
    AudioSource audioSource; //Referens till ljudk�llan
    AudioLowPassFilter loPass; //Referens till l�gpass-filter
    public AnimationCurve loPassCurve; //Animationskurva som st�ller diskantinneh�ll

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        loPass = GetComponent<AudioLowPassFilter>();
        StartCoroutine(OceanTimer());
    }

    private IEnumerator OceanTimer() //Timerfunktion
    {
        OceanCheck();
        yield return new WaitForSeconds(0.5f); //Begr�nsa antalet volymchecker
        StartCoroutine(OceanTimer()); //kalla p� dig sj�lv
    }

    void OceanCheck()
    {
        oceanDistance = Vector3.Distance(player.transform.position,
        reverbDistance.transform.position); //R�kna ut avst�ndet till aktuell kub
                                            // audioDistance = Mathf.InverseLerp(oceanCheckRadius, 0, oceanDistance); //S�tt en grundvolym
        audioSource.volume = audioDistance; //S�tt volymen till avst�ndet till oceandetection-kuben
                                            //nearestOcean = orderedByProximity[0]; //J�mf�r med den n�rmaste oceandetection-kuben
        loPass.cutoffFrequency = loPassCurve.Evaluate(audioDistance); //St�ll diskanten efter animationskurvans utseende
    }
}