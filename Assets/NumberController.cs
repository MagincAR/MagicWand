using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // AR 관련 클래스 사용
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARCore;
using Unity.Collections;

public class NumberController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject flameThrower; // FlameThrower 오브젝트
    public GameObject lightObject; // 빛 오브젝트
    public GameObject wandObject; // Wand 오브젝트
    public GameObject pointCloudManager; // PointCloudManager 오브젝트
    public GameObject glowingObjectPrefab; // 파란색 빛나는 오브젝트 프리팹
    public GameObject waterfallPrefab; // 폭포 파티클 프리팹

    [Header("Materials")]
    public GameObject[] materials;

    [Header("UI Components")]
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;

    [Header("StartMenu")]
    [SerializeField] private GameObject Panel;

    [Header("AR Components")]
    [SerializeField] private ARFaceManager arFaceManager; // ARFaceManager 컴포넌트
    [SerializeField] private XROrigin xrOrigin; //ARSession 컴포넌트
    [SerializeField]private ARCameraManager arCameraManager; // ARCameraManager를 참조할 변수
    private bool isTracking = false;
   
    private GameObject flowerObject;
    private NativeArray<ARCoreFaceRegionData> faceRegions;

    private int switchIndex = 0;
    private List<GameObject> glowingObjects = new List<GameObject>(); // 현재 화면의 파란색 오브젝트들
    private List<GameObject> waterfalls = new List<GameObject>(); // 생성된 폭포 파티클들
    private float checkInterval = 0.5f; // 주기적으로 카메라 위치를 체크하는 간격
    private Transform mainCamera;
    private Coroutine generateObjectsCoroutine; // 코루틴 제어를 위한 변수


    void Start()
    {
        mainCamera = Camera.main.transform; // 메인 카메라의 Transform 참조

        // 초기화
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        // 초기 상태 설정
        if (arFaceManager != null) {arFaceManager.enabled = isTracking; }
        // 버튼 이벤트 연결
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());

    }
    private void Update()
    {
        var beforeIndex = switchIndex;
        if (isTracking==false || arFaceManager == null || materials == null || materials.Length == 0) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            switchIndex = (switchIndex + 1) % materials.Length;
        }
        ARCoreFaceSubsystem subsystem = (ARCoreFaceSubsystem)arFaceManager.subsystem;
        foreach (ARFace face in arFaceManager.trackables)
        {
            subsystem.GetRegionPoses(face.trackableId, Unity.Collections.Allocator.Persistent, ref faceRegions);
            Vector3 foreheadLeftPosition = Vector3.zero;
            Vector3 foreheadRightPosition = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            bool foundLeft = false;
            bool foundRight = false;
            foreach (ARCoreFaceRegionData faceRegion in faceRegions)
            {
                ARCoreFaceRegion regionType = faceRegion.region;
                if (regionType == ARCoreFaceRegion.NoseTip)
                {
                    rotation = faceRegion.pose.rotation;
                }
                if (regionType == ARCoreFaceRegion.ForeheadLeft)
                {
                    foreheadLeftPosition = faceRegion.pose.position;
                    foundLeft = true;
                }
                else if (regionType == ARCoreFaceRegion.ForeheadRight)
                {
                    foreheadRightPosition = faceRegion.pose.position;
                    foundRight = true;
                }
            }
            if (foundLeft && foundRight)
            {
                Vector3 centerPosition = (foreheadLeftPosition + foreheadRightPosition) / 2;
                // 위로 올리기 위해 y 값을 조정 (예: 0.1만큼 위로 올림)
                centerPosition.y += 0.031f;

                // prefab을 해당 위치에 인스턴스화
                if (!flowerObject || beforeIndex != switchIndex)
                {
                    // 이전에 생성된 flowerObject가 있으면 삭제
                    if (flowerObject != null)
                    {
                        Destroy(flowerObject);
                    }
                    flowerObject = Instantiate(materials[switchIndex], xrOrigin.TrackablesParent);
                }
                flowerObject.transform.localPosition = centerPosition;
                flowerObject.transform.localRotation = rotation;
            }
        }
    }

    void OnButton1Click()
    {
        Debug.Log("1번 버튼 클릭: 새 카메라 생성 및 user-facing 방향 설정");

        // 기존 기능 비활성화
        DisableAll();

        if (arFaceManager == null) return;

        isTracking = true;

        arFaceManager.enabled = isTracking; // ARFaceManager 활성화

        StartCoroutine(SwitchCamera());

        // AR 얼굴 추적이 활성화되었을 때 UI 버튼을 비활성화
        button1.interactable = false;

        text1.SetActive(true);
    }
  
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
        button2.interactable = false;

        text2.SetActive(true);
    }

    void OnButton3Click()
    {
        Debug.Log("3번 버튼 클릭: 파란색 오브젝트 생성 활성화");
        DisableAll();

        // 3번 버튼을 누르면 파란색 오브젝트 생성 코루틴 시작
        /*f (generateObjectsCoroutine == null)
        {
            generateObjectsCoroutine = StartCoroutine(GenerateGlowingObjectsAroundCamera());
        }
        button3.interactable = false;

        text3.SetActive(true);*/
        if(pointCloudManager != null)
        {
            pointCloudManager.SetActive(true);
            generateObjectsCoroutine=StartCoroutine(GenerateObjectsBasedOnPointCloud());

        }
        button3.interactable = false;
        text3.SetActive(true);
    }

    void OnButton4Click()
    {
        Debug.Log("4번 버튼 클릭: FlameThrower 활성화");
        DisableAll();
        if (flameThrower != null) flameThrower.SetActive(true);
        button4.interactable = false;

        text4.SetActive(true);
    }

    void DisableAll()
    {
        button1.interactable = true;
        button2.interactable = true; // 예시로 다른 버튼을 활성화
        button3.interactable = true;
        button4.interactable = true;

        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);

        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
            Debug.Log("카메라 방향을 World로 변경했습니다.");
        }
        else
        {
            Debug.LogError("ARCameraManager가 할당되지 않았습니다.");
        }
        isTracking = false;
        // 초기 상태 설정
        if (arFaceManager != null)
        {
            arFaceManager.enabled = isTracking;
        }
        if (flowerObject != null)
        {
           Destroy(flowerObject);
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
        
        //if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

        // 파란색 오브젝트 생성 코루틴 중지
        if (generateObjectsCoroutine != null)
        {
            StopCoroutine(generateObjectsCoroutine);
            generateObjectsCoroutine = null;
        }
    }
    /*
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
    }*/
    IEnumerator GenerateObjectsBasedOnPointCloud()
    {
        float minimumDistance = 0.5f; // 최소 거리 조건

        while (true)
        {
            // pointCloudManager가 ARPointCloudManager를 포함하고 있는지 확인
            if (pointCloudManager.TryGetComponent(out ARPointCloudManager pointCloudManagerComponent))
            {
                // pointCloudManager의 trackables에서 각 ARPointCloud 처리
                foreach (ARPointCloud pointCloud in pointCloudManagerComponent.trackables)
                {
                    // 포인트 클라우드 데이터 가져오기
                    if (pointCloud.positions.HasValue)
                    {
                        var positions = new List<Vector3>();
                        // NativeArray<Vector3> 데이터를 List<Vector3>로 변환
                        foreach (var position in pointCloud.positions.Value)
                        {
                            positions.Add(position);
                        }

                        // 최소 8개의 점이 있는 경우 처리
                        if (positions.Count >= 8)
                        {
                            Vector3 clusterCenter = Vector3.zero;

                            // 중심 위치 계산
                            foreach (var position in positions)
                            {
                                clusterCenter += position;
                            }
                            clusterCenter /= positions.Count;

                            // 기존 오브젝트와의 거리 확인
                            bool tooClose = false;
                            foreach (var obj in glowingObjects)
                            {
                                if (Vector3.Distance(obj.transform.position, clusterCenter) < minimumDistance)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            // 너무 가까운 경우 생성을 건너뜀
                            if (tooClose)
                            {
                                continue;
                            }

                            // 생성 위치에 오브젝트를 생성
                            GameObject glowingObject = Instantiate(glowingObjectPrefab, clusterCenter, Quaternion.identity);
                            glowingObjects.Add(glowingObject);
                            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this); // 클릭 이벤트 추가
                        }
                    }
                }
            }

            // 체크 주기
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
