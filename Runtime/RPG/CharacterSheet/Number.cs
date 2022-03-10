namespace INUlib.RPG.CharacterSheet
{
    public enum NumberType
    {
        Integer,
        Float
    }

    public class Number
    {
        #region Fields
        private NumberType _type;
        private int _intVal;
        private float _floatVal;
        #endregion

        #region Constructor
        public Number(NumberType t)
        {
            _type  = t;
            _intVal = 0;
            _floatVal = 0;
        }

        public Number(NumberType t, float val)
        {
            _type = t;
            if(t == NumberType.Integer)
                _intVal = (int)val;
            else if(t == NumberType.Float)
                _floatVal = val;
        }

        public Number(NumberType t, int val)
        {
            _type = t;
            if(t == NumberType.Integer)
                _intVal = val;
            else if(t == NumberType.Float)
                _floatVal = val;
        }
        #endregion


        #region Methods
        public float AsFloat()
        {
            if(_type == NumberType.Integer)
                return _intVal;
            else if(_type == NumberType.Float)
                return _floatVal;
            else 
                return 0;  
        }

        public int AsInt()
        {
            if(_type == NumberType.Integer)
                return _intVal;
            else if(_type == NumberType.Float)
                return (int)_floatVal;
            else 
                return 0;   
        }

        public void Add(int val)
        {
            if(_type == NumberType.Integer)
                _intVal += val;
            else if(_type == NumberType.Float)
                _floatVal += val;
        }
        
        public void Add(float val)
        {
            if(_type == NumberType.Integer)
                _intVal += (int)val;
            else if(_type == NumberType.Float)
                _floatVal += val;
        }

        public void Subtract(int val)
        {
            if(_type == NumberType.Integer)
                _intVal -= val;
            else if(_type == NumberType.Float)
                _floatVal -= val;
        }

        public void Subtract(float val)
        {
            if(_type == NumberType.Integer)
                _intVal -= (int)val;
            else if(_type == NumberType.Float)
                _floatVal -= val;
        }

        public void Multiplty(int val)
        {
            if(_type == NumberType.Integer)
                _intVal *= val;
            else if(_type == NumberType.Float)
                _floatVal *= val;
        }

        public void Multiplty(float val)
        {
            if(_type == NumberType.Integer)
                _intVal = (int)((float)_intVal * val);
            else if(_type == NumberType.Float)
                _floatVal *= val;
        }

        public void Divide(int val)
        {
            if(_type == NumberType.Integer)
                _intVal /= val;
            else if(_type == NumberType.Float)
                _floatVal /= val;
        }

        public void Divide(float val)
        {
            if(_type == NumberType.Integer)
                _intVal = (int)((float)_intVal/val);
            else if(_type == NumberType.Float)
                _floatVal = val;
        }
        #endregion
    }
}