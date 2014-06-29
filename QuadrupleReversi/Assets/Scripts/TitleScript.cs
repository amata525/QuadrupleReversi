using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// スタートボタンが押された
	void StartPressed()
	{
		Application.LoadLevel("SelectStone");
	}
}
