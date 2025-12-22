// StarGradeData.cs
using System.Collections.Generic;
using UnityEngine;

public enum StarGrade
{
    Common,
    Rare,
    Epic
}


[System.Serializable]
public class StarGradeData
{
    public StarGrade grade;
    [Range(0f, 1f)] public float probability; // 해당 등급의 확률
    public List<GameObject> prefabs;         // 등급에 속하는 프리팹 여러 개
}
