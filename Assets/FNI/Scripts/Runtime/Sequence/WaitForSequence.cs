
using UnityEngine;

namespace FNI
{
    public class WaitForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.Wait;

        public bool IsFinish { get => isFinish; }
        private bool isFinish = false;

        /// <summary>
        /// Wait 관련 시간 변수
        /// </summary>
        private float checkTime = 0f;
        private float _waitTime;

        public void Active(CutData option)
        {
            checkTime = 0f;
            _waitTime = option.waitOption.waitTime;
            isFinish = false;

            //InvokeRepeating("PrintWait", 0f, 1f);
        }

        public void Init()
        {

        }

        public void MyUpdate()
        {
            checkTime += Time.deltaTime;
            if (checkTime > _waitTime)
            {
                checkTime = 0f;
                _waitTime = 0;
                isFinish = true;
                //CancelInvoke("PrintWait");
            }
        }

        private void PrintWait()
        {
            Debug.Log($"<color=red> Wait Time : {checkTime} </color>");
        }
    }
}
