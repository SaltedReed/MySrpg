using UnityEngine;
using UnityEngine.UI;

namespace MySrpg.UI
{

    public class TrophyListItem : MonoBehaviour
    {
        public Image iconImg;
        public Text expNumText;

        public Sprite icon
        {
            get => iconImg.sprite;
            set => iconImg.sprite = value;
        }

        public float exp
        {
            get => int.Parse(expNumText.text);
            set => expNumText.text = Mathf.RoundToInt(value).ToString();
        }
    }

}