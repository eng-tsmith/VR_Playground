using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour {
	// Grip Button
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	public bool gripButtonDown = false;
	public bool gripButtonUp = false;
	public bool gripButtonPressed = false;
	// Trigger Button
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	public bool triggerButtonDown = false;
	public bool triggerButtonUp = false;
	public bool triggerButtonPressed = false;

	// Controller
	private SteamVR_Controller.Device controller{ get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;



	// Use this for initialization
	void Start () {
		// Get Device ID
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		// Controller Connected?
		if (controller == null) {
			Debug.Log("Controller not initialized");
		}

		// Get grip button
		gripButtonDown = controller.GetPressDown(gripButton);
		gripButtonUp = controller.GetPressUp(gripButton);
		gripButtonPressed = controller.GetPress(gripButton);

		if(gripButtonDown){
			Debug.Log("Grip Button pressed");
		}
		if(gripButtonUp){
			Debug.Log("Grip Button released");
		}

		// Get trigger button
		triggerButtonDown = controller.GetPressDown(triggerButton);
		triggerButtonUp = controller.GetPressUp(triggerButton);
		triggerButtonPressed = controller.GetPress(triggerButton);

		if(triggerButtonDown){
			Debug.Log("Trigger Button pressed");
		}
		if(triggerButtonUp){
			Debug.Log("Trigger Button released");
		}
	}
}
