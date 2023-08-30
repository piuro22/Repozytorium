using UnityEngine;
using System.Collections;
using System;

public class RenderListenerUtility : MonoBehaviour {

	public static event Action onQuit;
	public static event Action<bool> onPause;

	public static RenderListenerUtility instance;

	static RenderListenerUtility()
	{
		instance = new GameObject ("RenderUtility").AddComponent<RenderListenerUtility> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationPause (bool paused) {
		if (onPause != null) onPause(paused);
	}

	void OnApplicationQuit () {
		if (onQuit != null) onQuit();
	}

}
