using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private GameObject fireEffectPrefab; // �� ȿ�� ������
    [SerializeField] private float circleRadius = 1f; // ���� ������
    [SerializeField] private int circleSegments = 30; // ���� ���׸�Ʈ ���� (���е� ����)

    private void Update()
    {
        // ���콺 Ŭ�� �� ���� �׸��� �� ȿ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ�� ȭ�鿡�� ���� ��ǥ�� ��ȯ
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            // ���� �׸��� �޼��� ȣ��
            DrawCircle(spawnPosition);

            // �� ȿ���� �ش� ��ġ�� ����
            SpawnFireEffect(spawnPosition);
        }
    }

    // ���� �׸��� �޼���
    private void DrawCircle(Vector3 position)
    {
        GameObject circle = new GameObject("Circle");

        // LineRenderer�� �� �׸���
        LineRenderer lineRenderer = circle.AddComponent<LineRenderer>();
        lineRenderer.positionCount = circleSegments + 1; // ���� ���׸�Ʈ ����
        lineRenderer.useWorldSpace = false; // ���� ��ǥ ��� ����

        float angleStep = 360f / circleSegments;

        for (int i = 0; i < circleSegments + 1; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * circleRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * circleRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }

        // ���� ��ġ�� ���콺 Ŭ�� ��ġ�� ����
        circle.transform.position = position;
    }

    // �� ȿ���� �����ϴ� �޼���
    private void SpawnFireEffect(Vector3 position)
    {
        if (fireEffectPrefab != null)
        {
            // �� ȿ�� �������� �ش� ��ġ�� �ν��Ͻ�ȭ
            GameObject fireEffect = Instantiate(fireEffectPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("FireEffect prefab is not assigned!");
        }
    }
}
