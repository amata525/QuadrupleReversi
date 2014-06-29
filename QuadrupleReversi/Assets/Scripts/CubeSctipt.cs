using UnityEngine;
using System.Collections;

public class CubeSctipt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	/// <summary>右ボタン押してるか</summary>
	private bool isRightButtonPressed;

	/// <summary>左ボタン押してるか</summary>
	private bool isLeftButtonPressed;

	void Update()
	{
		if (this.isRightButtonPressed)
		{
			//右回転
			this.transform.Rotate(0, 100 * Time.deltaTime, 0);
		}
		else if (this.isLeftButtonPressed)
		{
			//左回転
			this.transform.Rotate(0, -100 * Time.deltaTime, 0);
		}
	}

	/// <summary>右ボタン押された</summary>
	void RightButtonPressed() { this.isRightButtonPressed = true; }

	/// <summary>左ボタン離れた</summary>
	void RightButtonReleased() { this.isRightButtonPressed = false; }

	/// <summary>左ボタン押された</summary>
	void LeftButtonPressed() { this.isLeftButtonPressed = true; }

	/// <summary>左ボタン離れた</summary>
	void LeftButtonReleased() { this.isLeftButtonPressed = false; }
}
