using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
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
	public GameObject wallPrefab;


	Stack<GameObject[,]> filedLists = new Stack<GameObject[,]>();

	/// ====================================================
	/// ↓ indexから現在の位置を求める
	/// ====================================================
	Vector3 GetPosition(int row, int col) {
		//return new Vector3(row - map.GetLength(1) / 2.0f + 0.5f, col - map.GetLength(0) / 2.0f, 0.0f);
		return new Vector3(col - map.GetLength(1) / 2.0f + 0.5f, -row + map.GetLength(0) / 2.0f, 0.0f);
	}



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
		if (filed[moveTo.y, moveTo.x] != null && filed[moveTo.y, moveTo.x].tag == "Wall") {
			return false;
		}

		///- 箱(2)があったときの処理
		if (filed[moveTo.y, moveTo.x] != null && filed[moveTo.y, moveTo.x].tag == "Box") {
			///- 移動方向の計算; 移動処理
			Vector2Int velocity = moveTo - moveForm;
			bool success = MoveNumber("Box", moveTo, moveTo + velocity);
			///- 箱が移動できなければPlayerも移動できない
			if (!success) { return false; }
		}


		///- 実際の位置を入れ替え
		//filed[moveForm.y, moveForm.x].transform.position = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
		filed[moveForm.y, moveForm.x].transform.position = GetPosition(moveTo.y, moveTo.x);

		///- 配列上の値を入れ替え
		filed[moveTo.y, moveTo.x] = filed[moveForm.y, moveForm.x];
		filed[moveForm.y, moveForm.x] = null;


		///- 移動したのがPlayerのときの処理
		if (filed[moveTo.y, moveTo.x].tag == "Player") {

			Vector3 form = new Vector3(moveForm.x, -moveForm.y, 0.0f);
			Vector3 to = new Vector3(moveTo.x, -moveTo.y, 0.0f);

			filed[moveTo.y, moveTo.x].transform.rotation = Quaternion.LookRotation(to - form, Vector3.back);

			for (int i = 0; i < 20; i++) {

				Instantiate(
					particlePrefab,
					GetPosition(moveTo.y, moveTo.x) + new Vector3(0.0f, 0.0f, -0.0f),
					Quaternion.identity
				); ;

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
		for (int y = 0; y < map.GetLength(1); y++) {
			for (int x = 0; x < map.GetLength(0); x++) {

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
	/// ↓ Reset用関数
	/// ====================================================
	void Reset() {


		///- Filedをすべてnullにする
		for (int y = 0; y < map.GetLength(1); y++) {
			for (int x = 0; x < map.GetLength(0); x++) {
				//filed[y, x] = null;
				Destroy(filed[y, x]);
			}
		}


		/// ----------------------
		/// ↓ Objectの位置を初期化
		/// ----------------------
		for (int row = 0; row < map.GetLength(1); row++) {
			for (int col = 0; col < map.GetLength(0); col++) {

				switch (map[row, col]) {
				case 1: ///- Playre
					filed[row, col] = Instantiate(
						playerPrefab,
						GetPosition(row, col),
						Quaternion.LookRotation(new Vector3(0.0f, 1.0f, 0.0f))
					);
					break;
				case 2: ///- Box
					filed[row, col] = Instantiate(
						boxPrefab,
						GetPosition(row, col),
						Quaternion.identity
					);
					break;
				case 9: ///- Wall

					filed[row, col] = Instantiate(
						wallPrefab,
						GetPosition(row, col),
						Quaternion.identity
					);

					break;
				}
			}
		}



	}



	/// ====================================================
	/// ↓ 1つ戻る
	/// ====================================================
	void Undo() {

		if (filedLists.Count <= 1) { return; }

		//for (int y = 0; y < filed.GetLength(1); ++y) {
		//	for (int x = 0; x < filed.GetLength(0); ++x) {
		//		if (filed[y, x] != null && filed[y, x].tag != "Wall") {
		//			Destroy(filed[y, x]);
		//		}
		//	}
		//}


		GameObject[,] copy = CopyFiled(filedLists.Pop());

		for (int y = 0; y < filed.GetLength(1); ++y) {
			for (int x = 0; x < filed.GetLength(0); ++x) {

				if (copy[y, x] != null) {
					filed[y, x] = copy[y, x];
					//filed[y, x].tag = copy[y, x].tag;
					filed[y, x].transform.position = GetPosition(y, x);
				}
			}
		}

	}


	/// ------------------------------------
	/// 引数のGameObject型の二次元配列をコピーする
	/// ------------------------------------
	GameObject[,] CopyFiled(GameObject[,] copyFrom) {
		GameObject[,] result = new GameObject[copyFrom.GetLength(0), copyFrom.GetLength(1)];

		for (int y = 0; y < copyFrom.GetLength(1); ++y) {
			for (int x = 0; x < copyFrom.GetLength(0); ++x) {
				result[y, x] = copyFrom[y, x];
			}
		}

		return result;
	}


	/// ====================================================
	/// ↓ Initializer Method
	/// ====================================================
	void Start() {


		///- windowフルスクリーンなどの設定
		Screen.SetResolution(1920, 1080, false);



		/// 配列の初期化; new をしても deleteの必要がない;
		map = new int[,] {
			{ 9, 9, 9, 9, 9, 9, 9},
			{ 9, 0, 0, 0, 0, 0, 9},
			{ 9, 0, 3, 1, 3, 0, 9},
			{ 9, 0, 2, 0, 2, 0, 9},
			{ 9, 0, 0, 3, 2, 0, 9},
			{ 9, 0, 0, 0, 0, 0, 9},
			{ 9, 9, 9, 9, 9, 9, 9},
		};
		//PrintArray();

		///- GameObject型配列をmapと同じ大きさに初期化
		filed = new GameObject[
			map.GetLength(0),
			map.GetLength(1)
		];



		///- Objectの初期位置を設定
		for (int row = 0; row < map.GetLength(1); row++) {
			for (int col = 0; col < map.GetLength(0); col++) {

				switch (map[row, col]) {
				case 1: ///- Player
					filed[row, col] = Instantiate(
						playerPrefab,
						GetPosition(row, col),
						Quaternion.LookRotation(new Vector3(0.0f, 1.0f, 0.0f))
					);

					break;
				case 2: ///- Box
					filed[row, col] = Instantiate(
						boxPrefab,
						GetPosition(row, col),
						Quaternion.identity
					);
					break;
				case 3: ///- Goal
					Instantiate(
						goalPrefab,
						GetPosition(row, col) + new Vector3(0.0f, 0.0f, 0.01f),
						Quaternion.identity
					);

					break;
				case 9: ///- Wall

					filed[row, col] = Instantiate(
						wallPrefab,
						GetPosition(row, col),
						Quaternion.identity
					);

					break;
				}


			}
		}



		filedLists.Push(filed);



	}



	/// ====================================================
	/// ↓ Update Method
	/// ====================================================
	void Update() {



		/// --------------------
		/// ↓ Reset
		/// --------------------
		if (Input.GetKeyUp(KeyCode.R)) {
			Reset();
		}


		/// --------------------
		/// ↓ Undo
		/// --------------------
		if (Input.GetKeyUp(KeyCode.U)) {
			Undo();
		}


		/// --------------------
		/// ↓ 右へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			GameObject[,] copy = CopyFiled(filed);
			if (MoveNumber("Player", playerIndex, playerIndex + Vector2Int.right)) {
				filedLists.Push(copy);
			}
		}


		/// --------------------
		/// ↓ 左へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			GameObject[,] copy = CopyFiled(filed);
			if (MoveNumber("Player", playerIndex, playerIndex + Vector2Int.left)) {
				filedLists.Push(copy);
			}
		}



		/// --------------------
		/// ↓ 上へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			GameObject[,] copy = CopyFiled(filed);
			if (MoveNumber("Player", playerIndex, playerIndex + Vector2Int.down)) {
				filedLists.Push(copy);
			}
		}


		/// --------------------
		/// ↓ 下へ移動
		/// --------------------
		if (Input.GetKeyUp(KeyCode.DownArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			GameObject[,] copy = CopyFiled(filed);
			if (MoveNumber("Player", playerIndex, playerIndex + Vector2Int.up)) {
				filedLists.Push(copy);
			}
		}


		if (IsCleard()) {
			//Debug.Log("CLEAR!!!");
			clearText.SetActive(true);
		}

	}
}
