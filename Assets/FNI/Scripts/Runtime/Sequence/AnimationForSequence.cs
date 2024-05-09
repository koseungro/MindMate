/// 작성자: 오상진
/// 작성일: 2021-05-24
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    public class AnimationForSequence : MonoBehaviour, IVisualObject
    {
        #region Singleton
        private static AnimationForSequence _instance;
        public static AnimationForSequence Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AnimationForSequence>();

                    if (_instance == null)
                        Debug.Log("AnimationForSequence를 찾을 수 없습니다. ");
                }
                return _instance;
            }
        }
        #endregion

        VisualType IVisualObject.Type => VisualType.Animation;

        [SerializeField] private Animator curAnimator;
        [SerializeField] private List<Animator> animator_List = new List<Animator>();

        bool IVisualObject.IsFinish => isFinish;
        private bool isFinish = false;

        private float timer = 0.0f;
        private float waitTime = 0.0f;
          
        /// <summary>
        /// 애니메이션 재생 및 해당 컷씬에 필요한 데이터 초기화
        /// </summary>
        /// <param name="option"></param>
        void IVisualObject.Active(CutData option)
        {
            isFinish = false;

            switch (option.animationOption.animType)
            {
                case Anim_FuncType.PlayAnimation:
                    SetAnimatior(option.animationOption);
                    break;
                case Anim_FuncType.ResetAnimList:
                    ResetAnimation();
                    break;
            }

        }

        void IVisualObject.Init()
        {
            // Characters의 Animator 모두 가져오기
            //Animator[] animators = GameObject.Find("Interactable Object").GetComponentsInChildren<Animator>();
            //animator_List.AddRange(animators);

        }

        /// <summary>
        /// 애니메이션 시간동안 Update , 캐릭터 애니메이션 변경 데이터 있다면 변경
        /// </summary>
        void IVisualObject.MyUpdate()
        {
            timer += Time.deltaTime;

            if (timer > waitTime)
            {
                timer = 0.0f;
                //curAnimator = null; // isSame 쓰기 위해선
                isFinish = true;
            }
        }

        private void SetAnimatior(AnimationOption option)
        {
            for (int i = 0; i < animator_List.Count; i++)
            {
                //Debug.Log($"Find : {option.animationOption.charType.ToString()}/ Name : {animator_List[i].gameObject.name}/ 11");

                if (animator_List[i].gameObject.CompareTag(option.charType.ToString()))
                {
                    curAnimator = animator_List[i];
                    //Debug.Log($"Find : {option.animationOption.charType.ToString()}/ Name : {animator_List[i].gameObject.name}/ 22");

                    break;
                }
                else
                {
                    if (i == animator_List.Count - 1) // 마지막 animator 리스트 count
                    {
                        animator_List.Add(GameObject.Find(option.charType.ToString()).GetComponent<Animator>()); // Animator 리스트에 없으면 추가
                        Debug.Log($"<color=yellow> [{option.charType.ToString()}] Animator [{animator_List.Count-1}]번째 추가.</color>");
                    }
                }

            }

            // 여러 애니메이션을 동시에는 안하나?
            //if (animator.isPlaying)
            //    animator.Stop();

            if (option.clip == null)
            {
                isFinish = true;
                curAnimator = null;
                return;
            }

            //Debug.Log(option.animationOption.clip.name);
            curAnimator.SetTrigger(option.clip.name);

            timer = 0.0f;

            if (option.isSame)
                waitTime = 0;
            else
                waitTime = option.clip.length; // * normalizedTime
            //Debug.Log("기다리는 시간" + waitTime);
        }

        /// <summary>
        /// 시퀀스에서 동적으로 추가된 애니메이터 초기화
        /// </summary>
        public void ResetAnimation()
        {
            Debug.Log("Anim Reset");
            for (int i = animator_List.Count - 1; i >= 0; i--)
            {
                if (animator_List[i] == null)
                {
                    animator_List.Remove(animator_List[i]);
                }
            }

            isFinish = true;
        }
    }
}