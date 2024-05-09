/// 작성자: 조효련
/// 작성일: 2022-02-15
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.XR;
#if UNITY_EDITOR
#endif
using UnityEngine;
using UnityEngine.EventSystems;

namespace FNI
{
    /// <summary>
    /// UI VR 버튼 공통 이펙트
    /// 
    /// </summary>
    public class UIXRButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private XRManager manager;

        private void Reset()
        {
            manager = FindObjectOfType<XRManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager?.PlayClickEffect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            manager?.PlayHoverEffect();
        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
    }
}