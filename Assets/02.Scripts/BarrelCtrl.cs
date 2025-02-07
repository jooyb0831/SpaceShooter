using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과 파티클을 연결할 변수
    public GameObject expEffect;

    //무작위로 적용할 텍스쳐 배열
    public Texture[] textures;
    //하위에 있는 MeshRenderer 컴포넌트를 저장할 변수
    private new MeshRenderer renderer;

    //컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;

    #region 변수 및 상수 목록
    //폭발 반경
    public float radius = 10.0f;
    //총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;
    //폭발 카운트
    const int MAX_HIT = 3;
    //효과 지속시간
    const float DELETE_TIME_EFFECT = 5.0f;
    //BARREL사라질 시간
    const float DELETE_TIME_BARREL = 3.0f;
    //BARREL에 가할 힘
    const float FORCE_BARREL = 1500f;
    //BARREL 질량
    const float MASS_BARREL = 1.0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();

        //난수 발생
        int idx = Random.Range(0, textures.Length);
        //텍스쳐 지정
        renderer.material.mainTexture = textures[idx];
    }


    // 충돌 시 발생하는 콜백함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            //총알 맞은 횟수를 증가시키고 3회 이상이면 폭발 처리
            if (++hitCount == MAX_HIT)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        //폭발 효과 파티클 5초 후 제거
        Destroy(exp, DELETE_TIME_EFFECT);

        //Rigidbody 컴포넌트의 mass를 1로 수정해 무게를 가볍게 함
        //rb.mass = MASS_BARREL;
        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * FORCE_BARREL);

        //간접 폭발력 전달
        IndirectDamage(tr.position);
        
        //3초 후 드럼통 제거
        Destroy(gameObject, DELETE_TIME_BARREL);

    }
    //결과값 저장할 정적 배열 미리 선언
    [SerializeField] Collider[] colls = new Collider[10];
    //폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        //주변에 있는 드럼통을 모두 추출
        //가비지 컬렉션 발생
        // Collider[] colls = Physics.OverlapSphere(pos, radius, 1<<3);

        //가비지 컬렉션 발생하지 않음
        Physics.OverlapSphereNonAlloc(pos, radius, colls, 1<<3);

        foreach(var coll in colls)
        {
            if(coll==null)
            {
                continue;
            }
            //폭발 범위에 포함된 드럼통의 rigidbody 컴포넌트 추출
            rb = coll.GetComponent<Rigidbody>();
            //드럼통의 무게를 가볍게 함
            rb.mass = MASS_BARREL;
            //freezeRotation의 제한값 해제
            rb.constraints = RigidbodyConstraints.None;
            //폭발력을 전달
            rb.AddExplosionForce(FORCE_BARREL, pos, radius, 1200.0f);
        }
    }
}
