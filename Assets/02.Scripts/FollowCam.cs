using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //따라가야 할 대상을 연결할 변수
    public Transform targetTr;

    //MainCamera 자신의 transform 컴포넌트
    private Transform camTr;

    //따라갈 대상으로부터 떨어질 거리
    [Range(2.0f, 20.0f)] //슬라이드 바처럼 인스펙터에 나타나며 범위 제한(min, max).
    public float distance = 10.0f;

    //Y축으로 이동할 높이
    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    //반응 속도
    public float damping = 10f;

    //카메라 LookAt의 Offset값
    public float targetOffset = 2.0f;
    
    //Smoothdamp에서 사용할 변수
    private Vector3 velocity = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        //MainCamera 자신의 Transform 컴포넌트 추출
        camTr = GetComponent<Transform>();
    }

    void LateUpdate() //모든 업데이트가 실행된 후 실행되는 Update.일반 Update하면 카메라 떨림 발생 가능.
    {
        //추적해야 할 대상의 뒤쪽으로 distance만큼 이동
        //높이를 height만큼 이동
        Vector3 pos = targetTr.position 
                        +(-targetTr.forward * distance)
                        +(Vector3.up * height);
        
        //구면 선형 보간 함수를 사용해 부드럽게 위치를 변경
        //camTr.position = Vector3.Slerp(camTr.position, pos, Time.deltaTime * damping); //시작위치, 목표위치, 시간

        //SmoothDamp활용하여 부드럽게 변경
        camTr.position = Vector3.SmoothDamp(camTr.position, // 시작위치
                                            pos,            //목표위치  
                                            ref velocity,   //현재속도
                                            damping);       //도달시간
                                            
        //Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
