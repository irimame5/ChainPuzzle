using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// なんかうまく揺らせないので保留
/// </summary>
public class CameraShake : MonoBehaviour {

    [SerializeField]
    public bool Shake;
    [SerializeField]
    public Vector2 ShakeDirection = Vector2.one;
    [SerializeField]
    public float ShakeImpulse = 1f;

    Vector3 firstPosition;
	void Start () {
        firstPosition = transform.position;
	}

    void Update()
    {
        if (Shake)
        {
            Vector2 normalizedShakeDirection = ShakeDirection.normalized;
            float random = Random.Range(-ShakeImpulse/2, ShakeImpulse/2);
            transform.position = firstPosition + (Vector3)normalizedShakeDirection * random*Time.deltaTime;
        }
    }
}
