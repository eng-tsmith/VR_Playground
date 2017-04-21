using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SharpOSC;


using UnityEngine;

public class sendpos : MonoBehaviour {
	// UDP shit
	private static string IP_add = "192.168.83.108";
	private static int port = 5510;
	private SharpOSC.UDPSender sender = new SharpOSC.UDPSender (IP_add, port);

	// flying sphere
	GameObject sphere;

	// Simple mapping function
	float map2range(float val, float min_old, float max_old, float min_new, float max_new){
		return min_new + (val - min_old) * (max_new - min_new) / (max_old - min_old);
	}

	// Function to send Msg to Linux
	void sendPos_polar(GameObject sound_sphere){

		// x and z for plane, y is height
		float pos_x = sound_sphere.GetComponent<Transform> ().position.x;
		float pos_y = sound_sphere.GetComponent<Transform> ().position.y;
		float pos_z = sound_sphere.GetComponent<Transform> ().position.z;

        float vol = sound_sphere.GetComponent<Transform>().localScale.x;

        float rad = Mathf.Sqrt(pos_x * pos_x + pos_z * pos_z + (pos_y - 1.6f) * (pos_y - 1.6f));
        float azi = Mathf.Atan2(pos_x, pos_z) * 180 / Mathf.PI;
        float ele = Mathf.Atan2(pos_y - 1.6f, Mathf.Sqrt(pos_x * pos_x + pos_z * pos_z)) * 180 / Mathf.PI;

        if (rad != 0f) {
            vol = vol / rad;
        }
        

        //Debug.Log(pos_x);
        //Debug.Log(pos_y);
        //Debug.Log(pos_z);
        //Debug.Log(vol);

        var msg_azi = new SharpOSC.OscMessage ("/0x00/azi", azi);
        var msg_ele = new SharpOSC.OscMessage("/0x00/ele", ele);
        var msg_vol = new SharpOSC.OscMessage("/0x00/gain", vol);

        sender.Send(msg_azi);
        sender.Send(msg_ele);
        sender.Send(msg_vol);
    }


	//// Function to send Msg to Linux
	//void sendPos_cartesian(GameObject pos_arr){

	//	// x and z for plane, y is height; Unity uses lefthanded system!!!! x to the right y up z to screen
	//	float pos_x = pos_arr.GetComponent<Transform> ().position.x;
	//	float pos_y = pos_arr.GetComponent<Transform> ().position.y;
	//	float pos_z = pos_arr.GetComponent<Transform> ().position.z;

	//	// OpenAL uses OpenGL right handed system!!!!! transform to x to left y up z to screen
	//	var msg_pos = new SharpOSC.OscMessage ("/set_source_position", "sphere", -pos_x, pos_y - 1.6, pos_z);

	//	sender.Send(msg_pos);
    //}

	// Use this for initialization
	void Start () {
        sphere = GameObject.Find ("Sphere");
	}
	
	// Update is called once per frame
	void Update () {
		// Found sphere?
		if (sphere == null) {
			Debug.Log("Sphere not initialized");
			return;
		}

        sendPos_polar(sphere);
	}
}
