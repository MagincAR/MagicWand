using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public GameObject panel; // 제어할 Panel

    public void StartGame()
    {
        if (panel != null)
        {
            panel.SetActive(false); // Panel 비활성화
        }
    }
}
