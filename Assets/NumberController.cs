using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스 추가
using System.Collections.Generic;
using System.Collections;

public class NumberController : MonoBehaviour
{
    public GameObject flameThrowerPrefab; // FlameThrower 프리팹
    public GameObject wandObject; // Wand 오브젝트
    public GameObject pointCloudManager; // PointCloudManager 오브젝트
    public GameObject glowingObjectPrefab; // 파란색 빛나는 오브젝트 프리팹
    public GameObject waterfallPrefab; // 폭포 파티클 프리팹

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private List<GameObject> glowingObjects = new List<GameObject>(); // 현재 화면의 파란색 오브젝트들
    private List<GameObject> waterfalls = new List<GameObject>(); // 생성된 폭포 파티클들

    private float checkInterval = 0.5f; // 주기적으로 카메라 위치를 체크하는 간격
    private Transform mainCamera;
    private Coroutine generateObjectsCoroutine; // 코루틴 제어를 위한 변수
    private GameObject currentFlameThrower; // 현재 생성된 FlameThrower
    private Vector3 flamePosition; // FlameThrower의 위치 (지팡이 끝)

    private float circleRadius = 1f; // 원의 반지름
    private int circleSegments = 30; // 원의 세그먼트 개수 (세밀도 조절)

    void Start()
    {
        mainCamera = Camera.main.transform; // 메인 카메라의 Transform 참조

        // 초기화
        if (pointCloudManager != null) pointCloudManager.SetActive(false);

        // 버튼 이벤트 연결
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }

    void OnButton1Click()
    {
        Debug.Log("1번 버튼 클릭");
        DisableAll();
    }

    void OnButton2Click()
    {
        Debug.Log("2번 버튼 클릭");
        DisableAll();
    }

    void OnButton3Click()
    {
        Debug.Log("3번 버튼 클릭: 파란색 오브젝트 생성 활성화");
        DisableAll();

        // 3번 버튼을 누르면 파란색 오브젝트 생성 코루틴 시작
        if (generateObjectsCoroutine == null)
        {
            generateObjectsCoroutine = StartCoroutine(GenerateGlowingObjectsAroundCamera());
        }
    }

    void OnButton4Click()
    {
        Debug.Log("4번 버튼 클릭: test 씬으로 이동");
        DisableAll();

        // test 씬으로 전환
        SceneManager.LoadScene("test");
    }

    
    // 불 효과를 생성하는 메서드
    private void SpawnFireEffect(Vector3 position)
    {
        // 불 효과 프리팹을 해당 위치에 인스턴스화
        if (flameThrowerPrefab != null)
        {
            GameObject fireEffect = Instantiate(flameThrowerPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("FireEffect prefab is not assigned!");
        }
    }


    void DisableAll()
    {
        // 모든 폭포 제거
        foreach (var waterfall in waterfalls)
        {
            Destroy(waterfall);
        }
        waterfalls.Clear();

        // 모든 파란색 오브젝트 제거
        foreach (var obj in glowingObjects)
        {
            Destroy(obj);
        }
        glowingObjects.Clear();

        // FlameThrower 및 PointCloudManager 비활성화
        if (currentFlameThrower != null)
        {
            Destroy(currentFlameThrower);
            currentFlameThrower = null;
        }

        if (pointCloudManager != null) pointCloudManager.SetActive(false);

        // 파란색 오브젝트 생성 코루틴 중지
        if (generateObjectsCoroutine != null)
        {
            StopCoroutine(generateObjectsCoroutine);
            generateObjectsCoroutine = null;
        }
    }

    IEnumerator GenerateGlowingObjectsAroundCamera()
    {
        while (true)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 1.5f), Random.Range(1.5f, 3f));
            Vector3 spawnPosition = mainCamera.position + mainCamera.forward * randomOffset.z + mainCamera.right * randomOffset.x + mainCamera.up * randomOffset.y;

            GameObject glowingObject = Instantiate(glowingObjectPrefab, spawnPosition, Quaternion.identity);

            // GlowingObjectInteraction을 초기화할 때 NumberController 전달
            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this);
            glowingObjects.Add(glowingObject);

            // 주기적으로 체크
            yield return new WaitForSeconds(checkInterval);
        }
    }
    public void SpawnWaterfall(Vector3 position, GameObject glowingObject)
    {
        // 폭포 생성
        GameObject waterfall = Instantiate(waterfallPrefab, position, Quaternion.identity);
        waterfalls.Add(waterfall);

        // 클릭된 파란색 오브젝트 제거
        glowingObjects.Remove(glowingObject);
        Destroy(glowingObject);
    }
}
