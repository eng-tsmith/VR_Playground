using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulse : MonoBehaviour {
    GameObject sphere_pul;
    private const float MIN_SCALE = 0.75f;

    // Use this for initialization
    void Start () {
        sphere_pul = GameObject.Find("Sphere_pulse");
    }
	
	// Update is called once per frame
	void Update () {
        float scale = MIN_SCALE + Mathf.PingPong(Time.time, 1f - MIN_SCALE);
        sphere_pul.transform.localScale = new Vector3(scale, scale, scale);
    }
}
