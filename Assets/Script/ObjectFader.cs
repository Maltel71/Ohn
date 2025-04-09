using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    private GameObject _playerObj;
    private Transform _cameraTransform;

    // Todo: re-assign old materials.
    // private readonly Dictionary<Transform, Material> _originalMaterials = new();

    private readonly Dictionary<Transform, float> _transformsToFadeOut = new();
    private readonly Dictionary<Transform, float> _transformsToFadeBackIn = new();

    public float fadeDuration = 1f;
    public float targetTransparency = 0.2f;

    public void Start()
    {
        _playerObj = GameObject.FindGameObjectWithTag("Player");
        _cameraTransform = Camera.main.transform;
    }

    public void Update()
    {
        var originPosition = new Vector3(
            _playerObj.transform.position.x,
            _playerObj.transform.position.y + _playerObj.transform.localScale.y / 2,
            _playerObj.transform.position.z);

        var direction = _cameraTransform.position - originPosition;
        var ray = new Ray(originPosition, direction);
        const int layerMask = 1 << 3;
        var hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask).Select(h => h.transform).ToArray();

        MoveNewHitsToFadeOutCollection(hits);
        MoveOldHitsToFadeInCollection(hits);
        FadeOutItems();
        FadeInItems();
    }

    private void MoveOldHitsToFadeInCollection(Transform[] hits)
    {
        var itemsToRemove = new List<Transform>();
        foreach (var t in _transformsToFadeOut.Keys)
        {
            if (hits.Contains(t))
                continue;

            _transformsToFadeBackIn[t] = 0;
            itemsToRemove.Add(t);
        }

        foreach (var t in itemsToRemove)
        {
            _transformsToFadeOut.Remove(t);
        }
    }

    private void MoveNewHitsToFadeOutCollection(Transform[] hits)
    {
        foreach (var t in hits)
        {
            if (_transformsToFadeOut.ContainsKey(t.transform))
                continue;

            _transformsToFadeBackIn.Remove(t.transform);
            _transformsToFadeOut[t.transform] = 0;
        }
    }

    private void FadeInItems()
    {
        var itemsToRemove = new List<Transform>();
        foreach (var t in _transformsToFadeBackIn.Keys.ToList())
        {
            foreach (var material in t.GetComponent<Renderer>().materials)
            {
                var color = material.color;
                color.a = Mathf.Lerp(color.a, 1, _transformsToFadeBackIn[t] / fadeDuration);
                material.color = color;
            }

            _transformsToFadeBackIn[t] += Time.deltaTime;
            if (_transformsToFadeBackIn[t] >= fadeDuration)
            {
                itemsToRemove.Add(t);
            }
        }

        foreach (var t in itemsToRemove)
        {
            //var rend = t.gameObject.GetComponent<Renderer>();
            //Destroy(rend.material);
            //rend.material = _originalMaterials[t];
            // _originalMaterials.Remove(t);
            _transformsToFadeBackIn.Remove(t);
        }
    }

    private void FadeOutItems()
    {
        foreach (var t in _transformsToFadeOut.Keys.ToList())
        {
            foreach (var material in t.GetComponent<Renderer>().materials)
            {
                //_originalMaterials.TryAdd(t, material);
                var color = material.color;
                color.a = Mathf.Lerp(color.a, targetTransparency, _transformsToFadeOut[t] / fadeDuration);
                material.color = color;
            }

            _transformsToFadeOut[t] += Time.deltaTime;
        }
    }
}
