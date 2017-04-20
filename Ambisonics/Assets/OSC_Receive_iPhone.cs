using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityOSC;

public class OSC_Receive_iPhone : MonoBehaviour
{
    private Dictionary<string, ServerLog> servers;
    private Dictionary<string, ClientLog> clients;
    public GameObject sphere;
    private string touchosc_object;
    private float pad_x;
    private float pad_y;
    private float height;
    //private float volume;


    // Script initialization
    void Start()
    {
        OSCHandler.Instance.Init(); //init OSC
        servers = new Dictionary<string, ServerLog>();
        clients = new Dictionary<string, ClientLog>();
        sphere = GameObject.Find("Sphere");

        touchosc_object = "";
        pad_x = 0f;
        pad_y = 0f;
        height = 1.5f;
        //volume = 0f;
    }

    // NOTE: The received messages at each server are updated here
    // Hence, this update depends on your application architecture
    // How many frames per second or Update() calls per frame?
    void Update()
    {
        OSCHandler.Instance.UpdateLogs();

        servers = OSCHandler.Instance.Servers;
        clients = OSCHandler.Instance.Clients;

        foreach (KeyValuePair<string, ServerLog> item in servers)
        {
            // If we have received at least one packet,
            // show the last received from the log in the Debug console
            if (item.Value.log.Count > 0)
            {
                int lastPacketIndex = item.Value.packets.Count - 1;
                touchosc_object = item.Value.packets[lastPacketIndex].Address;

                // Pad
                if (touchosc_object == "/1/pad"){
                    pad_x = float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString());
                    pad_y = float.Parse(item.Value.packets[lastPacketIndex].Data[1].ToString());

                    sphere.transform.position = new Vector3(pad_x, height, pad_y);
                }
                // Height fader
                if (touchosc_object == "/1/height"){
                    height = float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString());

                    sphere.transform.position = new Vector3(pad_x, height, pad_y);
                }
            }
        }
    }
}