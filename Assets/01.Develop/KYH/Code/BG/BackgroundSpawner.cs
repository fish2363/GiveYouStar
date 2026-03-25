using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;

    [Header("로컬 기준 시작 위치 (부모 기준)")]
    public Vector2 localStartPosition = new Vector2(12.9f, 3.3f);

    [Header("생성 범위")]
    public int tileXCount = 6; // 오른쪽으로 몇 칸
    public int tileYCount = 4; // 위쪽으로 몇 칸

    [Header("축별 간격")]
    public float xSpacing = 10f;
    public float ySpacing = 10f;

    public void SpawnBackgroundTiles()
    {
        for (int y = 0; y < tileYCount; y++)
        {
            for (int x = 0; x < tileXCount; x++)
            {
                Vector2 localPos = new Vector2(
                    localStartPosition.x + x * xSpacing,
                    localStartPosition.y + y * ySpacing
                );

                GameObject tile = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, transform);
                tile.transform.localPosition = localPos;
            }
        }
    }

    private void Start()
    {
        SpawnBackgroundTiles();
    }
}
