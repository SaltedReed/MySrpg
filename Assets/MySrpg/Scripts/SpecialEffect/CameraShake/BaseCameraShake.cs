using UnityEngine;

namespace MySrpg
{

    public abstract class BaseCameraShake : MonoBehaviour
    {
        public Camera camera;

        public abstract void Begin();

    }

}