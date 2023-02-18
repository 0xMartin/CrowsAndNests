using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationScript : MonoBehaviour
{

    public float amplitude = 1.0f;
    public float frequency = 1.0f;

    private Vector3 startPosition;
    private float timeRandomOffset;

    void Start()
    {
        startPosition = transform.position;
        timeRandomOffset = (float)(Random.Range(0, 100) / 5.0);
    }

    void Update()
    {
        float y = amplitude * Mathf.Sin(frequency * Time.time + timeRandomOffset);
        transform.position = startPosition + new Vector3(0, y, 0);
    }
    
}
