using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWand : MonoBehaviour
{
    public GameObject wandObject; // Wand ������Ʈ�� ����

    void Update()
    {
        if (Input.GetMouseButton(0)) // ���콺 Ŭ�� �Ǵ� ��ġ
        {
            // ȭ���� ��ġ(�Ǵ� ���콺 Ŭ��) ��ǥ ��������
            var mousePos = Input.mousePosition;

            // Z �� ���� ī�޶���� �Ÿ� ����
            mousePos.z = Camera.main.nearClipPlane + 1.0f; // ������ Z �� ����

            // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            wandObject.transform.LookAt(worldPos);
        }
    }
}
