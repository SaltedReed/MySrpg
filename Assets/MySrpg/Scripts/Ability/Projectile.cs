using UnityEngine;
using MyUtility;

namespace MySrpg
{

    public class Projectile : MonoBehaviour
    {
        public AbilityEvent[] onHit { get; set; }
        public float speed { get; set; }
        public Ability ability { get; set; }
        public Character target { get; set; }

        private void Awake()
        {
            Invoke(nameof(DestroySelf), 10.0f);
        }

        private void Update()
        {
            if (transform.position.SqrMagnitudeXZ(target.transform.position) > 0.25f)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
            else
            {
                ability.AddTarget(target);
                foreach (AbilityEvent ae in onHit)
                {
                    ae.Execute(ability);
                }
                ability.ClearTargets();

                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }

}
