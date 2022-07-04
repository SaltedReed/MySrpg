using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using MyUtility;

namespace MySrpg.UI
{

    public class TaskPageGuide : BaseGuide
    {
        private void Awake()
        {
            if (!ShouldAwakeAndDisactiveIfShouldnt())
                return;

            EventDispatcher.Register((int)EventId.OpenMainPage, ExecuteNext);
        }
    }

}