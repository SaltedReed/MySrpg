using System;

namespace MySrpg
{

    public abstract class LinearField<T>
    {
        public T basicVal;
        public T perUnitVal;
        public bool clampMin;
        public int minX;
        public bool clampMax;
        public int maxX;

        public abstract T Get(int x);
    }


    [Serializable]
    public sealed class LinearInt : LinearField<int>
    {
        public override int Get(int x)
        {
            if (clampMin && x < minX)
                x = minX;
            if (clampMax && x > maxX)
                x = maxX;
            return basicVal + perUnitVal * x;
        }
    }


    [Serializable]
    public sealed class LinearLong : LinearField<long>
    {
        public override long Get(int x)
        {
            if (clampMin && x < minX)
                x = minX;
            if (clampMax && x > maxX)
                x = maxX;
            return basicVal + perUnitVal * x;
        }
    }


    [Serializable]
    public sealed class LinearFloat : LinearField<float>
    {
        public override float Get(int x)
        {
            if (clampMin && x < minX)
                x = minX;
            if (clampMax && x > maxX)
                x = maxX;
            return basicVal + perUnitVal * x;
        }
    }

}