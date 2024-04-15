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
    /// int型の配列
    int[,] map;
    GameObject[,] filed;


    /// --------------------
    /// ↓ PlayerのIndexを返す
    /// --------------------
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



    /// --------------------
    /// ↓ 値の入れ替え
    /// --------------------
    bool MoveNumber(string tag, Vector2Int moveForm, Vector2Int moveTo) {
        ///- 配列外参照をしていたら false を返す
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }

        if (filed[moveForm.y, moveForm.x].tag != tag) { return false; }

        ///- 箱(2)があったときの処理
        //if (map[moveTo] == 2) {
        //    ///- 移動方向の計算; 移動処理
        //    int velocity = moveTo - moveForm;
        //    bool success = MoveNumber(2, moveTo, moveTo + velocity);
        //    ///- 箱が移動できなければPlayerも移動できない
        //    if (!success) { return false; }
        //}

        map[moveTo.y, moveTo.x] = 1;
        map[moveForm.y, moveForm.x] = 0;

        ///- 実際の位置を入れ替え
        filed[moveForm.y, moveForm.x].transform.position = new Vector3(moveTo.x, moveTo.y, 0);

        ///- 配列上の値を入れ替え
        filed[moveTo.y, moveTo.x] = filed[moveForm.y, moveForm.x];
        filed[moveForm.y, moveForm.x] = null;

        return true;
    }


    /// --------------------
    /// ↓ Initializer Method
    /// --------------------
    void Start() {

        /// 配列の初期化; new をしても deleteの必要がない;
        map = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
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
                if (map[row, col] == 1) {
                    filed[row, col] = Instantiate(
                        playerPrefab,
                        new Vector3(col, map.GetLength(0) - row, 0),
                        Quaternion.identity
                    );
                }
            }
        }


    }

    /// --------------------
    /// ↓ Update Method
    /// --------------------
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
            MoveNumber("Player", playerIndex, playerIndex + Vector2Int.up);
        }


        /// --------------------
        /// ↓ 下へ移動
        /// --------------------
        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + Vector2Int.down);
        }


    }
}
