using System.Collections;
using UnityEngine;

public class MaskingSphere : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Vector3 _targetScale = new Vector3(8, 8, 8);
    [SerializeField] private Vector3 _minScale = new Vector3(2, 2, 2);

    public bool Disabled { get; set; }

    public void Awake()
    {
        _targetScale = transform.localScale;
    }

    public void OnEnable()
    {
        transform.localScale = _minScale;
    }

    public void Update()
    {
        if (!Disabled)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, _speed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _minScale, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.localScale, _minScale) > 0.01f)
                return;

            Destroy(gameObject);
        }
    }
}