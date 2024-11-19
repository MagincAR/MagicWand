using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberController : MonoBehaviour
{
    public GameObject flameThrower; // FlameThrower ������Ʈ
    public GameObject wandObject; // Wand ������Ʈ�� ����

    public Button button1; // 1�� ��ư
    public Button button2; // 2�� ��ư
    public Button button3; // 3�� ��ư
    public Button button4; // 4�� ��ư

    void Start()
    {
        // FlameThrower �ʱ�ȭ (��Ȱ��ȭ)
        if (flameThrower != null)
        {
            flameThrower.SetActive(false); // ���� �� FlameThrower ��Ȱ��ȭ
        }
        else
        {
            Debug.LogError("FlameThrower�� ������� �ʾҽ��ϴ�!");
        }

        // �� ��ư�� Ŭ�� �̺�Ʈ ����
        button1.onClick.AddListener(() => OnButton1Click());
        button2.onClick.AddListener(() => OnButton2Click());
        button3.onClick.AddListener(() => OnButton3Click());
        button4.onClick.AddListener(() => OnButton4Click());
    }

    void Update()
    {
        // FlameThrower Ȱ��ȭ ���¿��� ���콺 Ŭ�� ����
        if (flameThrower != null && flameThrower.activeSelf && Input.GetMouseButton(0))
        {
            // ȭ���� ��ġ(�Ǵ� ���콺 Ŭ��) ��ǥ ��������
            var mousePos = Input.mousePosition;

            // Z �� ���� ī�޶���� �Ÿ� ����
            mousePos.z = Camera.main.nearClipPlane + 1.0f; // ������ Z �� ����

            // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // Wand�� ���콺 ��ġ�� �ٶ󺸰� ����
            wandObject.transform.LookAt(worldPos);
        }
    }

    void OnButton1Click()
    {
        Debug.Log("1�� ��ư Ŭ��: ��� ���� �ʿ�");
    }

    void OnButton2Click()
    {
        Debug.Log("2�� ��ư Ŭ��: ��� ���� �ʿ�");
    }

    void OnButton3Click()
    {
        Debug.Log("3�� ��ư Ŭ��: ��� ���� �ʿ�");
    }

    void OnButton4Click()
    {
        Debug.Log("4�� ��ư Ŭ��: FlameThrower Ȱ��ȭ");

        // FlameThrower Ȱ��ȭ
        if (flameThrower != null)
        {
            flameThrower.SetActive(true);
            Debug.Log("FlameThrower�� Ȱ��ȭ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("FlameThrower�� ������� �ʾҽ��ϴ�!");
        }
    }
}
