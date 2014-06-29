using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		guiText.text = "Start";
	}
	
	// Update is called once per frame
	void Update () {

	}

	void PutStone(Vector2 key)
	{
		guiText.text = "x:" + key.x + "\ny:" + key.y;
	
	}
}
