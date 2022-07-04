using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;
using MySrpg;

namespace MySrpg.UI
{

    public class TaskList : MonoBehaviour
    {
        public GameObject itemPrefab;

        private TaskListItem[] m_items;

        public void AddTasks(Task[] tasks)
        {
            if (m_items != null && m_items.Length > 0)
                RemoveTasks();

            m_items = new TaskListItem[tasks.Length];
            for (int i=0; i<tasks.Length; ++i)
            {
                Task task = tasks[i];

                GameObject go = Instantiate(itemPrefab);
                go.transform.SetParent(transform);
                TaskListItem item = go.GetComponent<TaskListItem>();

                item.task = task;
                item.content = task.content;
                item.rewardExp = task.expReward;
                if (!task.isUnlocked)
                    item.OnLock();
                else if (!task.isFinished)
                    item.OnUnlock();
                else if (!task.isRewarded)
                    item.OnFinish();
                else
                    item.OnReward();

                if (task.finishCondition is TaskFinishCondition_BattleNum)
                {
                    item.gotoBtn.onClick.AddListener(() =>
                    {
                        UIManager.Instance.Pop();
                    });
                }

                m_items[i] = item;
            }
        }

        public void RemoveTasks()
        {
            if (m_items is null)
                return;

            for (int i = 0; i < m_items.Length; ++i)
            {
                if (m_items[i] != null)
                {
                    Destroy(m_items[i].gameObject);
                }
            }
            m_items = null;
        }

    }

}