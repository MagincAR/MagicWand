using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // AR 관련 클래스 사용
public class NumberController : MonoBehaviour
{
    public GameObject flameThrower; // FlameThrower 오브젝트
    public GameObject lightObject; // 빛 오브젝트
    public GameObject wandObject; // Wand 오브젝트
    public GameObject pointCloudManager; // PointCloudManager 오브젝트
    public GameObject glowingObjectPrefab; // 파란색 빛나는 오브젝트 프리팹
    public GameObject waterfallPrefab; // 폭포 파티클 프리팹


    [SerializeField] private ARFaceManager arFaceManager; // ARFaceManager 컴포넌트
    //[SerializeField] private Material[] materials;
    //[SerializeField] private int switchIndex = 0;


    private bool isFaceTrackingEnabled = false;          // Face Tracking 상태
    private ARCameraManager arCameraManager; // ARCameraManager를 참조할 변수


    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private List<GameObject> glowingObjects = new List<GameObject>(); // 현재 화면의 파란색 오브젝트들
    private List<GameObject> waterfalls = new List<GameObject>(); // 생성된 폭포 파티클들

    private float checkInterval = 0.5f; // 주기적으로 카메라 위치를 체크하는 간격
    private Transform mainCamera;
    private Coroutine generateObjectsCoroutine; // 코루틴 제어를 위한 변수


    void Start()
    {
        mainCamera = Camera.main.transform; // 메인 카메라의 Transform 참조
        arCameraManager = FindObjectOfType<ARCameraManager>();
        arFaceManager = FindObjectOfType<ARFaceManager>();
        

        // 초기화
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        // 초기 상태 설정
        if (arFaceManager != null)
        {
            arFaceManager.enabled = isFaceTrackingEnabled;
           // arFaceManager.facePrefab.GetComponent<MeshRenderer>().material = materials[0];

        }

        // 버튼 이벤트 연결
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());

    }


    void OnButton1Click()
    {
        Debug.Log("1번 버튼 클릭: 새 카메라 생성 및 user-facing 방향 설정");

        // 기존 기능 비활성화
        DisableAll();

        if (arFaceManager == null) return;

        isFaceTrackingEnabled = !isFaceTrackingEnabled; // 상태 변경
        arFaceManager.enabled = isFaceTrackingEnabled; // ARFaceManager 활성화/비활성화
        Debug.Log($"Face Tracking is now {(isFaceTrackingEnabled ? "Enabled" : "Disabled")}");

       // StartCoroutine(SwitchCamera());
    
        // AR 얼굴 추적이 활성화되었을 때 UI 버튼을 비활성화
        button1.interactable = false;

    }
    
    /*
     * private void FaceMaterialSwitch()
{
    Debug.Log("FaceMaterialSwitch 호출됨");

    if (arFaceManager != null)
    {
        Debug.Log($"트래킹된 얼굴 개수: {arFaceManager.trackables.count}");

        if (arFaceManager.trackables.count > 0)
        {
            // 현재 materials 배열의 길이를 로그로 출력
            Debug.Log($"materials 배열의 길이: {materials.Length}");

            // 배열의 길이에 맞게 switchIndex 조정
            switchIndex = (switchIndex + 1) % materials.Length;
            Debug.Log($"새로운 메테리얼 인덱스: {switchIndex}");

            foreach (ARFace face in arFaceManager.trackables)
            {
                if (face != null && face.GetComponent<MeshRenderer>() != null)
                {
                    Debug.Log($"현재 메테리얼: {face.GetComponent<MeshRenderer>().material.name}");

                    // 메테리얼 변경
                    face.GetComponent<MeshRenderer>().material = materials[switchIndex];

                    Debug.Log($"새 메테리얼 적용됨: {face.GetComponent<MeshRenderer>().material.name}");
                }
                else
                {
                    Debug.LogWarning("얼굴 또는 MeshRenderer가 null입니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning("트래킹된 얼굴이 없습니다.");
        }
    }
    else
    {
        Debug.LogError("ARFaceManager가 null입니다.");
    }
}


private void Update()
{
    if(isFaceTrackingEnabled == true)
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            FaceMaterialSwitch();
        }

    }
}
    */
    IEnumerator SwitchCamera()
    {
        // 두 프레임을 건너뛰기 위해 대기
        yield return null;
        yield return null;

        // 카메라의 facing direction을 World로 변경
        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
            Debug.Log("카메라 방향을 User로 변경했습니다.");
        }
        else
        {
            Debug.LogError("ARCameraManager가 할당되지 않았습니다.");
        }
    }

    void OnButton2Click()
    {
        Debug.Log("2번 버튼 클릭");
        DisableAll();

        if (lightObject != null)
        {
            lightObject.SetActive(true);
        }
       
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
        /*
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
        */
        // 카메라의 facing direction을 World로 변경
        //switchIndex = 0;
        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
            Debug.Log("카메라 방향을 World로 변경했습니다.");
        }
        else
        {
            Debug.LogError("ARCameraManager가 할당되지 않았습니다.");
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
        if (lightObject != null) lightObject.SetActive(false);
        // 초기 상태 설정
        if (arFaceManager != null)
        {
            arFaceManager.enabled = isFaceTrackingEnabled;
        }
        //if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

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
