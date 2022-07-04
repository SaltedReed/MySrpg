using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;
using MySrpg;

namespace MySrpg.UI
{

    [RequireComponent(typeof(Button), typeof(Image), typeof(ValueBarWidget))]
    public class AbilityButton : BaseUIWidget
    {
        private Button m_btn;
        private Image m_img;
        private ValueBarWidget m_valueBar;
        [SerializeField]
        private GameObject m_cdGo;
        private int m_index;
        private AbilityPanel m_panel;

        public void OnBindAbility(Ability a, int index, AbilityPanel panel)
        {
            if (m_btn is null)
                m_btn = GetComponent<Button>();
            if (m_img is null)
                m_img = GetComponent<Image>();
            if (m_valueBar is null)
                m_valueBar = GetComponent<ValueBarWidget>();

            m_valueBar.MaxAmount = a.maxCooldown;
            m_valueBar.Amount = a.cooldown;
            if (a.cooldown > 0)
                m_cdGo.SetActive(true);
            else
                m_cdGo.SetActive(false);

            m_index = index;
            m_panel = panel;

            m_img.sprite = a.icon;
            m_btn.onClick.AddListener(OnClick);
        }

        public void OnUnbindAbility()
        {
            m_img.sprite = null;
            m_btn.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            m_panel.OnClickAbilityBtn(m_index);
        }

        public void OnCdStart(int cd)
        {
            m_valueBar.Amount = cd;
            m_cdGo.gameObject.SetActive(true);
        }

        public void OnCdUpdate(int cd)
        {
            m_valueBar.Amount = cd;
        }

        public void OnCdEnd()
        {
            m_valueBar.Amount = 0;
            m_cdGo.gameObject.SetActive(false);
        }
    }

}