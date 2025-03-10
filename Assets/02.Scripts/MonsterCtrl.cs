using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
    private const int MAX_MONSTER_HP = 100;
    private const int HIT_MONSTER_HP = 10;
    #region Public
    //몬스터의 현재 상태
    public State state = State.IDLE;
    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 2.0f;
    //몬스터의 사망 여부
    public bool isDie = false;
    #endregion
    #region Private
    //몬스터 생명 변수
    private int hp = MAX_MONSTER_HP;
    //혈흔 효과 프리팹
    private GameObject bloodEffect;
    //몬스터가 받는 데미지

    #endregion

    #region 컴포넌트 변수
    //컴포넌트의 캐시를 처리할 변수
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;
    #endregion

    #region 애니메이션 파라미터 해시값
    //Animator 파라미터의 해시값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashyPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    #endregion



    //스크립트가 활성화 될 때마다 호출되는 함수
    void OnEnable()
    {
        //이벤트 발생 시 수행할 함수 연결
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }

    //스크립트가 비활성화 될 때마다 호출되는 함수
    void OnDisable()
    {
        //기존에 연결된 함수 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Monster의 Transform할당
        monsterTr = GetComponent<Transform>();

        //추적 대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

        //NavMeshAgent 컴포넌트 할당
        agent = GetComponent<NavMeshAgent>();

        //Animator 컴포넌트 할당
        anim = GetComponent<Animator>();

        //BloodSprayEffect프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        //추적 대상의 위치를 설정하며 바로 추적 시작
        agent.destination = playerTr.position;

        //몬스터의 상태를 체크하는 코루틴 함수 호출
        StartCoroutine(CheckMonsterState());

        //상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            //0.3초동안 중지(대기)하는 동안 제어권을 메시지 루프에 양보
            yield return new WaitForSeconds(0.3f);

            //몬스터의 상태가 DIE일 때 코루틴을 종료
            if (state == State.DIE)
            {
                yield break;
            }

            //몬스터와 주인공 캐릭터 사이의 거리 측정
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            //공격 사정거리 범위로 들어왔는지 확인
            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }

            //추적 사정거리 범위로 들어왔는지 확인
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }

            else
            {
                state = State.IDLE;
            }
        }
    }

    //몬스터의 상태에 따라 몬스터의 동작을 수행
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                //IDLE 상태
                case State.IDLE:
                    //추적 중지
                    agent.isStopped = true;
                    //Animator의 IsTrace변수를 false로 설정
                    anim.SetBool(hashTrace, false);
                    break;

                //추적 상태
                case State.TRACE:
                    //추적 대상의 좌표로 이동 시작
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    //Animator의 IsTrace를 true로 설정
                    anim.SetBool(hashTrace, true);
                    //Animator의 IsAttack변수 false
                    anim.SetBool(hashAttack, false);
                    break;

                //공격상태
                case State.ATTACK:
                    //Animator의 IsAttack변수 true
                    anim.SetBool(hashAttack, true);
                    break;

                //사망
                case State.DIE:
                    isDie = true;
                    //추적 정지
                    agent.isStopped = true;
                    //사망 애니메이션 실행
                    anim.SetTrigger(hashDie);
                    //몬스터의 collider 컴포넌트 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    //몬스터 손에 달려있는 Collider도 비활성화
                    SphereCollider[] colls = GetComponentsInChildren<SphereCollider>();

                    //UI에 표시
                    foreach (var item in colls)
                    {
                        item.enabled = false;
                    }
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            //충돌한 총알을 삭제
            Destroy(collision.gameObject);

            //피격 리액션 애니메이션 실행
            anim.SetTrigger(hashHit);

            //총알의 충돌 지점
            Vector3 pos = collision.GetContact(0).point;
            //총알의 충돌 지점의 법선 벡터
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            //혈흔 효과를 생성하는 함수 호출
            ShowBloodEffect(pos, rot);

            //몬스터의 hp 차감
            hp -= HIT_MONSTER_HP;
            if (hp <= 0)
            {
                state = State.DIE;
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.name);
    }

    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        //혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    void OnDrawGizmos()
    {
        //추적 사정거리 표시
        if (state.Equals(State.TRACE))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        //공격 사정거리 표시
        if (state.Equals(State.ATTACK))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴 함수를 모두 정지
        StopAllCoroutines();

        if(state == State.DIE)
        {
            return;
        }
        
        //추적을 정지하고 애니메이션을 수행
        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashyPlayerDie);
    }

    private TMP_Text killCnt;
    void KillCntUI()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
