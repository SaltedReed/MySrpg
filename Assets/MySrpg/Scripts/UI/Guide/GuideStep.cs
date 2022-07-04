using UnityEngine;
using MyUtility;
using MyFramework;

namespace MySrpg.UI
{

    public class GuideStep : MonoBehaviour
    {
        public RectTransform target;
        public EventId eventId;
        public float fixedDuration = -1;
        public bool isLastStep;


        public void Execute()
        {
            GetComponentInParent<BaseGuideUI>().Guide(GetComponentInParent<Canvas>(), target);

            if (fixedDuration > 0.0f)
            {
                Invoke(nameof(MoveNext), fixedDuration);
            }

            if (isLastStep)
            {
                Game.Instance.isNovice = false;
            }
        }

        private void MoveNext()
        {
            GetComponentInParent<BaseGuide>().ExecuteNext();
        }
    }

}