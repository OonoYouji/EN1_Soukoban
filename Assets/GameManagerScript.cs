using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

	/// --------------------
	/// ↓ 変数の宣言
	/// --------------------

	public GameObject playerPrefab;
	public GameObject boxPrefab;
	/// int型の配列
	int[,] map;
	GameObject[,] filed;

	public GameObject clearText;

	public GameObject goalPrefab;

	public GameObject particlePrefab;


	/// ====================================================
	/// ↓ PlayerのIndexを返す
	/// ====================================================
	Vector2Int GetPlayerIndex() {
		for (int row = 0; row < map.GetLength(0); row++) {
			for (int col = 0; col < map.GetLength(1); col++) {
				///- NULLチェック
				if (filed[row, col] == null) { continue; }
				///- Tagチェック; タグがPlayerかどうか
				if (filed[row, col].tag == "Player") {
					return new Vector2Int(col, row);
				}
			}
		}
		return new Vector2Int(-1, -1);
	}



	/// ====================================================
	/// ↓ 値の入れ替え
	/// ====================================================
	bool MoveNumber(string tag, Vector2Int moveForm, Vector2Int moveTo) {
		///- 配列外参照をしていたら false を返す
		if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
		if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }

		if (filed[moveForm.y, moveForm.x].tag != tag) { return false; }

		///- 箱(2)があったときの処理
		if (filed[moveTo.y, moveTo.x] != null && filed[moveTo.y, moveTo.x].tag == "Box") {
			///- 移動方向の計算; 移動処理
			Vector2Int velocity = moveTo - moveForm;
			bool success = MoveNumber("Box", moveTo, moveTo + velocity);
			///- 箱が移動できなければPlayerも移動できない
			if (!success) { return false; }
		}


		///- 実際の位置を入れ替え
		filed[moveForm.y, moveForm.x].transform.position = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);

		///- 配列上の値を入れ替え
		filed[moveTo.y, moveTo.x] = filed[moveForm.y, moveForm.x];
		filed[moveForm.y, moveForm.x] = null;


		///- 移動したのがPlayerのときの処理
		if (filed[moveTo.y, moveTo.x].tag == "Player") {
			for (int i = 0; i < 20; i++) {

				Instantiate(
					particlePrefab,
					new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0.01f),
					Quaternion.identity
				);

			}
		}


		return true;
	}



	/// ====================================================
	/// ↓ クリア判定
	/// ====================================================
	bool IsCleard() {
		List<Vector2Int> goals = new List<Vector2Int>();


		///- Goalの場所を格納
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {

				///- 格納場所かどうか
				if (map[y, x] == 3) {
					goals.Add(new Vector2Int(x, y));
				}

			}
		}


		///- クリア判定を行う; ゴールに一つでも箱がなければクリアではない
		for (int i = 0; i < goals.Count; i++) {
			GameObject f = filed[goals[i].y, goals[i].x];
			if (f == null || f.tag != "Box") {
				return false;
			}
		}

		return true;
	}



	/// ====================================================
	/// ↓ Initializer Method
	/// ====================================================
	void Start() {


		///- windowフルスクリーンなどの設定
		Screen.SetResolution(1920, 1080, false);



		/// 配列の初期化; new をしても deleteの必要がない;
		map = new int[,] {
			{ 0, 0, 0, 0, 0 },
			{ 0, 3, 1, 3, 0 },
			{ 0, 2, 0, 2, 0 },
			{ 0, 0, 3, 2, 0 },
			{ 0, 0, 0, 0, 0 },
		};
		//PrintArray();

		///- GameObject型配列をmapと同じ大きさに初期化
		filed = new GameObject[
			map.GetLength(0),
			map.GetLength(1)
		];



		///- Playerの初期位置を設定
		for (int row = 0; row < map.GetLength(0); row++) {
			for (int col = 0; col < map.GetLength(1); col++) {

				switch (map[row, col]) {
				case 1: ///- Playre
					filed[row, col] = Instantiate(
						playerPrefab,
						new Vector3(col, map.GetLength(0) - row, 0),
						Quaternion.identity
					);
					break;
				case 2: ///- Box
					filed[row, col] = Instantiate(
						boxPrefab,
						new Vector3(col, map.GetLength(0) - row, 0),
						Quaternion.identity
					);
					break;
				case 3: ///- Goal
					Instantiate(
						goalPrefab,
						new Vector3(col, map.GetLength(0) - row, 0.01f),
						Quaternion.identity
					);

					break;
				}


			}
		}


	}



	/// ====================================================
	/// ↓ Update Method
	/// ====================================================
	void Update() {

		/// --------------------
		/// ↓ 右へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.right);
		}


		/// --------------------
		/// ↓ 左へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.left);
		}



		/// --------------------
		/// ↓ 上へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.down);
		}


		/// --------------------
		/// ↓ 下へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.DownArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.up);
		}


		if (IsCleard()) {
			//Debug.Log("CLEAR!!!");
			clearText.SetActive(true);
		}

	}
}
