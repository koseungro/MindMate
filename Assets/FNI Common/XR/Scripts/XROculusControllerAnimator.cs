/// 작성자: 조효련
/// 작성일: 2022-01-28
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using System.Collections;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace FNI.XR
{
    /// <summary>
    /// 손 애니메이션 관리 클래스
    /// ActionBasedController, XROculusController와 같이 붙어있는 컴포넌트
    /// </summary>
    public class XROculusControllerAnimator : MonoBehaviour
    {
        public bool isTest = false;

        [SerializeField] private XROculusController controller;
        [SerializeField] private Animator animator;


#if UNITY_EDITOR
        private void Reset()
        {
            controller = GetComponent<XROculusController>();
            animator = GetComponentInChildren<Animator>();
        }
#endif

        private void Start()
        {
            if (isTest)
            {
                StartCoroutine(PlayTest());
            }
        }

        private void Update()
        {
            if (!isTest)
            {
                float primary = controller.PrimaryButtonValue;
                float secondary = controller.SecondaryButtonValue;
                float menu = controller.MenuButtonValue;

                float x = controller.ThumbstickValue.x;
                float y = controller.ThumbstickValue.y;
                float trigger = controller.TriggerValue;
                float grip = controller.GripValue;

                animator.SetFloat("Button 1", primary);
                animator.SetFloat("Button 2", secondary);
                animator.SetFloat("Button 3", menu);

                animator.SetFloat("Trigger", trigger);
                animator.SetFloat("Grip", grip);

                animator.SetFloat("Joy X", x);
                animator.SetFloat("Joy Y", y);
            }
        }

        private IEnumerator PlayTest()
        {
            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Joy X", value); }));
            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Joy Y", value); }));

            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Button 1", value); }));
            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Button 2", value); }));
            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Button 3", value); }));

            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Trigger", value); }));
            yield return StartCoroutine(UIToolUtil.Fade(0, 1f, (value) => { animator.SetFloat("Grip", value); }));
        }
    }
}