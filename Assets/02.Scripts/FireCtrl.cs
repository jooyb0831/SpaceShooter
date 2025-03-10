using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//반드시 필요한 컴포넌트를 명시해해당 컴포넌트가 삭제되는 것을 방지하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{

    //총알 프리팹
    public GameObject bullet;

    //총알 발사 좌표
    public Transform firePos;

    //총소리에 사용할 오디오 음원
    public AudioClip fireSfx;

    //AUudidoSource를의 컴포넌트를 저장할 변수
    private new AudioSource audio;

    private MeshRenderer muzzleFlash;

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private bool isPlayerDie;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
        isPlayerDie = false;
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 눌렀을 때 Fire함수 호출
        if(Input.GetMouseButtonDown(0))
        {
            if(isPlayerDie) return;
            Fire();
        }

    }

    public void OnPlayerDie()
    {
        isPlayerDie = true;
    }

    void Fire()
    {
        //Bullet 프리팹을 동적으로 생성(생성할 객체, 위치, 회전)
        Instantiate(bullet, firePos.position, firePos.rotation);
        audio.PlayOneShot(fireSfx, 1.0f); //PlayOneSht : 한 번만재생.
        //총구 화염 효과 코루틴 함수호출
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        //오프셋 좌표값을 랜덤 함수로 생성 intRandom.Range(포함,불포함) floatRandom.Range(포함, 포함)
        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2)) * 0.5f;
        //텍스처의 오프셋 값 설정
        muzzleFlash.material.mainTextureOffset = offset;

        //MuzzleFlash의 회전 반경
        float angle = Random.Range(0,360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0,0,angle);

        //MuzzleFlash 크기 조절
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;
        
        //muzzleflahs 활성화
        muzzleFlash.enabled = true;
    
        //0.2초동안 대기(정지)하는 동안 메시지 루프로 제어권 양보
        yield return new WaitForSeconds(0.2f);
        
        //비활성화
        muzzleFlash.enabled = false;
    }
}
