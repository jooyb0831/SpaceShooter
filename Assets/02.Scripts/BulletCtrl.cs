using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public float damage = 20.0f;
    //총알의 발사 힘
    public float force = 1500.0f;

    private Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        //rigidbody 컴포넌트 추출
        rigid = GetComponent<Rigidbody>();

        //총알의 전진 방향으로의 힘(Force)을 가한다
        rigid.AddForce(transform.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
