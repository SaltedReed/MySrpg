using System;
using System.Collections.Generic;
using UnityEngine;
using MySrpg;


namespace MyTest
{


    public class Test_Json : MonoBehaviour
    {
        public string json;
        public AbilityConfig config;

        private void OnGUI()
        {
            if (GUILayout.Button("config -> json"))
            {
                config = new AbilityConfig
                {
                    abilityName = "a1",
                    rangeCell = 2,
                    rangeType = RangeType.AllDir,
                    abilityEvents = new List<AbilityEventConfig>
                    {
                        new AbilityEventConfig_AddBuff { timePoint = 0.63f, prototype = new BuffConfig_ModifyProperty { maxDuration=2, property=Character.PropertyIndex.Defense, valueToAdd=-5 } },
                        new AbilityEventConfig_AddBuff { timePoint = 0.53f, prototype = new BuffConfig_ModifyProperty { maxDuration=1, property=Character.PropertyIndex.Defense, valueToAdd=-3 } },
                        new AbilityEventConfig_SpawnVfx {timePoint = 0.35f, path=@"Prefabs/Vfx/v1"},
                    }
                };

                json = LitJson.JsonMapper.ToJson(config);

                Debug.Log(json);
            }
            if (GUILayout.Button("json -> config"))
            {
                config = LitJson.JsonMapper.ToObject<AbilityConfig>(json);

                Debug.Log(config.DebugStr());
            }
        }
    }

}