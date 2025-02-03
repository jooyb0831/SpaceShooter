using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{

    //충돌 시작시 발생하는 이벤트
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임 오브젝트의 태그값 비교
        if(coll.gameObject.CompareTag("BULLET"))
        {
            Destroy(coll.gameObject);
        }
    }
}
