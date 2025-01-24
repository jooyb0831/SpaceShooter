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
        camTr.position = targetTr.position 
                        +(-targetTr.forward * distance)
                        +(Vector3.up * height);
        
        //Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
