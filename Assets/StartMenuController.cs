using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public GameObject panel; // ������ Panel

    public void StartGame()
    {
        if (panel != null)
        {
            panel.SetActive(false); // Panel ��Ȱ��ȭ
        }
    }
}
