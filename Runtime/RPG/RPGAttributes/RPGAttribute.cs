using System;
using System.Collections.Generic;

namespace INUlib.RPG.RPGAttributes
{
    public enum AttributeType
    {
        Integer,
        Float
    }

    public abstract class RPGAttribute : IAttribute
    {
        #region Fields
        public readonly float defaultValue;
        public readonly float maxValue;

        protected AttributeType _type;
        protected float _currentValue;
        protected float _modsValue;
        protected List<IAttributeMod> _flatMods;
        protected List<IAttributeMod> _percentMods;
        #endregion


        #region Properties
        public float CurrenValue => _currentValue;
        public float ModsValue => _modsValue;
        public virtual float Total => _currentValue + _modsValue;
        public IReadOnlyList<IAttributeMod> FlatMods => _flatMods;
        public IReadOnlyList<IAttributeMod> PercentMods => _percentMods;
        #endregion


        #region Constructors
        public RPGAttribute(AttributeType t)
        {
            _type = t;
            defaultValue = 0;
            maxValue = -1;
            _currentValue = 0;

            _flatMods = new List<IAttributeMod>();
            _percentMods = new List<IAttributeMod>();
        }

        public RPGAttribute(AttributeType t, float dfVal)
        {
            _type = t;
            if(t == AttributeType.Integer)
            {
                defaultValue = (int)dfVal;
                _currentValue = (int)dfVal;
            }
            else
            {
                defaultValue = dfVal;
                _currentValue = dfVal;
            }

            maxValue = -1;
            _flatMods = new List<IAttributeMod>();
            _percentMods = new List<IAttributeMod>();
        }

        public RPGAttribute(AttributeType t, float dfVal, float maxVal)
        {
            _type = t;
            if(t == AttributeType.Integer)
            {
                defaultValue = (int)dfVal;
                maxValue = (int)maxVal;
                _currentValue = (int)dfVal;
            }
            else
            {
                defaultValue = dfVal;
                maxValue = maxVal;
                _currentValue = dfVal;
            }
            
            _flatMods = new List<IAttributeMod>();
            _percentMods = new List<IAttributeMod>();
        }
        #endregion

    
        #region IAttribute Methods
        public int ValueAsInt() => (int)Total;
        public float ValueAsFloat() => Total;
        public event Action onAttributeChanged = delegate { };
        #endregion


        #region Methods
        public abstract float CalculateMods();
        protected void RaiseAttributeChanged() => onAttributeChanged?.Invoke();

        public IAttributeMod AddFlatModifier(int flatVal)
        {
            IAttributeMod mod = new FlatAttributeMod(flatVal);
            _flatMods.Add(mod);

            _modsValue = CalculateMods();
            RaiseAttributeChanged();

            return mod;
        }

        public IAttributeMod AddFlatModifier(float flatVal)
        {
            IAttributeMod mod;
            if(_type == AttributeType.Integer)
                mod = new FlatAttributeMod((int)flatVal);
            else
                mod = new FlatAttributeMod(flatVal);
            
            _flatMods.Add(mod);

            _modsValue = CalculateMods();
            RaiseAttributeChanged();

            return mod;
        }

        public IAttributeMod AddPercentModifier(float pct)
        {
            bool truncate = _type == AttributeType.Integer;
            IAttributeMod mod = new PercentAttributeMod(pct, AttrGetter, truncate);
            
            _percentMods.Add(mod);
            _modsValue = CalculateMods();
            RaiseAttributeChanged();
        
            return mod;
        }

        public bool RemoveFlatModifier(IAttributeMod flatMod)
        {
            bool removed = _flatMods.Remove(flatMod);
            if(removed)
            {
                _modsValue = CalculateMods();
                RaiseAttributeChanged();
            }

            return removed;
        }

        public bool RemovePercentModifier(IAttributeMod pctMod)
        {
            bool removed = _percentMods.Remove(pctMod);
            if(removed)
            {
                _modsValue = CalculateMods();
                RaiseAttributeChanged();
            }

            return removed;
        }
        #endregion
    
        
        #region Helper Methods
        protected virtual float AttrGetter() => _currentValue;
        #endregion
    }
}