using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSNumberControll : MonoBehaviour
{
    public Button button1; // 1번 버튼
    public Button button2; // 2번 버튼
    public Button button3; // 3번 버튼

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 클릭 이벤트 연결
        button1.onClick.AddListener(() => OnButtonClick());
        button2.onClick.AddListener(() => OnButtonClick());
        button3.onClick.AddListener(() => OnButtonClick());

        // 씬 로드 후 초기화 작업을 처리할 수 있도록 이벤트 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 버튼 클릭 시 실행될 함수
    void OnButtonClick()
    {
        Debug.Log("버튼 클릭됨: SampleScene으로 이동");

        // SampleScene으로 이동
        SceneManager.LoadScene("SampleScene");
    }

    // 씬이 로드된 후 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            Debug.Log("SampleScene이 로드되었습니다. 초기화 작업 수행");
            // 필요한 초기화 작업을 여기서 처리
            // 예: 버튼 다시 연결, 필요한 오브젝트 상태 초기화 등
        }
    }

    // OnDisable 시 씬 이벤트 해제
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
