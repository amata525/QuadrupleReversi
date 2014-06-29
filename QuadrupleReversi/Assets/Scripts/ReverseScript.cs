using UnityEngine;
using System.Collections;

public class ReverseScript : MonoBehaviour {

	int setColor;
	int waitTime;
	int torqueX, torqueZ;
	int originX, originZ;
	int count;

	float[,] powerX = new float[3, 3] { { -7.07f, 0, 7.07f }, { -10f, 0, 10f }, { -7.07f, 0, 7.07f } };
	float[,] powerZ = new float[3, 3] { { 7.07f, 10f, 7.07f }, { 0, 0, 0 }, { -7.07f, -10f, -7.07f } };

	// Use this for initialization
	void Start () {
		waitTime = -1;
		count = 0;
		setColor =1;
	}
	
	// Update is called once per frame
	void Update () {
		if (waitTime > 0)
		{
			waitTime--;
			if (waitTime == 0)
			{
				count = 55;;
				rigidbody.AddForce(0, 180f, 0);
				rigidbody.AddTorque(powerX[torqueX, torqueZ], 0, powerZ[torqueX, torqueZ]);
			}
		}
		else if(waitTime == 0)
		{
			count--;
			if(count == 35)
			{
				// 用意していない色にならないように
				if(0 < setColor && setColor <= 5)
					renderer.material = (Material)Resources.Load("Color" + setColor);
			}
			else if(count == 0)
			{
				// 完全に停止させる。 色が０で指定されている場合は例外
				if (setColor != 0)
				{
					transform.position = new Vector3(originX, 0.5f, originZ);
					rigidbody.angularVelocity = Vector3.zero;
					rigidbody.velocity = Vector3.zero;
					transform.rotation = Quaternion.Euler(0, 0, 0);
				}
				waitTime = -1;
			}
		}
	}

	public void setReverse(int c, int w, int ox, int oz, int tx, int tz)
	{
		setColor = c;
		waitTime = w;
		torqueX = tx+1;
		torqueZ = tz+1;
		originX = ox;
		originZ = oz;
	}
}
