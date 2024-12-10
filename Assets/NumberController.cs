using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // AR ���� Ŭ���� ���
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARCore;
using Unity.Collections;

public class NumberController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject flameThrower; // FlameThrower ������Ʈ
    public GameObject lightObject; // �� ������Ʈ
    public GameObject wandObject; // Wand ������Ʈ
    public GameObject pointCloudManager; // PointCloudManager ������Ʈ
    public GameObject glowingObjectPrefab; // �Ķ��� ������ ������Ʈ ������
    public GameObject waterfallPrefab; // ���� ��ƼŬ ������

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
    [SerializeField] private ARFaceManager arFaceManager; // ARFaceManager ������Ʈ
    [SerializeField] private XROrigin xrOrigin; //ARSession ������Ʈ
    [SerializeField]private ARCameraManager arCameraManager; // ARCameraManager�� ������ ����
    private bool isTracking = false;
   
    private GameObject flowerObject;
    private NativeArray<ARCoreFaceRegionData> faceRegions;

    private int switchIndex = 0;
    private List<GameObject> glowingObjects = new List<GameObject>(); // ���� ȭ���� �Ķ��� ������Ʈ��
    private List<GameObject> waterfalls = new List<GameObject>(); // ������ ���� ��ƼŬ��
    private float checkInterval = 0.5f; // �ֱ������� ī�޶� ��ġ�� üũ�ϴ� ����
    private Transform mainCamera;
    private Coroutine generateObjectsCoroutine; // �ڷ�ƾ ��� ���� ����


    void Start()
    {
        mainCamera = Camera.main.transform; // ���� ī�޶��� Transform ����

        // �ʱ�ȭ
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        // �ʱ� ���� ����
        if (arFaceManager != null) {arFaceManager.enabled = isTracking; }
        // ��ư �̺�Ʈ ����
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
                // ���� �ø��� ���� y ���� ���� (��: 0.1��ŭ ���� �ø�)
                centerPosition.y += 0.031f;

                // prefab�� �ش� ��ġ�� �ν��Ͻ�ȭ
                if (!flowerObject || beforeIndex != switchIndex)
                {
                    // ������ ������ flowerObject�� ������ ����
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
        Debug.Log("1�� ��ư Ŭ��: �� ī�޶� ���� �� user-facing ���� ����");

        // ���� ��� ��Ȱ��ȭ
        DisableAll();

        if (arFaceManager == null) return;

        isTracking = true;

        arFaceManager.enabled = isTracking; // ARFaceManager Ȱ��ȭ

        StartCoroutine(SwitchCamera());

        // AR �� ������ Ȱ��ȭ�Ǿ��� �� UI ��ư�� ��Ȱ��ȭ
        button1.interactable = false;

        text1.SetActive(true);
    }
  
    IEnumerator SwitchCamera()
    {
        // �� �������� �ǳʶٱ� ���� ���
        yield return null;
        yield return null;

        // ī�޶��� facing direction�� World�� ����
        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
            Debug.Log("ī�޶� ������ User�� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("ARCameraManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    void OnButton2Click()
    {
        Debug.Log("2�� ��ư Ŭ��");
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
        Debug.Log("3�� ��ư Ŭ��: �Ķ��� ������Ʈ ���� Ȱ��ȭ");
        DisableAll();

        // 3�� ��ư�� ������ �Ķ��� ������Ʈ ���� �ڷ�ƾ ����
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
        Debug.Log("4�� ��ư Ŭ��: FlameThrower Ȱ��ȭ");
        DisableAll();
        if (flameThrower != null) flameThrower.SetActive(true);
        button4.interactable = false;

        text4.SetActive(true);
    }

    void DisableAll()
    {
        button1.interactable = true;
        button2.interactable = true; // ���÷� �ٸ� ��ư�� Ȱ��ȭ
        button3.interactable = true;
        button4.interactable = true;

        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);

        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
            Debug.Log("ī�޶� ������ World�� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("ARCameraManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
        isTracking = false;
        // �ʱ� ���� ����
        if (arFaceManager != null)
        {
            arFaceManager.enabled = isTracking;
        }
        if (flowerObject != null)
        {
           Destroy(flowerObject);
        }

        // ��� ���� ����
        foreach (var waterfall in waterfalls)
        {
            Destroy(waterfall);
        }
        waterfalls.Clear();

        // ��� �Ķ��� ������Ʈ ����
        foreach (var obj in glowingObjects)
        {
            Destroy(obj);
        }
        glowingObjects.Clear();

        // FlameThrower �� PointCloudManager ��Ȱ��ȭ
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        
        //if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

        // �Ķ��� ������Ʈ ���� �ڷ�ƾ ����
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
            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this); // Ŭ�� �̺�Ʈ �߰�
            glowingObjects.Add(glowingObject);

            // �ֱ������� üũ
            yield return new WaitForSeconds(checkInterval);
        }
    }*/
    IEnumerator GenerateObjectsBasedOnPointCloud()
    {
        float minimumDistance = 0.5f; // �ּ� �Ÿ� ����

        while (true)
        {
            // pointCloudManager�� ARPointCloudManager�� �����ϰ� �ִ��� Ȯ��
            if (pointCloudManager.TryGetComponent(out ARPointCloudManager pointCloudManagerComponent))
            {
                // pointCloudManager�� trackables���� �� ARPointCloud ó��
                foreach (ARPointCloud pointCloud in pointCloudManagerComponent.trackables)
                {
                    // ����Ʈ Ŭ���� ������ ��������
                    if (pointCloud.positions.HasValue)
                    {
                        var positions = new List<Vector3>();
                        // NativeArray<Vector3> �����͸� List<Vector3>�� ��ȯ
                        foreach (var position in pointCloud.positions.Value)
                        {
                            positions.Add(position);
                        }

                        // �ּ� 8���� ���� �ִ� ��� ó��
                        if (positions.Count >= 8)
                        {
                            Vector3 clusterCenter = Vector3.zero;

                            // �߽� ��ġ ���
                            foreach (var position in positions)
                            {
                                clusterCenter += position;
                            }
                            clusterCenter /= positions.Count;

                            // ���� ������Ʈ���� �Ÿ� Ȯ��
                            bool tooClose = false;
                            foreach (var obj in glowingObjects)
                            {
                                if (Vector3.Distance(obj.transform.position, clusterCenter) < minimumDistance)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            // �ʹ� ����� ��� ������ �ǳʶ�
                            if (tooClose)
                            {
                                continue;
                            }

                            // ���� ��ġ�� ������Ʈ�� ����
                            GameObject glowingObject = Instantiate(glowingObjectPrefab, clusterCenter, Quaternion.identity);
                            glowingObjects.Add(glowingObject);
                            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this); // Ŭ�� �̺�Ʈ �߰�
                        }
                    }
                }
            }

            // üũ �ֱ�
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void SpawnWaterfall(Vector3 position, GameObject glowingObject)
    {
        // ���� ����
        GameObject waterfall = Instantiate(waterfallPrefab, position, Quaternion.identity);
        waterfalls.Add(waterfall);

        // Ŭ���� �Ķ��� ������Ʈ ����
        glowingObjects.Remove(glowingObject);
        Destroy(glowingObject);
    }
}
