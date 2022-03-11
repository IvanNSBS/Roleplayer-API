namespace INUlib.RPG.RPGAttributes
{
    public class FlatAttributeMod : IAttributeMod
    {
        #region Fields
        private float _value;
        #endregion

        #region Constructor
        public FlatAttributeMod(int value) => _value = value;
        public FlatAttributeMod(float value) => _value = value;
        #endregion


        #region Methods
        public int ValueAsInt() => (int)_value;
        public float ValueAsFloat() => _value;
        public void RefreshValue() { }
        #endregion
    }
}