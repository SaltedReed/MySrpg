

namespace MySrpg
{


    public class Buff_ModifyProperty : BaseBuff
    {
        public Character.PropertyIndex property;
        public float valueToAdd;

        protected override void OnAttachInternal()
        {
            owner[(int)property] += valueToAdd;
        }

        protected override void OnDetachInternal()
        {
            owner[(int)property] -= valueToAdd;
        }

        public override BaseBuff Copy()
        {
            Buff_ModifyProperty buff = new Buff_ModifyProperty();

            buff.name = name;
            buff.icon = icon;
            buff.maxDuration = maxDuration;
            buff.duration = duration;
            buff.ability = ability;
            buff.owner = owner;
            buff.property = property;
            buff.valueToAdd = valueToAdd;

            return buff;
        }

    }


}
