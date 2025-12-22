using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class PointerOnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent<bool> onPointerOn;
        
        public void OnPointerEnter(PointerEventData _)
        {
            onPointerOn.Invoke(true);
        }

        public void OnPointerExit(PointerEventData _)
        {
            onPointerOn.Invoke(false);
        }
    }
}