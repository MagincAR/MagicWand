using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWand : MonoBehaviour
{
    public GameObject wandObject; // Wand 오브젝트를 연결

    void Update()
    {
        if (Input.GetMouseButton(0)) // 마우스 클릭 또는 터치
        {
            // 화면의 터치(또는 마우스 클릭) 좌표 가져오기
            var mousePos = Input.mousePosition;

            // Z 축 기준 카메라와의 거리 설정
            mousePos.z = Camera.main.nearClipPlane + 1.0f; // 적절한 Z 값 설정

            // 화면 좌표를 월드 좌표로 변환
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            wandObject.transform.LookAt(worldPos);
        }
    }
}
