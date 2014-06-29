using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// カメラの解像度を設定する
	void Awake()
	{
		Camera cam = gameObject.GetComponent<Camera>();

		float baseAspect ;

		// iPhone
		//baseAspect = 640f / 960f;

		// 16:9
		baseAspect = 720f / 1280f;


		float nowAspect = (float)Screen.height / (float)Screen.width;
		float changeAspect;

		if (baseAspect > nowAspect)
		{
			changeAspect = nowAspect / baseAspect;
			cam.rect = new Rect((1 - changeAspect) * 0.5f, 0, changeAspect, 1);
		}
		else
		{
			changeAspect = baseAspect / nowAspect;
			cam.rect = new Rect(0, (1 - changeAspect) * 0.5f, 1, changeAspect);
		}

		Destroy(this);
	}
}
