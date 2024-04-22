using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

	/// --------------------
	/// �� �ϐ��̐錾
	/// --------------------

	public GameObject playerPrefab;
	public GameObject boxPrefab;
	/// int�^�̔z��
	int[,] map;
	GameObject[,] filed;

	public GameObject clearText;

	public GameObject goalPrefab;

	public GameObject particlePrefab;



	Vector3 GetPosition(int row, int col) {
		return new Vector3(col - map.GetLength(1) / 2.0f, row - map.GetLength(0) / 2.0f, 0.0f);
	}



	/// ====================================================
	/// �� Player��Index��Ԃ�
	/// ====================================================
	Vector2Int GetPlayerIndex() {
		for (int row = 0; row < map.GetLength(0); row++) {
			for (int col = 0; col < map.GetLength(1); col++) {
				///- NULL�`�F�b�N
				if (filed[row, col] == null) { continue; }
				///- Tag�`�F�b�N; �^�O��Player���ǂ���
				if (filed[row, col].tag == "Player") {
					return new Vector2Int(col, row);
				}
			}
		}
		return new Vector2Int(-1, -1);
	}



	/// ====================================================
	/// �� �l�̓���ւ�
	/// ====================================================
	bool MoveNumber(string tag, Vector2Int moveForm, Vector2Int moveTo) {
		///- �z��O�Q�Ƃ����Ă����� false ��Ԃ�
		if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
		if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }

		if (filed[moveForm.y, moveForm.x].tag != tag) { return false; }

		///- ��(2)���������Ƃ��̏���
		if (filed[moveTo.y, moveTo.x] != null && filed[moveTo.y, moveTo.x].tag == "Box") {
			///- �ړ������̌v�Z; �ړ�����
			Vector2Int velocity = moveTo - moveForm;
			bool success = MoveNumber("Box", moveTo, moveTo + velocity);
			///- �����ړ��ł��Ȃ����Player���ړ��ł��Ȃ�
			if (!success) { return false; }
		}


		///- ���ۂ̈ʒu�����ւ�
		//filed[moveForm.y, moveForm.x].transform.position = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
		filed[moveForm.y, moveForm.x].transform.position = GetPosition(map.GetLength(0) - moveTo.y, moveTo.x);

		///- �z���̒l�����ւ�
		filed[moveTo.y, moveTo.x] = filed[moveForm.y, moveForm.x];
		filed[moveForm.y, moveForm.x] = null;


		///- �ړ������̂�Player�̂Ƃ��̏���
		if (filed[moveTo.y, moveTo.x].tag == "Player") {
			for (int i = 0; i < 20; i++) {

				Instantiate(
					particlePrefab,
					GetPosition(map.GetLength(0) - moveTo.y, moveTo.x) + new Vector3(0.0f, 0.0f, -0.0f),
					Quaternion.identity
				); ;

			}
		}


		return true;
	}



	/// ====================================================
	/// �� �N���A����
	/// ====================================================
	bool IsCleard() {
		List<Vector2Int> goals = new List<Vector2Int>();


		///- Goal�̏ꏊ���i�[
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {

				///- �i�[�ꏊ���ǂ���
				if (map[y, x] == 3) {
					goals.Add(new Vector2Int(x, y));
				}

			}
		}


		///- �N���A������s��; �S�[���Ɉ�ł������Ȃ���΃N���A�ł͂Ȃ�
		for (int i = 0; i < goals.Count; i++) {
			GameObject f = filed[goals[i].y, goals[i].x];
			if (f == null || f.tag != "Box") {
				return false;
			}
		}

		return true;
	}



	/// ====================================================
	/// �� Reset�p�֐�
	/// ====================================================
	void Reset() {


		///- Filed�����ׂ�null�ɂ���
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {
				//filed[y, x] = null;
				Destroy(filed[y, x]);
			}
		}


		/// ----------------------
		/// �� Object�̈ʒu��������
		/// ----------------------
		for (int row = 0; row < map.GetLength(0); row++) {
			for (int col = 0; col < map.GetLength(1); col++) {

				switch (map[row, col]) {
				case 1: ///- Playre
					filed[row, col] = Instantiate(
						playerPrefab,
						GetPosition(map.GetLength(0) - row, col),
						Quaternion.identity
					);
					break;
				case 2: ///- Box
					filed[row, col] = Instantiate(
						boxPrefab,
						GetPosition(map.GetLength(0) - row, col),
						Quaternion.identity
					);
					break;
				}
			}
		}



	}



	/// ====================================================
	/// �� Initializer Method
	/// ====================================================
	void Start() {


		///- window�t���X�N���[���Ȃǂ̐ݒ�
		Screen.SetResolution(1920, 1080, false);



		/// �z��̏�����; new �����Ă� delete�̕K�v���Ȃ�;
		map = new int[,] {
			{ 0, 0, 0, 0, 0 },
			{ 0, 3, 1, 3, 0 },
			{ 0, 2, 0, 2, 0 },
			{ 0, 0, 3, 2, 0 },
			{ 0, 0, 0, 0, 0 },
		};
		//PrintArray();

		///- GameObject�^�z���map�Ɠ����傫���ɏ�����
		filed = new GameObject[
			map.GetLength(0),
			map.GetLength(1)
		];



		///- Player�̏����ʒu��ݒ�
		for (int row = 0; row < map.GetLength(0); row++) {
			for (int col = 0; col < map.GetLength(1); col++) {

				switch (map[row, col]) {
				case 1: ///- Playre
					filed[row, col] = Instantiate(
						playerPrefab,
						GetPosition(map.GetLength(0) - row, col),
						Quaternion.identity
					);
					break;
				case 2: ///- Box
					filed[row, col] = Instantiate(
						boxPrefab,
						GetPosition(map.GetLength(0) - row, col),
						Quaternion.identity
					);
					break;
				case 3: ///- Goal
					Instantiate(
						goalPrefab,
						GetPosition(map.GetLength(0) - row, col) + new Vector3(0.0f, 0.0f, 0.01f),
						Quaternion.identity
					);

					break;
				}


			}
		}


	}



	/// ====================================================
	/// �� Update Method
	/// ====================================================
	void Update() {



		/// --------------------
		/// �� Reset
		/// --------------------
		if (Input.GetKeyUp(KeyCode.R)) {
			Reset();
		}





		/// --------------------
		/// �� �E�ֈړ�
		/// --------------------
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.right);
		}


		/// --------------------
		/// �� ���ֈړ�
		/// --------------------
		if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.left);
		}



		/// --------------------
		/// �� ��ֈړ�
		/// --------------------
		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + Vector2Int.down);
		}


		/// --------------------
		/// �� ���ֈړ�
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
