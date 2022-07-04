using System;
using UnityEngine;

namespace MySrpg
{

    public abstract class BaseBuff
    {
        public string name;
        public Sprite icon;

        public int maxDuration
        {
            get => m_maxDuration;
            set 
            {
                m_maxDuration = value < 0 ? 0 : value;
                if (m_duration > m_maxDuration)
                    m_duration = m_maxDuration;
            }
        }
        protected int m_maxDuration;

        public int duration
        {
            get => m_duration;
            set => m_duration = Mathf.Clamp(value, 0, maxDuration);
        }
        protected int m_duration;

        public Ability ability { get; set; }
        public Character owner { get; protected set; }


        public void OnAttach(Character character)
        {
            if (character is null)
                throw new ArgumentNullException();

            duration = maxDuration;
            owner = character;
            OnAttachInternal();
        }

        // no need to call base
        protected virtual void OnAttachInternal() { }

        public void OnTick()
        {
            if (duration <= 0)
                throw new InvalidOperationException();

            OnTickInternal();
            --duration;
        }

        // no need to call base
        protected virtual void OnTickInternal() { }

        public void OnDetach()
        {
            OnDetachInternal();
            owner = null;
        }

        // no need to call base
        protected virtual void OnDetachInternal() { }

        public abstract BaseBuff Copy();
    }


}