using UnityEngine;

namespace MySrpg
{

    public enum CharacterType
    {
        Melee,
        Archer,
        Tank,
        Aux
    }


    [CreateAssetMenu(fileName = "new CharacterConfig", menuName = "MySrpg/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        [Header("Basic Info")]
        public string characterName;
        public Sprite icon;
        public GameObject prefab;
        public GameObject displayPrefab;

        [Header("Level")]
        public int maxLevel;
        public LinearFloat maxExpFormula;

        [Header("Movement")]
        public int movementRangeCell;
        public MapNodeTag traversal;

        [Header("Battle")]
        public LinearFloat maxHpFormula;
        public LinearFloat maxEnergyFormula;
        public LinearFloat defenseFormula;
        public int attackmentRangeCell;
        public LinearFloat attackmentFormula;
        public CharacterType characterType;
        public string[] abilityConfigPaths;
    }

}