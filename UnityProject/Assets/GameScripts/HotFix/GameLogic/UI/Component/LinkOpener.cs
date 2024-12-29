using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
namespace HsHall.Script.UI.Component
{
    [RequireComponent(typeof(TMP_Text))]
    public class LinkOpener : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, Camera.main);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}

