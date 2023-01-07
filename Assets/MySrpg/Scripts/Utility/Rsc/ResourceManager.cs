#define SRPG_DEBUG

using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace MyUtility
{

    public class ResourceManager
    {
        private static Dictionary<string, Object> m_cachedAssets = new Dictionary<string, Object>(30);
        private static Dictionary<string, AssetBundle> m_cachedAb = new Dictionary<string, AssetBundle>(10);

        public static T Load<T>(string bundleName, string assetName) where T : Object
        {
            Object obj;
            if (m_cachedAssets.TryGetValue(assetName, out obj))
                return obj as T;

#if SRPG_DEBUG
            obj = Resources.Load<T>(Path.Combine(bundleName, assetName));
#else
            LoadDependencies(bundleName);
            AssetBundle ab = LoadAb(bundleName);
            obj = ab.LoadAsset<T>(assetName);
#endif

            m_cachedAssets.Add(assetName, obj);
            return obj as T;
        }

        public static void Clear()
        {
            m_cachedAssets.Clear();
#if !SRPG_DEBUG
            foreach (AssetBundle ab in m_cachedAb.Values)
                ab.Unload(false);
            m_cachedAb.Clear();
#endif
        }

        private static void LoadDependencies(string bundleName)
        {
            AssetBundle maniAb = LoadAb("StandaloneWindows");
            AssetBundleManifest mani = maniAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] dependencies = mani.GetAllDependencies(bundleName);

            foreach (string d in dependencies)
            {
                LoadAb(d);
            }
        }

        private static AssetBundle LoadAb(string bundleName)
        {
            AssetBundle ab;
            if (m_cachedAb.TryGetValue(bundleName, out ab))
                return ab;

            ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
            m_cachedAb.Add(bundleName, ab);
            return ab;
        }

    }

}