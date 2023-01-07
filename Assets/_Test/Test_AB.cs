using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyTest
{

    public class Test_AB : MonoBehaviour
    {
        public string bundleName = "player_dog";
        public string assetName = "player_dog";
        
        public T Load<T>() where T : Object
        {
            LoadDependencies();

            AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
            T ass = ab.LoadAsset<T>(assetName);
            return ass;
        }

        private void LoadDependencies()
        {
            AssetBundle maniAb = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "StandaloneWindows"));
            AssetBundleManifest mani = maniAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] dependencies = mani.GetAllDependencies(bundleName);

            foreach (string d in dependencies)
            {
                AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, d));
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("load"))
            {
                GameObject go = Load<GameObject>();
                Instantiate(go);
            }
        }
    }

}