using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;

    //이동속도 변수
    public float moveSpeed = 10.0f;

    //회전속도 변수
    public float turnSpeed = 80.0f;

    // Start is called before the first frame update
    void Start()
    {   
        tr = GetComponent<Transform>();
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
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right*h);
        //Translate
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed); //포워드로 초당 10유닛만큼 이동하는.
        //Translate(이동할 방향 * time.deltaTime * 전/후진 변수 * 속도)
        //normailzed : 정규화백터속성->대각선으로 해도1.

        //Vector3.up 축을 기준으로 turnSpeed 만크의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

    }
}
