
using UnityEngine;

namespace FNI
{

    public class QuitForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.Quit;

        public bool IsFinish => true;

        public void Active(CutData option)
        {

        }

        public void Init()
        {

        }

        public void MyUpdate()
        {

        }

    }
}


