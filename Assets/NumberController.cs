using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // AR 관련 클래스 사용
public class NumberController : MonoBehaviour
{
    public GameObject flameThrower; // FlameThrower 오브젝트
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

    // AR Face 관련 변수
    private GameObject userFacingCamera; // 동적으로 생성한 User-Facing 카메라
    public GameObject arFaceManagerObject; // AR Face Manager가 포함된 GameObject
    //private ARCameraManager arCameraManager; // ARCameraManager
    private Unity.XR.CoreUtils.XROrigin xrOrigin; // XROrigin (기존 ARSessionOrigin 대신)

    private bool isARFaceTrackingActive = false; // AR Face Tracking 상태


    void Start()
    {
        mainCamera = Camera.main.transform; // 메인 카메라의 Transform 참조
        xrOrigin = GetComponent<Unity.XR.CoreUtils.XROrigin>(); // XROrigin 참조

        // 초기화
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

        // 버튼 이벤트 연결
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }
    void Update()
    {
        if (isARFaceTrackingActive)
        {
            // 얼굴 추적이 활성화된 경우에만 실행되는 코드
            // 예를 들어, AR face tracking을 기반으로 특정 애니메이션이나 효과를 실행
            Debug.Log("AR Face Tracking is active, performing specific actions.");
        }
        else
        {
            // AR face tracking이 비활성화되었을 때 다른 작업 수행
            Debug.Log("AR Face Tracking is inactive, performing default actions.");
        }
    }

    void OnButton1Click()
    {
        Debug.Log("1번 버튼 클릭: 새 카메라 생성 및 user-facing 방향 설정");

        // 기존 기능 비활성화
        DisableAll();

        // 기존 메인 카메라 비활성화
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }

        // User-Facing 카메라 생성
        if (userFacingCamera = null)
        {
            userFacingCamera = new GameObject("UserFacingCamera");
            Camera newCamera = userFacingCamera.AddComponent<Camera>();

            // 카메라 기본 설정
            newCamera.clearFlags = CameraClearFlags.Skybox;
            newCamera.fieldOfView = 60f;
            newCamera.nearClipPlane = 0.1f;
            newCamera.farClipPlane = 100f;

            // AR 환경에 맞게 user-facing 카메라로 설정
            var arCameraManager = userFacingCamera.AddComponent<ARCameraManager>();
     
        }
        // AR 얼굴 추적이 활성화되었을 때 UI 버튼을 비활성화
        button1.interactable = false;
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
        Debug.Log("4번 버튼 클릭: FlameThrower 활성화");
        DisableAll();
        if (flameThrower != null) flameThrower.SetActive(true);
    }

    void DisableAll()
    {
        button1.interactable = true;
        button2.interactable = true; // 예시로 다른 버튼을 활성화
        button3.interactable = true;
        button4.interactable = true;
        // AR Face Manager 비활성화 (AR Face Tracking을 끔)
        ARFaceManager arFaceManager = arFaceManagerObject.GetComponent<ARFaceManager>();
        if (arFaceManager != null)
        {
            arFaceManager.enabled = false; // ARFaceManager 비활성화
        }

        // 카메라 회전 초기화 (world 기준으로 돌아가게 설정)
        if (xrOrigin != null)
        {
            // 카메라를 world 기준 회전으로 설정 (Quaternion.identity)
            xrOrigin.transform.rotation = Quaternion.identity; // world 기준으로 회전 초기화
        }
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
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

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
            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this); // 클릭 이벤트 추가
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
