using System;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;

namespace MySrpg
{

    public delegate void OnTaskUnlockHandler(Task task);
    public delegate void OnTaskFinishHandler(Task task);
    public delegate void OnTaskRewardHandler(Task task);


    public abstract class TaskPrecondition
    {
        public abstract void ListenEvents(Task task);

        public abstract void RemoveListeners(Task task);     
    }

    public class TaskPrecondition_None : TaskPrecondition
    {
        public override void ListenEvents(Task task)
        {
            task.Unlock();
        }

        public override void RemoveListeners(Task task)
        {
        }
    }

    public class TaskPrecondition_BattleNum : TaskPrecondition
    {
        // 0: pve, 1: pvp 
        public int battleType;
        public int number;
        public int battleNum { get; private set; }

        private Task m_task;

        public override void ListenEvents(Task task)
        {
            m_task = task;
            (Game.Instance as SrpgGame).onBattleFinishHandler += OnBattleFinish;
        }

        public override void RemoveListeners(Task task)
        {
            (Game.Instance as SrpgGame).onBattleFinishHandler -= OnBattleFinish;
        }

        private void OnBattleFinish(bool won, int t)
        {
            if (t == battleType)
            {
                ++battleNum;
                if (battleNum == number)
                {
                    m_task.Unlock();
                }
            }
        }
    }


    public abstract class TaskFinishCondition
    {
        public float rewardExp;

        public abstract void ListenEvents(Task task);

        public abstract void RemoveListeners(Task task);
    }

    public class TaskFinishCondition_BattleNum : TaskFinishCondition
    {
        // 0: pve, 1: pvp 
        public int battleType;
        public int number;
        public int battleNum { get; private set; }

        private Task m_task;

        public override void ListenEvents(Task task)
        {
            m_task = task;
            (Game.Instance as SrpgGame).onBattleFinishHandler += OnBattleFinish;
        }

        public override void RemoveListeners(Task task)
        {
            (Game.Instance as SrpgGame).onBattleFinishHandler -= OnBattleFinish;
        }

        private void OnBattleFinish(bool won, int t)
        {
            if (t == battleType)
            {
                ++battleNum;
                if (battleNum == number)
                {
                    m_task.Finish();
                }
            }
        }
    }


    public class Task
    {
        public string content;
        public int expReward;
        public TaskPrecondition precondition;
        public TaskFinishCondition finishCondition;

        public int id { get; set; }

        public bool isUnlocked { get; protected set; }
        public bool isFinished { get; protected set; }
        public bool isRewarded { get; protected set; }

        protected TaskSystem m_taskSys;


        public Task(TaskSystem taskSys)
        {
            m_taskSys = taskSys;
        }

        public void OnInit()
        {
            ListenEventsForUnlock();
        }

        public void Unlock()
        {
            Debug.Assert(!isUnlocked);
            Debug.Assert(!isFinished);
            Debug.Assert(!isRewarded);

            isUnlocked = true;
            RemoveListenersForUnlock();
            ListenEventsForFinish();

            m_taskSys.onTaskUnlockHandler?.Invoke(this);
        }

        public void Finish()
        {
            Debug.Assert(isUnlocked);
            Debug.Assert(!isFinished);
            Debug.Assert(!isRewarded);

            isFinished = true;
            RemoveListenersForFinish();

            m_taskSys.onTaskFinishHandler?.Invoke(this);
        }

        public void Reward()
        {
            Debug.Assert(isUnlocked);
            Debug.Assert(isFinished);
            Debug.Assert(!isRewarded);

            isRewarded = true;
            // temp
            Debug.Log($"+{expReward} exp");

            m_taskSys.onTaskRewardHandler?.Invoke(this);
        }

        protected void ListenEventsForUnlock()
        {
            precondition.ListenEvents(this);
        }

        protected void RemoveListenersForUnlock()
        {
            precondition.RemoveListeners(this);
        }

        protected void ListenEventsForFinish()
        {
            finishCondition.ListenEvents(this);
        }

        protected void RemoveListenersForFinish()
        {
            finishCondition.RemoveListeners(this);
        }

    }


    public class TaskSystem : BaseGameSystem
    {
        public List<Task> tasks { get; private set; }

        public OnTaskUnlockHandler onTaskUnlockHandler;
        public OnTaskFinishHandler onTaskFinishHandler;
        public OnTaskRewardHandler onTaskRewardHandler;

        protected override void Start()
        {
            base.Start();
            LoadTasks();
        }

        private void LoadTasks()
        {
            tasks = new List<Task>(10);

            Task t0 = new Task(this)
            {
                id = 0,
                content = "一共进行一场pve战斗",
                expReward = 10,
                precondition = new TaskPrecondition_None(),
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 0,
                    number = 1
                }
            };
            t0.OnInit();
            tasks.Add(t0);

            Task t1 = new Task(this)
            {
                id = 1,
                content = "一共进行三场pve战斗",
                expReward = 30,
                precondition = new TaskPrecondition_BattleNum
                { 
                    battleType = 0,
                    number = 1
                },
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 0,
                    number = 3
                }
            };
            t1.OnInit();
            tasks.Add(t1);

            Task t2 = new Task(this)
            {
                id = 2,
                content = "一共进行十场pve战斗",
                expReward = 100,
                precondition = new TaskPrecondition_BattleNum
                {
                    battleType = 0,
                    number = 3
                },
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 0,
                    number = 10
                }
            };
            t2.OnInit();
            tasks.Add(t2);

            Task t3 = new Task(this)
            {
                id = 3,
                content = "一共进行一场pvp战斗",
                expReward = 10,
                precondition = new TaskPrecondition_None(),
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 1,
                    number = 1
                }
            };
            t3.OnInit();
            tasks.Add(t3);

            Task t4 = new Task(this)
            {
                id = 4,
                content = "一共进行三场pvp战斗",
                expReward = 30,
                precondition = new TaskPrecondition_BattleNum
                {
                    battleType = 1,
                    number = 1
                },
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 1,
                    number = 3
                }
            };
            t4.OnInit();
            tasks.Add(t4);

            Task t5 = new Task(this)
            {
                id = 5,
                content = "一共进行十场pvp战斗",
                expReward = 100,
                precondition = new TaskPrecondition_BattleNum
                {
                    battleType = 1,
                    number = 3
                },
                finishCondition = new TaskFinishCondition_BattleNum
                {
                    battleType = 1,
                    number = 10
                }
            };
            t5.OnInit();
            tasks.Add(t5);
        }
    }

}