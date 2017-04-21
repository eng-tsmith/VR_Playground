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
	//UDP shit
	private static string IP_add = "192.168.83.109";
	private static int port = 9001;
	private SharpOSC.UDPSender sender = new SharpOSC.UDPSender (IP_add, port);

	// flying sphere
	GameObject sphere_green;

	// Simple mapping function
	float map2range(float val, float min_old, float max_old, float min_new, float max_new){
		return min_new + (val - min_old) * (max_new - min_new) / (max_old - min_old);
	}

	// Function to send Msg to Linux
	void sendPos_polar(GameObject pos_arr){

		// x and z for plane, y is height
		float pos_x = pos_arr.GetComponent<Transform> ().position.x;
		float pos_y = pos_arr.GetComponent<Transform> ().position.y;
		float pos_z = pos_arr.GetComponent<Transform> ().position.z;

		float azi;
		float ele;
		//Debug.Log(pos_x);
		//Debug.Log(pos_y);
		//Debug.Log(pos_z);

		azi = Mathf.Atan2 (pos_x, pos_z)*180/Mathf.PI;
		ele = Mathf.Atan2(pos_y - 1.6f, Mathf.Sqrt(pos_x*pos_x + pos_z*pos_z))*180/Mathf.PI;

		var msg_azi = new SharpOSC.OscMessage ("/0x00/azi", azi);
		var msg_ele = new SharpOSC.OscMessage ("/0x00/ele", ele);

		sender.Send(msg_azi);
		sender.Send(msg_ele);
	}


	// Function to send Msg to Linux
	void sendPos_cartesian(GameObject pos_arr){

		// x and z for plane, y is height; Unity uses lefthanded system!!!! x to the right y up z to screen
		float pos_x = pos_arr.GetComponent<Transform> ().position.x;
		float pos_y = pos_arr.GetComponent<Transform> ().position.y;
		float pos_z = pos_arr.GetComponent<Transform> ().position.z;

		// OpenAL uses OpenGL right handed system!!!!! transform to x to left y up z to screen
		var msg_pos = new SharpOSC.OscMessage ("/set_source_position", "sphere", -pos_x, pos_y - 1.6, pos_z);

		sender.Send(msg_pos);

	}

	// Use this for initialization
	void Start () {
		sphere_green = GameObject.Find ("Sphere_path");
		sender.Send (new SharpOSC.OscMessage ("/new_source", "sphere"));
	}
	
	// Update is called once per frame
	void Update () {
		// Found sphere?
		if (sphere_green == null) {
			Debug.Log("Sphere not initialized");
			return;
		}

		sendPos_cartesian(sphere_green);
	}
}
