using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 스파크 파티클 프리팹을 연결할 변수
    public GameObject sparkEffect;

    //충돌 시작시 발생하는 이벤트
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임 오브젝트의 태그값 비교
        if(coll.gameObject.CompareTag("BULLET"))
        {
            //첫 번째 충돌 지점의 정보 추출
            ContactPoint cp = coll.GetContact(0);

            //충돌한 총알의 법선 벡터를 쿼터니언 타입으로 변환
            Quaternion rot = Quaternion.LookRotation(-cp.normal);
            //normal : 충돌 지점의 법선
            //point : 충돌 지점의 위치
            //separation : 충돌한 두 collider 의 거리
            //thisCollider : 충돌 지점의 첫 번째 Collider
            //otherCollider : 충돌 지점의 다른 collider

            //스파크 파티클을 동적으로 생성
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);

            //일정 시간이 지난 후 이펙트 삭제
            Destroy(spark, 0.5f);
            
            //충돌한 오브젝트 삭제
            Destroy(coll.gameObject);


        }
    }
}
