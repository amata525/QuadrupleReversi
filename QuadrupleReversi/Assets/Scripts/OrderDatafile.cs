using UnityEngine;
using System.Collections;

public class OrderDatafile : MonoBehaviour {

	// SelectStoneScriptから石の順番が格納される。
	// このスクリプトを設定するゲームオブジェクト OrderData は次のシーンでも消されず残る
	public int[] Player1Color = new int[4];
	public int[] Player2Color = new int[4];

	// Use this for initialization
	void Start () {
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
}
