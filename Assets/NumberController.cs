using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // AR ���� Ŭ���� ���
public class NumberController : MonoBehaviour
{
    public GameObject flameThrower; // FlameThrower ������Ʈ
    public GameObject wandObject; // Wand ������Ʈ
    public GameObject pointCloudManager; // PointCloudManager ������Ʈ
    public GameObject glowingObjectPrefab; // �Ķ��� ������ ������Ʈ ������
    public GameObject waterfallPrefab; // ���� ��ƼŬ ������



    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private List<GameObject> glowingObjects = new List<GameObject>(); // ���� ȭ���� �Ķ��� ������Ʈ��
    private List<GameObject> waterfalls = new List<GameObject>(); // ������ ���� ��ƼŬ��

    private float checkInterval = 0.5f; // �ֱ������� ī�޶� ��ġ�� üũ�ϴ� ����
    private Transform mainCamera;
    private Coroutine generateObjectsCoroutine; // �ڷ�ƾ ��� ���� ����

    // AR Face ���� ����
    private GameObject userFacingCamera; // �������� ������ User-Facing ī�޶�
    public GameObject arFaceManagerObject; // AR Face Manager�� ���Ե� GameObject
    //private ARCameraManager arCameraManager; // ARCameraManager
    private Unity.XR.CoreUtils.XROrigin xrOrigin; // XROrigin (���� ARSessionOrigin ���)

    private bool isARFaceTrackingActive = false; // AR Face Tracking ����


    void Start()
    {
        mainCamera = Camera.main.transform; // ���� ī�޶��� Transform ����
        xrOrigin = GetComponent<Unity.XR.CoreUtils.XROrigin>(); // XROrigin ����

        // �ʱ�ȭ
        if (flameThrower != null) flameThrower.SetActive(false);
        if (pointCloudManager != null) pointCloudManager.SetActive(false);
        if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

        // ��ư �̺�Ʈ ����
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }
    void Update()
    {
        if (isARFaceTrackingActive)
        {
            // �� ������ Ȱ��ȭ�� ��쿡�� ����Ǵ� �ڵ�
            // ���� ���, AR face tracking�� ������� Ư�� �ִϸ��̼��̳� ȿ���� ����
            Debug.Log("AR Face Tracking is active, performing specific actions.");
        }
        else
        {
            // AR face tracking�� ��Ȱ��ȭ�Ǿ��� �� �ٸ� �۾� ����
            Debug.Log("AR Face Tracking is inactive, performing default actions.");
        }
    }

    void OnButton1Click()
    {
        Debug.Log("1�� ��ư Ŭ��: �� ī�޶� ���� �� user-facing ���� ����");

        // ���� ��� ��Ȱ��ȭ
        DisableAll();

        // ���� ���� ī�޶� ��Ȱ��ȭ
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }

        // User-Facing ī�޶� ����
        if (userFacingCamera = null)
        {
            userFacingCamera = new GameObject("UserFacingCamera");
            Camera newCamera = userFacingCamera.AddComponent<Camera>();

            // ī�޶� �⺻ ����
            newCamera.clearFlags = CameraClearFlags.Skybox;
            newCamera.fieldOfView = 60f;
            newCamera.nearClipPlane = 0.1f;
            newCamera.farClipPlane = 100f;

            // AR ȯ�濡 �°� user-facing ī�޶�� ����
            var arCameraManager = userFacingCamera.AddComponent<ARCameraManager>();
     
        }
        // AR �� ������ Ȱ��ȭ�Ǿ��� �� UI ��ư�� ��Ȱ��ȭ
        button1.interactable = false;
    }

    void OnButton2Click()
    {
        Debug.Log("2�� ��ư Ŭ��");
        DisableAll();
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
        if (arFaceManagerObject != null) arFaceManagerObject.SetActive(false);

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
