using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Particle : MonoBehaviour {



    /// ========================================
    /// ↓ 変数の宣言
    /// ========================================

    ///- 消滅するまでの時間
    private float lifeTime;
    ///- 消滅するまでの残り時間
    private float leftLifeTime;
    ///- 移動量
    private Vector3 velocity;
    ///- 初期scale
    private Vector3 defaultScale;




    /// ========================================
    /// ↓ 初期化
    /// ========================================
    void Start() {

        lifeTime = 0.3f;
        leftLifeTime = lifeTime;

        defaultScale = transform.localScale;

        float maxVelocity = 5;
        velocity = new Vector3(
            Random.Range(-maxVelocity, maxVelocity),
            Random.Range(-maxVelocity, maxVelocity),
            0
        );



    }




    /// ========================================
    /// ↓ 更新処理
    /// ========================================
    void Update() {


        ///- 残り時間の減少
        leftLifeTime -= Time.deltaTime;

        ///- 自身の座標移動
        transform.position += velocity * Time.deltaTime;

        ///- 残り時間によりScaleを小さくする
        transform.localScale = Vector3.Lerp(
            new Vector3(0, 0, 0),
            defaultScale,
            leftLifeTime / lifeTime
        );

        if (leftLifeTime <= 0) { Destroy(gameObject); }

    }
}
