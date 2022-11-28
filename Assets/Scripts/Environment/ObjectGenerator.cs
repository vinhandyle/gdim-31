using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that continuously generators objects.
/// </summary>
public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] [Range(0, 100)] private float genRadiusX;
    [SerializeField] [Range(0, 100)] private float genRadiusY;
    [SerializeField] [Range(0, 100)] private float minGenTime;
    [SerializeField] [Range(0, 100)] private float maxGenTime;
    private float timeLeft;

    private void Awake()
    {
        timeLeft = 0;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            float dX = Random.Range(-genRadiusX, genRadiusX);
            float dY = Random.Range(-genRadiusY, genRadiusY);
            Vector3 genPos = transform.position + new Vector3(dX, dY, 0);
            Instantiate(obj, genPos, transform.rotation, transform);
            timeLeft = Random.Range(minGenTime, maxGenTime);
        }
    }
}
