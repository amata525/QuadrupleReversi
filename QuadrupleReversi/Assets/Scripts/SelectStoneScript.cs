using UnityEngine;
using System.Collections;

public class SelectStoneScript : MonoBehaviour {

	public GameObject DataObj;
	OrderDatafile Data;

	UILabel Message;
	RaycastHit hit;
	GameObject[] selects = new GameObject[4];

	int count = 0;
	int player = 1;

	// Use this for initialization
	void Start () {
		DataObj = GameObject.Find("OrderData");       // OrderDatafileスクリプトに石の順番を保存する
		Data = DataObj.GetComponent<OrderDatafile>();

		Message = GameObject.Find("Message").GetComponent<UILabel>(); // NGUIのLabelをいじれるようにする。

		for (int i = 0; i < 4; i++)
		{
			selects[i] = null;
		}
	}
	

	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Fire1"))
		{
			Vector3 pos = Input.mousePosition;
			pos.z = 7.0f;

			Vector3 v = Camera.main.ScreenToWorldPoint(pos);
			
			// 判定する範囲を制限
			if (-1 < v.x && v.x < 1)
			{
				Ray ray = Camera.main.ScreenPointToRay(pos);

				if (Physics.Raycast(ray, out hit, 10))
				{
					if (hit.transform.gameObject.name.StartsWith("Next"))
					{
						selects[count] = hit.transform.gameObject;
						hit.transform.position = new Vector3(-3f, 0.5f, 3.75f - count);
						count++;
					}
				}
			}
		}
	}

	void PressedOK()
	{
		if (count > 3)
		{
			if (player == 1)
			{
				for (int i = 0; i < 4; i++)
				{
					if (selects[i].name.EndsWith("0")) Data.Player1Color[i] = 1;
					else if (selects[i].name.EndsWith("1")) Data.Player1Color[i] = 2;
					else if (selects[i].name.EndsWith("2")) Data.Player1Color[i] = 3;
					else if (selects[i].name.EndsWith("3")) Data.Player1Color[i] = 3;
				}
				player = 2;
				PressedReset();
				GameObject.Find("Next2").renderer.material = Resources.Load("Color4") as Material;
				GameObject.Find("Next3").renderer.material = Resources.Load("Color4") as Material;

				Message.text = "プレイヤー：イエロー\n石の順番を選んで下さい";

				GameObject.Find("Anchor").transform.rotation = Quaternion.Euler(0, 0, 180);
				Camera.main.transform.rotation = Quaternion.Euler(90, 0, 180);
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					if (selects[i].name.EndsWith("0")) Data.Player2Color[i] = 1;
					else if (selects[i].name.EndsWith("1")) Data.Player2Color[i] = 2;
					else if (selects[i].name.EndsWith("2")) Data.Player2Color[i] = 4;
					else if (selects[i].name.EndsWith("3")) Data.Player2Color[i] = 4;
				}

				// OrderDataのゲームオブジェクトを次のシーンに引き継ぐ
				DontDestroyOnLoad(DataObj);
				Application.LoadLevel("Reversi");
			}
		}
	}

	void PressedReset()
	{
		for (int i = 0; i < 4; i++)
		{
			if (selects[i] != null)
			{
				if (selects[i].name.EndsWith("0"))	    selects[i].transform.position = new Vector3(0, 0.5f, 4.5f);
				else if (selects[i].name.EndsWith("1")) selects[i].transform.position = new Vector3(0, 0.5f, 3);
				else if (selects[i].name.EndsWith("2")) selects[i].transform.position = new Vector3(0, 0.5f, 1.5f);
				else if (selects[i].name.EndsWith("3")) selects[i].transform.position = new Vector3(0, 0.5f, 0);

				selects[i] = null;
			}
		}
		count = 0;
	}
}
