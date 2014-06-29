using UnityEngine;
using System.Collections;

public class TheVoid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision co)
	{
		GameObject.Destroy(co.gameObject, 5f);
	}
}
