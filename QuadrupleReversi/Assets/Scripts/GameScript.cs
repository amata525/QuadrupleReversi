using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	/* 変数ここから */ 

	// ボード情報
	int[,] board = new int[7, 7];
	int[,] canPut = new int[7, 7];

	GameObject[,] stoneList = new GameObject[7, 7];
	GameObject[,] markList = new GameObject[7, 7];

	// NGUI関係
	GameObject message1;
	GameObject message2;
	GameObject sprite1;
	GameObject sprite2;
	GameObject hpNum1;
	GameObject hpNum2;
	GameObject damage1;
	GameObject damage2;
	GameObject next1;
	GameObject next2;
	GameObject againButton1;
	GameObject againButton2;
	GameObject endButton1;
	GameObject endButton2;

	UILabel messageP1;
	UILabel messageP2;
	UILabel nextMark1;
	UILabel nextMark2;
	UILabel HPnumP1;
	UILabel HPnumP2;
	UILabel damageNumP1;
	UILabel damageNumP2;
	UISlider HPbarP1;
	UISlider HPbarP2;

	// 初期化フラグ
	int initFlag;

	// 次の色、ターン
	OrderDatafile Data;
	int[] nextP1 = new int[4] {1, 3, 2, 3};
	int[] nextP2 = new int[4] {2, 4, 1, 4};
	int countP1;
	int countP2;
	int turn;

	// 置ける場所があるかないか
	int nothing ;

	// 挟んだライン数と石の数
	int line ;
	int num ;

	// HP
	int hpP1;
	int hpP2;

	// 操作停止時間
	int waitTime;

	// ボードをひっくり返す時間、フラグ
	int turnCount;
	int turnFlag;

	// ダメージ処理の時間
	int damageCount;
	int damegeFlag;

	// ゲーム終了フラグ
	int gameEnd;

	/* 変数ここまで */ 

	// ゲーム起動時にやること
	void Start ()
	{
		GameObject DataObj;

		DataObj = GameObject.Find("OrderData");    // OrderDatafileスクリプトにある石の順番を読み込む

		if (DataObj != null)
		{
			Data = DataObj.GetComponent<OrderDatafile>();

			for (int i = 0; i < 4; i++)
			{
				nextP1[i] = Data.Player1Color[i];
				nextP2[i] = Data.Player2Color[i];
			}
		}

		message1 = GameObject.Find("Message1");
		message2 = GameObject.Find("Message2");
		sprite1 = GameObject.Find("Sprite1");
		sprite2 = GameObject.Find("Sprite2");
		hpNum1 = GameObject.Find("P1HPnum");
		hpNum2 = GameObject.Find("P2HPnum");
		damage1 = GameObject.Find("P1Damage");
		damage2 = GameObject.Find("P2Damage");
		next1 = GameObject.Find("NextMark1");
		next2 = GameObject.Find("NextMark2");
		againButton1 = GameObject.Find("AgainButton1");
		againButton2 = GameObject.Find("AgainButton2");
		endButton1 = GameObject.Find("EndButton1");
		endButton2 = GameObject.Find("EndButton2");

		// 最初から見えてる必要のないGUI達
		// Inspectorのチェックボックスでもfalseにできるが、Findで見つけられなくなるようだ
		message1.SetActive(false);
		message2.SetActive(false);
		sprite1.SetActive(false);
		sprite2.SetActive(false);
		damage1.SetActive(false);
		damage2.SetActive(false);
		againButton1.SetActive(false);
		againButton2.SetActive(false);
		endButton1.SetActive(false);
		endButton2.SetActive(false);

		// .textでLabelを編集できるように
		nextMark1 = next1.GetComponent<UILabel>();
		nextMark2 = next2.GetComponent<UILabel>();
		HPnumP1 = hpNum1.GetComponent<UILabel>();
		HPnumP2 = hpNum2.GetComponent<UILabel>();
		damageNumP1 = damage1.GetComponent<UILabel>();
		damageNumP2 = damage2.GetComponent<UILabel>();
		messageP1 = message1.GetComponent<UILabel>();
		messageP2 = message2.GetComponent<UILabel>();

		// .sliderValueでHPバーを可変できるように
		HPbarP1 = GameObject.Find("P1HPbar").GetComponent<UISlider>();
		HPbarP2 = GameObject.Find("P2HPbar").GetComponent<UISlider>();

		// NGUI2.7とNGUI3.xでここの変数名が違う模様 NGUI3.xならば.valueでOK 
		// UISliderの中身を見てこの数値を変更すれば良いと思ったが、間違っている可能性があるのでメモ
		// 動作は希望通り0～1fの間でHPバーの幅を調節できる。
		HPbarP1.sliderValue = 1f;
		HPbarP2.sliderValue = 1f;

		// 各ステータスの初期化
		line = 0;
		num = 0;
		turn = 1;

		countP1 = 0;
		countP2 = 0;

		hpP1 = 100;
		hpP2 = 100;

		initFlag = 0; 
		nothing = 0;
		waitTime = -1;
		turnCount = 0;
		turnFlag = 0;
		damageCount = 0;
		damegeFlag = 0;

		gameEnd = 0;

		for (int i = 0; i < 4; i++)
		{
			GameObject.Find("P1Next" + i).renderer.material = (Material)Resources.Load("Color" + nextP1[i]);
			GameObject.Find("P2Next" + i).renderer.material = (Material)Resources.Load("Color" + nextP2[i]);
		}

		// プレイヤー１の先攻でゲームスタート!!
		initBoard();
		checkCanPut(nextP1[countP1]);
		putStoneMark(nextP1[countP1]);
	}
	
	// 1フレームごとにやること
	void Update () 
	{
		// 置く石を回転させる
		if (gameEnd == 0)
		{
			if (turn == 1)
			{
				GameObject.Find("P1Next" + countP1).transform.Rotate(0, -2, -5);
			}
			else
			{
				GameObject.Find("P2Next" + countP2).transform.Rotate(0, 2, 5);
			}
		}

		
		// 石の回転等の処理待ち
		if (waitTime > 0)
		{
			waitTime--;
		}
		// 処理待ち時間終了
		else if (waitTime == 0)
		{
			// プレイヤーのHPが0ならば、ゲーム終了
			if (hpP1 == 0)
			{
				winMessage(2);
			}
			else if (hpP2 == 0)
			{
				winMessage(1);

			}

			if (gameEnd == 0)
			{
				// ダメージの処理
				if (damegeFlag == 1)
				{
					damageManage();
				}
				// ボードに空きマスが無い　ボードをひっくり返す
				else if (turnFlag == 1)
				{
					turnBoard();
				}
				else
				{
					if (initFlag == 1)
					{
						initBoard();
						initFlag = 0;
					}
					// 次のプレイヤーへ
					switchTurn();
					//　置ける位置のチェック
					if (turn == 1)
					{
						checkCanPut(nextP1[countP1]);
						putStoneMark(nextP1[countP1]);
					}
					else
					{
						checkCanPut(nextP2[countP2]);
						putStoneMark(nextP2[countP2]);
					}

					// 置ける位置が無い場合メッセージを表示
					if (nothing == 1)
					{
						cannotPutMessage();
					}
					waitTime--;
				}
			}
		}
		else
		{
			// 左クリックでマウスの座標を検出 
			if (Input.GetButtonDown("Fire1"))
			{
				Vector3 screenPoint = Input.mousePosition;

				// 奥行き設定
				screenPoint.z = 10;

				// カメラから見た座標に変換
				Vector3 v = Camera.main.ScreenToWorldPoint(screenPoint);

				GameMain(v);
			}
		}
	}

	// ゲームのメイン処理、マウスの座標を与える
	void GameMain(Vector3 v)
	{
		int x, y;
		
		//小数点以下切り捨て 
		v.x += 0.5f;
		v.z += 0.5f;

		x = (int)Mathf.Floor(v.x);
		y = (int)Mathf.Floor(v.z);

		if (0 <= x && x <= 6 && 0 <= y && y <= 6 && gameEnd == 0)
		{
			// 置ける場所が無い 石が置いていない好きな場所に置いて終了
			if (nothing == 1)
			{

				// 石がないところへ置く
				if (canPut[x, y] == 0)
				{
					if (turn == 1)
					{
						putStone(x, y, nextP1[countP1]);					
					}
					else
					{
						putStone(x, y, nextP2[countP2]);
					}

					// メッセージボックスを消す
					deleteMessage();
					waitTime = 0;
				}
			}
			// 置ける場所があるのでひっくり返せる
			else
			{
				if (canPut[x, y] == 1)
				{
					if (turn == 1)
					{
						putStone(x, y, nextP1[countP1]);
						changeStone(x, y);

					}
					else
					{
						putStone(x, y, nextP2[countP2]);
						changeStone(x, y);

					}
					// 古いマークを消す
					destroyMark();

					// ダメージ処理フラグを立てる
					damegeFlag = 1;
					damageCount = 0;

					// 石を裏返す処理待ち時間セット
					waitTime = 10;
				}
			}


			// 全てのマスが埋まっているので板をリセット
			if (emptyCheck())
			{
				turnCount = 0;
				turnFlag = 1;
				waitTime += 60;
			}

			if (hpP1 == 0 || hpP2 == 0) waitTime = 100;
		}
	}

	// ボード情報をクリアし、石を初期配置で並べる
	void initBoard()
	{
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				board[i, j] = 0;

				GameObject.Destroy(stoneList[i, j]);
				stoneList[i, j] = null;
			}
		}

		// 初期配置
		putStone(2, 2, 3);
		putStone(2, 3, 2);
		putStone(3, 2, 4);
		putStone(3, 3, 1);
		putStone(3, 4, 3);
		putStone(4, 3, 2);
		putStone(4, 4, 4);

		deleteMessage();
	}

	// 色を渡すと、石を置ける座標に1を入れる。すでに石がある座標は2 置けない場合は0
	// 格納する配列はcanPut[7,7] 置ける場所がどこにも無い場合はnothingに1を格納
	void checkCanPut(int c)
	{
		int put;

		int[] vecX = { -1, -1, -1, 0, 0, 1, 1, 1 };
		int[] vecY = { -1, 0, 1, -1, 1, -1, 0, 1 };
		int v;
		int x, y;

		nothing = 1;

		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				put = 0;
				v = 0;

				// 石が置かれていない場所のみサーチ
				if (board[i, j] == 0)
				{
					while (v < 8)
					{
						x = i + vecX[v];
						y = j + vecY[v];

						if (0 <= x && x <= 6 && 0 <= y && y <= 6)
						{
							// 1マス先が自分の色ではなく、石が置かれている
							if (board[x, y] != c && board[x, y] != 0)
							{
								x += vecX[v];
								y += vecY[v];
								while (0 <= x && x <= 6 && 0 <= y && y <= 6)
								{
									// 進んだところに自分の色発見 石が置ける
									if (board[x, y] == c)
									{
										put = 1;
										nothing = 0;
										x = 99;
										v = 99;
									}
									// 空白ならば置けない
									else if (board[x, y] == 0)
									{
										x = 99;
									}
									else
									{
										x += vecX[v];
										y += vecY[v];
									}
								}
							}
						}
						v++;
					}

					if (put == 1)
					{
						canPut[i, j] = 1;
					}
					else
					{
						canPut[i, j] = 0;
					}
				}
				else
				{
					canPut[i, j] = 2;
				}
			}
		}
	}

	// 指定された場所に石を置く すでにある場合は色を上書き
	void putStone(int x, int y, int c)
	{
		// 座標の外だったら置けない
		if (x < 0 || 6 < x || y < 0 || 6 < y)
		{
			return;
		}

		// 何も無いならば石を生成
		if (board[x, y] == 0)
		{
			GameObject stonePrefab = (GameObject)Resources.Load("StonePrefab");
			stoneList[x, y] = Instantiate(stonePrefab) as GameObject;
			stoneList[x, y].transform.localPosition = new Vector3(x, 1, y);	
		}

		// 色チェンジ
		stoneList[x, y].renderer.material = (Material)Resources.Load("Color" + c);
		board[x, y] = c;
	}

	// 石が置ける場所にマークをつける
	void putStoneMark(int c)
	{
		GameObject putMarkPrefab = (GameObject)Resources.Load("PutMarkPrefab");

		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				if (canPut[i, j] == 1)
				{
					markList[i, j] = Instantiate(putMarkPrefab) as GameObject;
					markList[i, j].transform.localPosition = new Vector3(i, 0.7f, j);
					markList[i, j].renderer.material = (Material)Resources.Load("Color" + c);
				}
			}
		}
	}

	// マークを削除する
	void destroyMark()
	{
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				if (markList[i, j] != null)
				{
					GameObject.Destroy(markList[i, j]);
				}
			}

		}
	}

	// 座標を渡すと、挟んだ石の色を変える
	// 変えた石の個数とライン数が更新される
	void changeStone(int oriX, int oriY)
	{
		int[] vecX = { -1, -1, -1, 0, 0, 1, 1, 1 };
		int[] vecY = { -1, 0, 1, -1, 1, -1, 0, 1 };
		int v;
		int x, y, c;
		int counter;

		c = board[oriX, oriY];

		num = 0;
		line = 0;

		v = 0;
		while (v < 8)
		{
			x = oriX + vecX[v];
			y = oriY + vecY[v];

			if (0 <= x && x <= 6 && 0 <= y && y <= 6)
			{
				// 1マス先が自分の色ではなく、石が置かれている
				if (board[x, y] != c && board[x, y] != 0)
				{
					x += vecX[v];
					y += vecY[v];
					counter = 1;

					while (0 <= x && x <= 6 && 0 <= y && y <= 6)
					{	
						// 進んだところに自分の色発見 挟んだ色を自分の色にする
						if (board[x, y] == c)
						{
							for (int i = 0; i < counter; i++)
							{
								x -= vecX[v];
								y -= vecY[v];
								
								// 変えた石の数++
								num++;

								// 色チェンジ
								stoneList[x, y].GetComponent<ReverseScript>().setReverse(c, (counter - i) * 10, x, y, vecX[v], vecY[v]);
								
								board[x, y] = c;
							}
							x = 99;

							//変えたライン数++
							line++;
						}
						// 空白ならば挟む石無し
						else if (board[x, y] == 0)
						{
							x = 99;
						}
						else
						{
							x += vecX[v];
							y += vecY[v];
							counter++;
						}
					}
				}
			}
			v++;
		}
	}

	// 全てのマスが埋まっているかチェックする
	bool emptyCheck()
	{
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++) {
				if(board[i,j] == 0){
					i = 99;
					j = 99;
					return false;
				}
			}
		}

		return true;
	}

	// turnを次のプレイヤーにする。次に置く石の番号を進める。 
	void switchTurn()
	{
		if (turn == 1)
		{
			GameObject.Find("P1Next" + countP1).transform.rotation = Quaternion.Euler(0, 0, 0);
			
			countP1++;
			if (countP1 > 3) countP1 = 0;

			nextMark1.transform.localPosition = new Vector3(-425, 261 - 174 * countP1, 0);
			
			turn = 2;
		}
		else
		{
			GameObject.Find("P2Next" + countP2).transform.rotation = Quaternion.Euler(0, 0, 0);
			
			countP2++;
			if (countP2 > 3) countP2 = 0;

			nextMark2.transform.localPosition = new Vector3(425, -261 + 174 * countP2, 0);

			turn = 1;
		}
	}

	// ダメージの表示とHPゲージの減少 Updateで呼び出す
	void damageManage()
	{
		if (damageCount == 0)
		{
			damage1.SetActive(true);
			damage2.SetActive(true);

			if (turn == 1)
			{
				damageNumP1.color = new Color(252, 255, 255);
				damageNumP2.color = new Color(250, 0, 0);
			}
			else
			{
				damageNumP1.color = new Color(250, 0, 0);
				damageNumP2.color = new Color(252, 255, 255);
			}

			damageNumP1.text = num.ToString();
			damageNumP2.text = num.ToString();
		}
		if (damageCount == 10)
		{
			damageNumP1.text = num + "×";
			damageNumP2.text = num + "×";
		}
		if (damageCount == 20)
		{
			damageNumP1.text = num + "×" + line;
			damageNumP2.text = num + "×" + line;
		}
		if (damageCount == 30)
		{
			if (turn == 1)
			{
				damageNumP1.text = (num * line).ToString();
				damageNumP2.text = (-num * line).ToString();
			}
			else
			{
				damageNumP1.text = (-num * line).ToString();
				damageNumP2.text = (num * line).ToString();
			}
		}
		
		if (30 < damageCount && damageCount <= 30 + num * line * 3)
		{
			if (damageCount % 3 == 0)
			{
				if (turn == 1)
				{
					if (hpP2 > 0) hpP2--;
					HPnumP2.text = hpP2.ToString();
					HPbarP2.sliderValue = hpP2 / 100f;
				}
				else
				{
					if (hpP1 > 0) hpP1--;
					HPnumP1.text = hpP1.ToString();
					HPbarP1.sliderValue = hpP1 / 100f;
				}
			}
		}
		else if (60 + num * line * 3 < damageCount)
		{
			damage1.SetActive(false);
			damage2.SetActive(false);
			damegeFlag = 0;
		}
		damageCount++;
	}

	// ボードに空きマスが無くなった時、ボードをひっくり返してリセット
	void turnBoard()
	{
		GameObject.Find("Board").transform.Rotate(1, 0, 0);

		if (turnCount == 0)
		{
			// それっぽく見せるために石を跳ねさせる。
			for (int i = 0; i < 7; i++)
				for (int j = 0; j < 7; j++)
					if (stoneList[i, j] != null)
						stoneList[i, j].GetComponent<ReverseScript>().setReverse(0, 1, i, j, 1, 0);

		}
		else if (turnCount < 90)
		{
			GameObject.Find("Board").transform.position = new Vector3(3, (-turnCount) / 50f, 3);
		}
		else if(turnCount < 180)
		{
			GameObject.Find("Board").transform.position = new Vector3(3, (turnCount-180) / 50f, 3);
		}
		else
		{
			GameObject.Find("Board").transform.position = new Vector3(3, 0, 3);
			GameObject.Find("Board").transform.rotation = Quaternion.Euler(0, 0, 0);
			initFlag = 1;
			turnFlag = 0;
		}
		turnCount++;
	}

	// 石が置けない場合のメッセージを表示する
	void cannotPutMessage()
	{

		if (turn == 1)
		{
			message1.SetActive(true);
			sprite1.SetActive(true);
		}
		else
		{
			message2.SetActive(true);
			sprite2.SetActive(true);
		}
	}

	// メッセージを消す。
	void deleteMessage()
	{
		if (turn == 1)
		{
			message1.SetActive(false);
			sprite1.SetActive(false);
		}
		else
		{
			message2.SetActive(false);
			sprite2.SetActive(false);
		}
	}

	// ゲーム終了画面の表示
	void winMessage(int winPlayer)
	{

		gameEnd = 1;

		message1.SetActive(true);
		sprite1.SetActive(true);
		message2.SetActive(true);
		sprite2.SetActive(true);

		if (winPlayer == 1)
		{
			// メッセージ表示
			messageP1.text = "\nYOU WIN!!";
			messageP2.text = "\nYOU LOSE...";

			// もう一度遊ぶかどうかは負けたプレイヤーに聞く
			againButton2.SetActive(true);
			endButton2.SetActive(true);

			for (int i = 6; i >= 0; i--)
			{
				for (int j = 6; j >= 0; j--)
				{
					if (stoneList[i, j] != null)  
						stoneList[i, j].GetComponent<ReverseScript>().setReverse(3, ((6-i) + (6-j) + 1) * 10, i, j, -1, -1);
				}
			}

			GameObject.Find("P2Next" + countP2).transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		else
		{
			messageP1.text = "\nYOU LOSE...";
			messageP2.text = "\nYOU WIN!!";

			againButton1.SetActive(true);
			endButton1.SetActive(true);

			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					if (stoneList[i, j] != null)
						stoneList[i, j].GetComponent<ReverseScript>().setReverse(4, (i + j + 1) * 10, i, j, 1, 1);
				}
			}

			GameObject.Find("P1Next" + countP1).transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}

	// もう一度遊ぶ！（Again）ボタンが押された時の処理
	void PressedAgain()
	{
		Application.LoadLevel("SelectStone");
	}

	// タイトルへ戻る（End）ボタンが押された時の処理
	void PressedEnd()
	{
		Application.LoadLevel("Title");
	}
}
