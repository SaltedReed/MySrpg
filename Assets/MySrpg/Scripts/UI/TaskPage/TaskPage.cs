using UnityEngine.UI;
using MyFramework;
using MyFramework.UI;
using MyUtility;

namespace MySrpg.UI
{

    public class TaskPage : BaseUIWidget
    {
        public TaskList taskList;
        public Button closeBtn;

        public override void OnCreate(UIManager uiMngr, object args = null)
        {
            base.OnCreate(uiMngr, args);

            closeBtn.onClick.AddListener(() =>
            {
                uiManager.Pop();
            });
        }

        public override void OnOpen(object args = null)
        {
            base.OnOpen(args);

            TaskSystem taskSys = (Game.Instance as SrpgGame).taskSystem;
            taskList.AddTasks(taskSys.tasks.ToArray());

            EventDispatcher.DispatchEvent((int)EventId.OpenTaskPage);
        }

        public override void OnClose()
        {
            taskList.RemoveTasks();

            base.OnClose();
        }

    }

}