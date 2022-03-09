namespace INUlib.RPG.CharacterSheet
{
    public abstract class INumber
    {
        public abstract INumber Sum(INumber b);
        public abstract INumber Sum(float b);
        public abstract INumber Sum(int b);
        public abstract INumber Subtract(INumber b);
        public abstract INumber Subtract(float b, bool preceding);
        public abstract INumber Subtract(int b, bool preceding);
        public abstract INumber Multiply(INumber b);
        public abstract INumber Multiply(float b);
        public abstract INumber Multiply(int b);
        public abstract INumber Divide(INumber b);
        public abstract INumber Divide(float b, bool preceding);
        public abstract INumber Divide(int b, bool preceding);
        public abstract int AsInt();
        public abstract float AsFloat();


        #region Operators
        // public static implicit operator INumber(int a) => new Int(a);
        // public static implicit operator INumber(float a) => new Float(a);
        public static implicit operator int(INumber num) => num.AsInt();
        public static implicit operator float(INumber num) => num.AsFloat();

        public static INumber operator +(INumber a, INumber b) => a.Sum(b);
        public static INumber operator +(INumber a, int b) => a.Sum(b);
        public static INumber operator +(INumber a, float b) => a.Sum(b);
        public static INumber operator +(int a, INumber b) => b.Sum(a);
        public static INumber operator +(float a, INumber b) => b.Sum(a);

        public static INumber operator -(INumber a, INumber b) => a.Subtract(b);
        public static INumber operator -(INumber a, float b) => a.Subtract(b, false);
        public static INumber operator -(INumber a, int b) => a.Subtract(b, false);
        public static INumber operator -(int a, INumber b) => b.Subtract(a, true);
        public static INumber operator -(float a, INumber b) => b.Subtract(a, true);


        public static INumber operator *(INumber a, INumber b) => a.Multiply(b);
        public static INumber operator *(INumber a, int b) => a.Multiply(b);
        public static INumber operator *(INumber a, float b) => a.Multiply(b);
        public static INumber operator *(int a, INumber b) => b.Multiply(a);
        public static INumber operator *(float a, INumber b) => b.Multiply(a);
        
        public static INumber operator /(INumber a, INumber b) => a.Divide(b);
        public static INumber operator /(INumber a, float b) => a.Divide(b, false);
        public static INumber operator /(INumber a, int b) => a.Divide(b, false);
        public static INumber operator /(float a, INumber b) => b.Divide(a, true);
        public static INumber operator /(int a, INumber b) => b.Divide(a, true);
        #endregion
    }
}