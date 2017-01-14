using UnityEngine;
using System.Collections;

public class Resizing : MonoBehaviour {

	private float screenWidthBase = 480.0f;
	private Canvas canvas;
	// Use this for initialization
	void Start () {
		canvas = gameObject.GetComponent<Canvas>();
		canvas.scaleFactor = getScaleFacter ();
	}

	// Update is called once per frame
	void Update () {

	}

	public float getScaleFacter () {
		return Screen.width / screenWidthBase;
	}
}
