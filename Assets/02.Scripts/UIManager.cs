using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI를 사용하기 위한 네임스페이스
using UnityEngine.Events; // UnityEvent 사용하기 위한 네임스페이스

public class UIManager : MonoBehaviour
{
    //버튼을 연결할 변수
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    // Start is called before the first frame update
    void Start()
    {
        //UnityAction을 사용하기 위한 이벤트 연결 방식
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        //무명 메서드를 활용한 이벤트 연결 방식
        optionButton.onClick.AddListener(delegate {OnButtonClick(optionButton.name);});

        //람다식을 이용한 이벤트 연결 방식
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button : {msg}");
    }
}
