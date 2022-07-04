using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using UnityEngine.SceneManagement;

namespace MySrpg
{

    // battleType: 0: pve, 1: pvp
    public delegate void OnBattleFinishHandler(bool won, int battleType);


    public sealed class SrpgGame : Game
    {
        public string loadingSceneName = "Loading";
        public List<CharacterConfig> characterConfigs;

        public BattleSystem battleSystem { get; private set; }
        public IndicatorSystem indicatorSystem { get; private set; }
        public TaskSystem taskSystem { get; private set; }

        public OnBattleFinishHandler onBattleFinishHandler;


        /*protected override void Awake()
        {
            AddSystem<TaskSystem>();
        }*/

        private void Start()
        {
            AddSystem<TaskSystem>();

        }

        protected override void OnAddSystem<T>(T sys)
        {
            base.OnAddSystem<T>(sys);

            if (sys is BattleSystem)
            {
                battleSystem = sys as BattleSystem;
            }
            else if (sys is IndicatorSystem)
            {
                indicatorSystem = sys as IndicatorSystem;
            }
            else if (sys is TaskSystem)
            {
                taskSystem = sys as TaskSystem;
            }
        }

        protected override void OnRemoveSystem<T>(T sys)
        {
            if (sys is BattleSystem)
            {
                battleSystem = null;
            }
            else if (sys is IndicatorSystem)
            {
                indicatorSystem = null;
            }
            else if (sys is TaskSystem)
            {
                taskSystem = null;
            }

            base.OnRemoveSystem(sys);
        }

        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(loadingSceneName);
            StartCoroutine(LoadSceneCoroutine(scene));
        }

        private IEnumerator LoadSceneCoroutine(string scene)
        {
            yield return new WaitForSeconds(0.4f);
            SceneManager.LoadScene(scene);
        }

    }

}