// StarSpawnConfigSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StarSpawnConfig", menuName = "ScriptableObjects/StarSpawnConfig")]
public class StarSpawnConfigSO : ScriptableObject
{
    public List<StarGradeData> starGrades;
}