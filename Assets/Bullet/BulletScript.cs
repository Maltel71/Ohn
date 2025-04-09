using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 targetPos;
    public float moveSpeed = 100f;

    public void Setup(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    void Update()
    {
        float distanceBefore = Vector3.Distance(transform.position, targetPos);

        Vector3 moveDir = (targetPos - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, targetPos);

        if (distanceBefore < distanceAfter)
        {
            Destroy(gameObject);
        }
    }
}
