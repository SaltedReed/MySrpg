using System;
using UnityEngine;

namespace MyUtility
{

    public sealed class VfxManager
    {
        public static void SpawnParticle(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            if (prefab is null)
            {
                return;
            }

            GameObject go = GameObject.Instantiate(prefab, pos, rot);
            ParticleSystem particles = go.GetComponent<ParticleSystem>();
            GameObject.Destroy(go, particles.main.duration);
        }

        public static void SpawnParticle(GameObject prefab, Vector3 localPosOffset, Matrix4x4 l2w, Quaternion rot)
        {
            if (prefab is null)
            {
                return;
            }

            Vector3 pos = localPosOffset.L2W(l2w);
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.position = pos;
            go.transform.Rotate(rot.eulerAngles); // todo: wrong

            ParticleSystem particles = go.GetComponent<ParticleSystem>();
            GameObject.Destroy(go, particles.main.duration);
        }

    }

}