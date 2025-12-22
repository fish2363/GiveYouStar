using System;
using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class MoveUI : MonoBehaviour
    {
        public Vector2 hidePos;
        
        public void Show()
        {
            transform.DOMove(Vector3.zero, 0.5f);
        }

        public void Hide()
        {
            transform.DOMove(hidePos, 0.5f);
        }
    }
}