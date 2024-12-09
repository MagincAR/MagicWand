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

    [SerializeField] private ARFaceManager arFaceManager; // ARFaceManager ������Ʈ
    [SerializeField] private XROrigin xrOrigin; //ARSession ������Ʈ

    private bool isFaceTrackingEnabled = false;          // Face Tracking ����
    private ARCameraManager arCameraManager; // ARCameraManager�� ������ ����
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
        arCameraManager = FindObjectOfType<ARCameraManager>();


        // �ʱ�ȭ
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        // �ʱ� ���� ����
        if (arFaceManager != null) {arFaceManager.enabled = isFaceTrackingEnabled;}
        // ��ư �̺�Ʈ ����
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());

    }
    private void Update()
    {
        var beforeIndex = switchIndex; 
        if (arFaceManager == null || materials == null || materials.Length == 0) return;

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

        isFaceTrackingEnabled = !isFaceTrackingEnabled; // ���� ����
        arFaceManager.enabled = isFaceTrackingEnabled; // ARFaceManager Ȱ��ȭ/��Ȱ��ȭ
        Debug.Log($"Face Tracking is now {(isFaceTrackingEnabled ? "Enabled" : "Disabled")}");

        StartCoroutine(SwitchCamera());
    
        // AR �� ������ Ȱ��ȭ�Ǿ��� �� UI ��ư�� ��Ȱ��ȭ
        button1.interactable = false;

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
       
    }

    void OnButton3Click()
    {
        Debug.Log("3�� ��ư Ŭ��: �Ķ��� ������Ʈ ���� Ȱ��ȭ");
        DisableAll();

        // 3�� ��ư�� ������ �Ķ��� ������Ʈ ���� �ڷ�ƾ ����
        if (generateObjectsCoroutine == null)
        {
            generateObjectsCoroutine = StartCoroutine(GenerateGlowingObjectsAroundCamera());
        }
    }

    void OnButton4Click()
    {
        Debug.Log("4�� ��ư Ŭ��: FlameThrower Ȱ��ȭ");
        DisableAll();
        if (flameThrower != null) flameThrower.SetActive(true);
    }

    void DisableAll()
    {
        button1.interactable = true;
        button2.interactable = true; // ���÷� �ٸ� ��ư�� Ȱ��ȭ
        button3.interactable = true;
        button4.interactable = true;
        // AR Face Manager ��Ȱ��ȭ (AR Face Tracking�� ��)
        /*
        ARFaceManager arFaceManager = arFaceManagerObject.GetComponent<ARFaceManager>();
        if (arFaceManager != null)
        {
            arFaceManager.enabled = false; // ARFaceManager ��Ȱ��ȭ
        }
        
        // ī�޶� ȸ�� �ʱ�ȭ (world �������� ���ư��� ����)
        if (xrOrigin != null)
        {
            // ī�޶� world ���� ȸ������ ���� (Quaternion.identity)
            xrOrigin.transform.rotation = Quaternion.identity; // world �������� ȸ�� �ʱ�ȭ
        }
        */
        // ī�޶��� facing direction�� World�� ����
        //switchIndex = 0;
        if (arCameraManager != null)
        {
            arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
            Debug.Log("ī�޶� ������ World�� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("ARCameraManager�� �Ҵ���� �ʾҽ��ϴ�.");
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
        // �ʱ� ���� ����
        if (arFaceManager != null)
        {
            arFaceManager.enabled = isFaceTrackingEnabled;
        }
        //if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

        // �Ķ��� ������Ʈ ���� �ڷ�ƾ ����
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
            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this); // Ŭ�� �̺�Ʈ �߰�
            glowingObjects.Add(glowingObject);

            // �ֱ������� üũ
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
