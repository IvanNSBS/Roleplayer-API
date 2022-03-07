namespace INUlib.RPG.CharacterSheet
{
    public abstract class INumber
    {
        public abstract INumber Sum(INumber b);
        public abstract INumber Subtract(INumber b);
        public abstract INumber Multiply(INumber b);
        public abstract INumber Divide(INumber b);
        protected abstract int AsInt();
        protected abstract float AsFloat();


        #region Operators
        public static implicit operator int(INumber num) => num.AsInt();
        public static implicit operator float(INumber num) => num.AsFloat();

        public static INumber operator +(INumber a, INumber b) => a.Sum(b);
        public static INumber operator -(INumber a, INumber b) => a.Subtract(b);
        public static INumber operator *(INumber a, INumber b) => a.Multiply(b);
        public static INumber operator /(INumber a, INumber b) => a.Divide(b);
        #endregion
    }
}