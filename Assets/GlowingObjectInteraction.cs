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
            // Ŭ���� ��ġ���� ���� ���� �� ������Ʈ ����
            numberController.SpawnWaterfall(transform.position, gameObject);
        }
    }
}
