using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01.Develop.LSW._01._Scripts.UI.MainGameScene
{
    public class HoverOutlineUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        Material mat;

        void Awake()
        {
            mat = GetComponent<Image>().material;
            mat.SetColor(OutlineColor, new Color(1,1,1,0));
        }

        public void OnPointerEnter(PointerEventData e)
        {
            mat.SetColor(OutlineColor, Color.white);
        }

        public void OnPointerExit(PointerEventData e)
        {
            mat.SetColor(OutlineColor, new Color(1,1,1,0));
        }

        public void DisableOutline()
        {
            mat.SetColor(OutlineColor, new Color(1,1,1,0));
        }
    }
}