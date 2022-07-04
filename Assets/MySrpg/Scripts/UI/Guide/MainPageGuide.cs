using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using MyUtility;

namespace MySrpg.UI
{

    public class MainPageGuide : BaseGuide
    {
        private void Awake()
        {
            if (!ShouldAwakeAndDisactiveIfShouldnt())
                return;

            EventDispatcher.Register((int)EventId.OpenMainPage, ExecuteNext);
        }

        protected override void OnComplete()
        {
            base.OnComplete();

            EventDispatcher.Unregister((int)EventId.OpenMainPage, ExecuteNext);
        }
    }

}