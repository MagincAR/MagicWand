using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberController : MonoBehaviour
{
    public GameObject flameThrower; // FlameThrower 오브젝트
    public GameObject wandObject; // Wand 오브젝트를 연결

    public Button button1; // 1번 버튼
    public Button button2; // 2번 버튼
    public Button button3; // 3번 버튼
    public Button button4; // 4번 버튼

    void Start()
    {
        // FlameThrower 초기화 (비활성화)
        if (flameThrower != null)
        {
            flameThrower.SetActive(false); // 시작 시 FlameThrower 비활성화
        }
        else
        {
            Debug.LogError("FlameThrower가 연결되지 않았습니다!");
        }

        // 각 버튼에 클릭 이벤트 연결
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }

    void Update()
    {
        // FlameThrower 활성화 상태에서 마우스 클릭 감지
        if (flameThrower != null && flameThrower.activeSelf && Input.GetMouseButton(0))
        {
            // 화면의 터치(또는 마우스 클릭) 좌표 가져오기
            var mousePos = Input.mousePosition;

            // Z 축 기준 카메라와의 거리 설정
            mousePos.z = Camera.main.nearClipPlane + 1.0f; // 적절한 Z 값 설정

            // 화면 좌표를 월드 좌표로 변환
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // Wand가 마우스 위치를 바라보게 설정
            wandObject.transform.LookAt(worldPos);
        }
    }

    void OnButton1Click()
    {
        Debug.Log("1번 버튼 클릭: 기능 구현 필요");
    }

    void OnButton2Click()
    {
        Debug.Log("2번 버튼 클릭: 기능 구현 필요");
    }

    void OnButton3Click()
    {
        Debug.Log("3번 버튼 클릭: 기능 구현 필요");
    }

    void OnButton4Click()
    {
        Debug.Log("4번 버튼 클릭: FlameThrower 활성화");

        // FlameThrower 활성화
        if (flameThrower != null)
        {
            flameThrower.SetActive(true);
            Debug.Log("FlameThrower가 활성화되었습니다.");
        }
        else
        {
            Debug.LogError("FlameThrower가 연결되지 않았습니다!");
        }
    }
}
