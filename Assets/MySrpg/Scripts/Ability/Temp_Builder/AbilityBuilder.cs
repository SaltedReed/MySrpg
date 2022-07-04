using UnityEngine;
using MyUtility;


namespace MySrpg
{

    public sealed class AbilityBuilder
    {
        private Ability m_target;

        public Ability Build()
        {
            return m_target;
        }

        public AbilityBuilder Ability(string name, Character owner, AbilityExecutor executor)
        {
            m_target = new Ability(name, owner, executor);
            return this;
        }

        public AbilityBuilder Icon(string path)
        {
            m_target.icon = ResourceManager.Load<Sprite>(path);
            if (m_target.icon is null)
                Debug.LogWarning($"failed to load sprite from {path} for {m_target.abilityName}");
            return this;
        }

        public AbilityBuilder UseOwnerRange(bool value)
        {
            m_target.useOwnerRange = value;
            return this;
        }

        public AbilityBuilder RangeType(RangeType t)
        {
            m_target.rangeType = t;
            return this;
        }

        public AbilityBuilder RangeCell(int range)
        {
            m_target.rangeCell = range;
            return this;
        }

        public AbilityBuilder Event(AbilityEvent ae)
        {
            m_target.AddEvent(ae);
            return this;
        }

    }

}