using UnityEngine;

public class GlowingObjectInteraction : MonoBehaviour
{
    private NumberController numberController;

    public void Initialize(NumberController controller)
    {
        numberController = controller;
    }

    private void OnMouseDown()
    {
        if (numberController != null)
        {
            // 클릭된 위치에서 폭포 생성 및 오브젝트 제거
            numberController.SpawnWaterfall(transform.position, gameObject);
        }
    }
}
