using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;


namespace MyTest
{

    public class Test_ResourceManager : MonoBehaviour
    {
        Object ass;

        private void OnGUI()
        {
            if (GUILayout.Button("test"))
            {
                GameObject p = ResourceManager.Load<GameObject>("player_dog", "player_dog");
                Instantiate(p);
                p = ResourceManager.Load<GameObject>("player_dog", "player_dog");
                Instantiate(p);
            }
        }

    }

}