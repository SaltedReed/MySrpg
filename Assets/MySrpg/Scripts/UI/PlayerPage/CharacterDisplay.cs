using System;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class CharacterDisplay : BaseUIWidget
    {
        public Transform modelSlot;
        public Text hpNumText;
        public Text energyNumText;
        public Text movRngNumText;
        public Text atkRngNumText;
        public Text atkNumText;
        public Text dfnsNumText;

        private CharacterList m_characterList;
        private GameObject m_model;
        private int m_animKey_display = Animator.StringToHash("display");


        /// <param name="args">CharacterList</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("CharacterDisplay.Open requires CharacterList as args");

            m_characterList = args as CharacterList;
            if (m_characterList is null)
                throw new InvalidCastException("CharacterDisplay.Open requires CharacterList as args");

            base.OnOpen(args);

            m_characterList.onSelectCharacterHandler += OnSelectCharacter;
        }

        public override void OnClose()
        {
            if (m_characterList != null)
                m_characterList.onSelectCharacterHandler -= OnSelectCharacter;

            base.OnClose();
        }

        public void OnSelectCharacter(CharacterConfig character)
        {
            m_model = Instantiate(character.displayPrefab);
            m_model.transform.position = Vector3.zero;
            if (modelSlot.childCount > 0)
                Destroy(modelSlot.GetChild(0).gameObject);
            m_model.transform.SetParent(modelSlot, false);

            hpNumText.text = character.maxHpFormula.basicVal.ToString();
            energyNumText.text = character.maxEnergyFormula.basicVal.ToString();
            movRngNumText.text = character.movementRangeCell.ToString();
            atkRngNumText.text = character.attackmentRangeCell.ToString();
            atkNumText.text = character.attackmentFormula.basicVal.ToString();
            dfnsNumText.text = character.defenseFormula.basicVal.ToString();

            Invoke(nameof(ModelPlayAnim), 0.5f);
        }

        private void ModelPlayAnim()
        {
            Animator animator = m_model.GetComponentInChildren<Animator>();
            if (animator is null)
                return;
            animator.SetTrigger(m_animKey_display);
        }
    }

}