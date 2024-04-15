using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    /// --------------------
    /// �� �ϐ��̐錾
    /// --------------------

    public GameObject playerPrefab;
    /// int�^�̔z��
    int[,] map;
    GameObject[,] filed;


    /// --------------------
    /// �� Player��Index��Ԃ�
    /// --------------------
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



    /// --------------------
    /// �� �l�̓���ւ�
    /// --------------------
    bool MoveNumber(string tag, Vector2Int moveForm, Vector2Int moveTo) {
        ///- �z��O�Q�Ƃ����Ă����� false ��Ԃ�
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }

        if (filed[moveForm.y, moveForm.x].tag != tag) { return false; }

        ///- ��(2)���������Ƃ��̏���
        //if (map[moveTo] == 2) {
        //    ///- �ړ������̌v�Z; �ړ�����
        //    int velocity = moveTo - moveForm;
        //    bool success = MoveNumber(2, moveTo, moveTo + velocity);
        //    ///- �����ړ��ł��Ȃ����Player���ړ��ł��Ȃ�
        //    if (!success) { return false; }
        //}

        map[moveTo.y, moveTo.x] = 1;
        map[moveForm.y, moveForm.x] = 0;

        ///- ���ۂ̈ʒu�����ւ�
        filed[moveForm.y, moveForm.x].transform.position = new Vector3(moveTo.x, moveTo.y, 0);

        ///- �z���̒l�����ւ�
        filed[moveTo.y, moveTo.x] = filed[moveForm.y, moveForm.x];
        filed[moveForm.y, moveForm.x] = null;

        return true;
    }


    /// --------------------
    /// �� Initializer Method
    /// --------------------
    void Start() {

        /// �z��̏�����; new �����Ă� delete�̕K�v���Ȃ�;
        map = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
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
    /// �� Update Method
    /// --------------------
    void Update() {

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
            MoveNumber("Player", playerIndex, playerIndex + Vector2Int.up);
        }


        /// --------------------
        /// �� ���ֈړ�
        /// --------------------
        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + Vector2Int.down);
        }


    }
}
