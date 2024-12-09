using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private GameObject fireEffectPrefab; // 불 효과 프리팹
    [SerializeField] private float circleRadius = 1f; // 원의 반지름
    [SerializeField] private int circleSegments = 30; // 원의 세그먼트 개수 (세밀도 조절)

    private void Update()
    {
        // 마우스 클릭 시 원을 그리고 불 효과를 생성
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치를 화면에서 월드 좌표로 변환
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            // 원을 그리는 메서드 호출
            DrawCircle(spawnPosition);

            // 불 효과를 해당 위치에 생성
            SpawnFireEffect(spawnPosition);
        }
    }

    // 원을 그리는 메서드
    private void DrawCircle(Vector3 position)
    {
        GameObject circle = new GameObject("Circle");

        // LineRenderer로 원 그리기
        LineRenderer lineRenderer = circle.AddComponent<LineRenderer>();
        lineRenderer.positionCount = circleSegments + 1; // 원의 세그먼트 개수
        lineRenderer.useWorldSpace = false; // 월드 좌표 사용 안함

        float angleStep = 360f / circleSegments;

        for (int i = 0; i < circleSegments + 1; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * circleRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * circleRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }

        // 원의 위치를 마우스 클릭 위치로 설정
        circle.transform.position = position;
    }

    // 불 효과를 생성하는 메서드
    private void SpawnFireEffect(Vector3 position)
    {
        if (fireEffectPrefab != null)
        {
            // 불 효과 프리팹을 해당 위치에 인스턴스화
            GameObject fireEffect = Instantiate(fireEffectPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("FireEffect prefab is not assigned!");
        }
    }
}
