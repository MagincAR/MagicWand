using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSNumberControll : MonoBehaviour
{
    public Button button1; // 1�� ��ư
    public Button button2; // 2�� ��ư
    public Button button3; // 3�� ��ư

    // Start is called before the first frame update
    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ ����
        button1.onClick.AddListener(() => OnButtonClick());
        button2.onClick.AddListener(() => OnButtonClick());
        button3.onClick.AddListener(() => OnButtonClick());

        // �� �ε� �� �ʱ�ȭ �۾��� ó���� �� �ֵ��� �̺�Ʈ �߰�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ��ư Ŭ�� �� ����� �Լ�
    void OnButtonClick()
    {
        Debug.Log("��ư Ŭ����: SampleScene���� �̵�");

        // SampleScene���� �̵�
        SceneManager.LoadScene("SampleScene");
    }

    // ���� �ε�� �� ȣ��Ǵ� �Լ�
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            Debug.Log("SampleScene�� �ε�Ǿ����ϴ�. �ʱ�ȭ �۾� ����");
            // �ʿ��� �ʱ�ȭ �۾��� ���⼭ ó��
            // ��: ��ư �ٽ� ����, �ʿ��� ������Ʈ ���� �ʱ�ȭ ��
        }
    }

    // OnDisable �� �� �̺�Ʈ ����
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
