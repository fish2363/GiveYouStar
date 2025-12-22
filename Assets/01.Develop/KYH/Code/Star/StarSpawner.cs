using UnityEngine;
using System.Collections.Generic;

public class StarSpawner : MonoBehaviour
{
    public Transform bottomLeft;
    public Transform topRight;
    public StarSpawnConfigSO spawnConfig;

    private void Start()
    {
        SpawnStars(30);
    }

    public void SpawnStars(int totalCount)
    {
        if (spawnConfig == null || spawnConfig.starGrades.Count == 0)
        {
            Debug.LogWarning("Spawn config missing or empty.");
            return;
        }

        for (int i = 0; i < totalCount; i++)
        {
            StarGradeData gradeData = GetRandomGrade();
            if (gradeData == null || gradeData.prefabs.Count == 0) continue;

            GameObject prefabToSpawn = gradeData.prefabs[Random.Range(0, gradeData.prefabs.Count)];
            Vector3 spawnPos = GetRandomPositionInArea();
            GameObject star = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            Vector3 chosenDir = Random.value < 0.5f
                ? new Vector3(-1f, 1f, 0f)   // 왼쪽 위
                : new Vector3(1f, -1f, 0f);  // 오른쪽 아래

            StarMover mover = star.GetComponent<StarMover>();
            if (mover != null)
            {
                mover.SetMoveDirection(chosenDir);
            }
        }
    }

    private StarGradeData GetRandomGrade()
    {
        float rand = Random.value; // 0.0 ~ 1.0
        float cumulative = 0f;

        foreach (var gradeData in spawnConfig.starGrades)
        {
            cumulative += gradeData.probability;
            if (rand <= cumulative)
                return gradeData;
        }

        // 확률 누적합이 1보다 작을 경우 대비
        return spawnConfig.starGrades[spawnConfig.starGrades.Count - 1];
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(bottomLeft.position.x, topRight.position.x);
        float y = Random.Range(bottomLeft.position.y, topRight.position.y);
        float z = Random.Range(bottomLeft.position.z, topRight.position.z);
        return new Vector3(x, y, z);
    }
}