using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class ValueBarWidget : BaseUIWidget
    {
        public float MaxAmount 
        {
            get => m_maxAmount; 
            set
            {
                if (value < Amount)
                    Amount = value;
                m_maxAmount = value;
                m_amountImg.fillAmount = m_amount / MaxAmount;
            }
        }
        protected float m_maxAmount;

        public float Amount
        {
            get => m_amount;
            set
            {
                m_amount = Mathf.Clamp(value, 0, MaxAmount);
                m_amountImg.fillAmount = m_amount / MaxAmount;
            }
        }
        protected float m_amount;

        [SerializeField]
        protected Image m_amountImg;
        
    }

}