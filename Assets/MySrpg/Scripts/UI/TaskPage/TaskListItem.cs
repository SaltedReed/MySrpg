using UnityEngine;
using UnityEngine.UI;
using MyFramework;
using MySrpg;

namespace MySrpg.UI
{

    public class TaskListItem : MonoBehaviour
    {
        public Text contentText;
        public Text rewardNumText;
        public Button gotoBtn;
        public Button rewardBtn;

        public Task task { get; set; }

        public string content
        {
            get => contentText.text;
            set => contentText.text = value;
        }

        public int rewardExp
        {
            get => -1; // no need to get
            set => rewardNumText.text = $"+{value} exp";
        }

        public void OnLock()
        {
            gotoBtn.gameObject.SetActive(true);
            gotoBtn.interactable = false;
            rewardBtn.gameObject.SetActive(false);
        }

        public void OnUnlock()
        {
            rewardBtn.gameObject.SetActive(false);
            gotoBtn.gameObject.SetActive(true);
            gotoBtn.interactable = true;
        }

        public void OnFinish()
        {
            gotoBtn.gameObject.SetActive(false);
            rewardBtn.gameObject.SetActive(true);
            rewardBtn.interactable = true;
        }

        public void OnReward()
        {
            gotoBtn.gameObject.SetActive(false);
            rewardBtn.gameObject.SetActive(true);
            rewardBtn.interactable = false;
        }

        private void Start()
        {
            rewardBtn.onClick.AddListener(() =>
            {
                task.Reward();
            });

            TaskSystem taskSys = (Game.Instance as SrpgGame).taskSystem;
            taskSys.onTaskUnlockHandler += OnTaskUnlock;
            taskSys.onTaskFinishHandler += OnTaskFinish;
            taskSys.onTaskRewardHandler += OnTaskReward;
        }

        private void OnDestroy()
        {
            TaskSystem taskSys = (Game.Instance as SrpgGame).taskSystem;
            taskSys.onTaskUnlockHandler -= OnTaskUnlock;
            taskSys.onTaskFinishHandler -= OnTaskFinish;
            taskSys.onTaskRewardHandler -= OnTaskReward;
        }

        private void OnTaskUnlock(Task t)
        {
            if (t == task)
                OnUnlock();
        }

        private void OnTaskFinish(Task t)
        {
            if (t == task)
                OnFinish();
        }

        private void OnTaskReward(Task t)
        {
            if (t == task)
                OnReward();
        }

    }

}