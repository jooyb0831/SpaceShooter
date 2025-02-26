using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
#region Private
    private Transform tr;
    private Animation anim;
    
    //초기 생명 값
    private readonly float initHP = 100.0f;
#endregion

#region Public
    //현재 생명 값
    public float currHP;

    //이동속도 변수
    public float moveSpeed = 10.0f;
    //회전속도 변수
    public float turnSpeed;
    //델리게이트 선언
    public delegate void PlayerDieHandler();
    //이벤트 선언
    public static event PlayerDieHandler OnPlayerDie;

#endregion

    // Start is called before the first frame update
    IEnumerator Start() //스타트는 코루틴으로 변경가능.StartCorutine없어도 됨(Start함수라)
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        //HP 초기화
        currHP = initHP;
        //애니메이션 실행
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // -1.0f ~ 0.0f ~ +1.0f -> 연속적인 값 반환(부드럽게)
        float v = Input.GetAxis("Vertical"); //cf. GetAxisRaw = -1,0,1 세가지만 반환
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h="+h);
        //Debug.Log("v="+v);

        //Transform 컴포넌트의 position 속성값을 변경
        //tr.position += Vector3.forward * 1;


        //전후좌우 이동방향 벡터계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //Translate
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed); //포워드로 초당 10유닛만큼 이동하는.
        //Translate(이동할 방향 * time.deltaTime * 전/후진 변수 * 속도)
        //normailzed : 정규화 벡터속성->대각선으로 해도 1.

        //Vector3.up 축을 기준으로 turnSpeed 만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);


        //주인공 캐릭터 애니메이션 설정
        PlayerAnim(h, v);

    }

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }

    //충돌한 collider의 isTrigger 값이 체크되어 있을 때 발생
    void OnTriggerEnter(Collider coll)
    {
        //충돌한 collider가 몬스터의 PUNCH이면 Player의 HP 차감
        if (currHP >= 0 && coll.CompareTag("PUNCH"))
        {
            /*
            MonsterCtrl monster = coll.transform.parent.GetComponent<MonsterCtrl>();
            if(monster.state != MonsterCtrl.State.ATTACK)
            {
                return;
            }
            */
            currHP -= 10;
            Debug.Log($"Player HP : {currHP} / {initHP}");

            //Player의 생명이 0 이하이면 사망처리

            if (currHP <= 0)
            {
                PlayerDie();
            }
        }
    }

    //Player사망 처리
    void PlayerDie()
    {
        Debug.Log("Player Die!");
        /*
        //Monster 태그를 가진 모든 게임오브젝트들을 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //모든 몬스터의 OnPlayerDie함수를 순차적으로 호출
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
            //SendMessage : 첫 번째 인자로 전달한 함수명과 동일한 함수가 해당 게임 오브젝트 스크립트에 있다면 실행
            //DontRequireReceiver:호출한 함수가 없더라도 함수가 없다는 반환 안 받음.
        }
        */

        //Todo : UI에서 게임 오버 화면 추가 예정
        //UI직접 연결 말고 이벤트 호출을 통해서
        //주인공 사망 처리 이벤트 호출(발생)
        OnPlayerDie();
    }

}
