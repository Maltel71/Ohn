using UnityEngine;
using UnityEngine.Splines;

public class ForceSplinePlay : MonoBehaviour
{
    private SplineAnimate splineAnimator;

    void Start()
    {
        splineAnimator = GetComponent<SplineAnimate>();
        if (splineAnimator != null)
        {
            splineAnimator.Play();
        }
    }
}