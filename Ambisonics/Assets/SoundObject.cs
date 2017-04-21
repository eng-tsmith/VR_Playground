using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpOSC;

public class SoundObject : MonoBehaviour {
    //-------------------------------------
    // Attributes
    //-------------------------------------
    // GameObject transform data 
    public GameObject gameobject_sound;
    private float pos_x;
    private float pos_y;
    private float pos_z;
    private float scale;
    // OSC send
    private static string IP_add = "192.168.83.108"; // TODO link to values in Inspector
    private static int port = 5510;
    private UDPSender sender = new UDPSender(IP_add, port);
    // OSC receive
    private Dictionary<string, ServerLog> servers;
    private Dictionary<string, ClientLog> clients;
    // TouchOSC data
    private string touchosc_object;
    private float pad_x;
    private float pad_y;
    private float fader_height;
    private float fader_vol;
    // Pulsing
    private float MIN_SCALE;
    private float scale_pulse;

    //-------------------------------------
    // Methods
    //-------------------------------------
    // Simple mapping function
    float map2range(float val, float min_old, float max_old, float min_new, float max_new)
    {
        return min_new + (val - min_old) * (max_new - min_new) / (max_old - min_old);
    }

    // Function to send OSC messages to Linux
    void sendPos_polar(float pos_x, float pos_y, float pos_z, float scale)
    {
        // Calculate polar coordinates
        float vol = scale;
        float rad = Mathf.Sqrt(pos_x * pos_x + pos_z * pos_z + (pos_y - 1.6f) * (pos_y - 1.6f));
        float azi = Mathf.Atan2(pos_x, pos_z) * 180 / Mathf.PI;
        float ele = Mathf.Atan2(pos_y - 1.6f, Mathf.Sqrt(pos_x * pos_x + pos_z * pos_z)) * 180 / Mathf.PI;
        // Include distance for volume
        if (rad != 0f)
        {
            vol = vol / rad;
        }
        // Create messages
        var msg_azi = new SharpOSC.OscMessage("/0x00/azi", azi);
        var msg_ele = new SharpOSC.OscMessage("/0x00/ele", ele);
        var msg_vol = new SharpOSC.OscMessage("/0x00/gain", vol);
        // Send to Linux
        sender.Send(msg_azi);
        sender.Send(msg_ele);
        sender.Send(msg_vol);
    }
    
    //-------------------------------------
    // Execution
    //-------------------------------------
    // Use this for initialization
    void Start () {
        // Find Gameobject the script is attached to
        gameobject_sound = this.gameObject;
        // OSC receive init
        OSCHandler.Instance.Init();
        servers = new Dictionary<string, ServerLog>();
        clients = new Dictionary<string, ClientLog>();
        // TouchOSC init
        touchosc_object = "";
        pad_x = 0f;
        pad_y = 0f;
        fader_height = 1.5f;
        fader_vol = 0.8f;
        // Pulsing
        MIN_SCALE = 0.75f;


    }
	
	// Update is called once per frame
	void Update () {
        // Receive TouchOSC data
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
                if (touchosc_object == "/1/pad")
                {
                    pad_x = float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString());
                    pad_y = float.Parse(item.Value.packets[lastPacketIndex].Data[1].ToString());
                }
                // Height fader
                if (touchosc_object == "/1/height")
                {
                    fader_height = float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString());
                }
                // Volume fader
                if (touchosc_object == "/1/vol")
                {
                    fader_vol = float.Parse(item.Value.packets[lastPacketIndex].Data[0].ToString());
                }
            }
        }
        // Store current transform data
        pos_x = pad_x;
        pos_y = fader_height;
        pos_z = pad_y;
        scale = fader_vol;
        // Send Audio data via OSC
        sendPos_polar(pos_x, pos_y, pos_z, scale);
        // Pulse effect
        if (scale!=0)
        {
            scale_pulse = scale + Mathf.PingPong(Time.time * scale * 0.8f, 0.2f * scale);
        }
        else
        {
            scale_pulse = scale;
        }
        // Update visualisation
        gameobject_sound.transform.position = new Vector3(pos_x, pos_y, pos_z);
        gameobject_sound.transform.localScale = new Vector3(scale_pulse, scale_pulse, scale_pulse);

    }
}
