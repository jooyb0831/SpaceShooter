using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;
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

        Debug.Log("h="+h);
        Debug.Log("v="+v);

        //Transform 컴포넌트의 position 속성값을 변경
        tr.position += Vector3.forward * 1;

        //Translate
        tr.Translate(Vector3.forward * 1.0f);
    }
}
