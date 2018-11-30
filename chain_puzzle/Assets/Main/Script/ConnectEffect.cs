using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectEffect : MonoBehaviour {

	void Start () {

    }

    public void Initialize(Vector3 start,Vector3 end)
    {
        const float ZShift = 0.8f;

        var particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystem.transform.up = end - start;
        start.z += ZShift;
        end.z += ZShift;
        transform.position = start;
        var distance = Vector3.Distance(start, end);
        float speed = particleSystem.main.startSpeed.constantMax;
        var main = particleSystem.main;
        main.startLifetime = distance / speed;
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        Initialize(Vector2.zero, Vector2.zero);
    }
	
	void Update () {

	}
}
