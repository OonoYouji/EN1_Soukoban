using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Particle : MonoBehaviour {



    /// ========================================
    /// �� �ϐ��̐錾
    /// ========================================

    ///- ���ł���܂ł̎���
    private float lifeTime;
    ///- ���ł���܂ł̎c�莞��
    private float leftLifeTime;
    ///- �ړ���
    private Vector3 velocity;
    ///- ����scale
    private Vector3 defaultScale;




    /// ========================================
    /// �� ������
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
    /// �� �X�V����
    /// ========================================
    void Update() {


        ///- �c�莞�Ԃ̌���
        leftLifeTime -= Time.deltaTime;

        ///- ���g�̍��W�ړ�
        transform.position += velocity * Time.deltaTime;

        ///- �c�莞�Ԃɂ��Scale������������
        transform.localScale = Vector3.Lerp(
            new Vector3(0, 0, 0),
            defaultScale,
            leftLifeTime / lifeTime
        );

        if (leftLifeTime <= 0) { Destroy(gameObject); }

    }
}
