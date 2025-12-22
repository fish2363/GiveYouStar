using System;
using DG.Tweening;
using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class MoveUI : MonoBehaviour
    {
        public Transform hideTrm;
        public Transform showTrm;

        public void Show()
        {
            transform.DOMove(showTrm.position, 0.5f);
        }

        public void Hide()
        {
            transform.DOMove(hideTrm.position, 0.5f);
        }
    }
}