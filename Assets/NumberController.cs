using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽� �߰�
using System.Collections.Generic;
using System.Collections;

public class NumberController : MonoBehaviour
{
    public GameObject flameThrowerPrefab; // FlameThrower ������
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
    private GameObject currentFlameThrower; // ���� ������ FlameThrower
    private Vector3 flamePosition; // FlameThrower�� ��ġ (������ ��)

    private float circleRadius = 1f; // ���� ������
    private int circleSegments = 30; // ���� ���׸�Ʈ ���� (���е� ����)

    void Start()
    {
        mainCamera = Camera.main.transform; // ���� ī�޶��� Transform ����

        // �ʱ�ȭ
        if (pointCloudManager != null) pointCloudManager.SetActive(false);

        // ��ư �̺�Ʈ ����
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }

    void OnButton1Click()
    {
        Debug.Log("1�� ��ư Ŭ��");
        DisableAll();
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
        Debug.Log("4�� ��ư Ŭ��: test ������ �̵�");
        DisableAll();

        // test ������ ��ȯ
        SceneManager.LoadScene("test");
    }

    
    // �� ȿ���� �����ϴ� �޼���
    private void SpawnFireEffect(Vector3 position)
    {
        // �� ȿ�� �������� �ش� ��ġ�� �ν��Ͻ�ȭ
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
        if (currentFlameThrower != null)
        {
            Destroy(currentFlameThrower);
            currentFlameThrower = null;
        }

        if (pointCloudManager != null) pointCloudManager.SetActive(false);

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

            // GlowingObjectInteraction�� �ʱ�ȭ�� �� NumberController ����
            glowingObject.AddComponent<GlowingObjectInteraction>().Initialize(this);
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
