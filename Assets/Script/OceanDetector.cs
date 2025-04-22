using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OceanDetector : MonoBehaviour
{
    Collider[] oceanColliders; //Array best�ende av oceandetection-kuber
    public Transform player; //referens till spelaren
    public float oceanCheckRadius = 20f; //Avst�nd f�r att hitta oceandetection-kuber
    public LayerMask oceanMask; //Ett layer inneh�llande oceandetection-kuber
    public Collider nearestOcean; //Den oceandetection-kub som �r n�rmast efter sortering
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
        loPass.enabled = true; //Tvinga filtret att vara p�slaget
        yield return new WaitForSeconds(0.5f); //Begr�nsa antalet volymchecker
        StartCoroutine(OceanTimer()); //kalla p� dig sj�lv
    }

   void OceanCheck()
    {
        //Kolla efter oceandetection-kuber inom oceandistance-avst�ndet
        oceanColliders = Physics.OverlapSphere(player.transform.position, oceanCheckRadius, oceanMask);
        //Sortera oceancolliders-arrayen s� att den n�rmaste oceandetection-kuben ligger �verst 
        var orderedByProximity = oceanColliders.OrderBy(c => (player.transform.position - c.transform.position
        ).sqrMagnitude).ToArray();

        if (oceanColliders.Length != 0) //Kolla s� att arayen inte �r tom
        {
            oceanDistance = Vector3.Distance(player.transform.position,
            orderedByProximity[0].transform.position); //R�kna ut avst�ndet till aktuell kub
            audioDistance = Mathf.InverseLerp(oceanCheckRadius, 0, oceanDistance); //S�tt en grundvolym
            audioSource.volume = audioDistance; //S�tt volymen till avst�ndet till oceandetection-kuben
            nearestOcean = orderedByProximity[0]; //J�mf�r med den n�rmaste oceandetection-kuben
            loPass.cutoffFrequency = loPassCurve.Evaluate(audioDistance); //St�ll diskanten efter animationskurvans utseende
        }
        else
        {
            audioSource.volume = 0f; //Ingen oceandetection-kub inom r�ckh�ll, st�ng av volymen
        }   
    }
}
