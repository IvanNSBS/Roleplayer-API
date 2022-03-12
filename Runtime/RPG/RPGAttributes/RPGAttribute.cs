using System;
using System.Collections.Generic;

namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Defines how the RPG Attribute will calculate his internal value, 
    /// either as a Integer or Float
    /// </summary>
    public enum AttributeType
    {
        Integer,
        Float
    }

    /// <summary>
    /// Default IAttribute implementation. RPGAttribute manages the attribute modifiers
    /// and hols two values: Current Value, the intrinsic value of the Attribute, and the
    /// ModsValue, that holds the mods value.
    /// It accepts, by default, two types of Mods: Flat mods and Percentage mods.
    /// Percent mods acts on the current attribute value.
    /// How the Mods will be calculated to set the ModsValue is defined by the abstract function
    /// CalculateMods.
    /// </summary>
    public abstract class RPGAttribute : IAttribute
    {
        #region Fields
        /// <summary>
        /// The default and initial Attribute Value
        /// </summary>
        public readonly float defaultValue;

        /// <summary>
        /// The max value an attribute can reach. -1 means that there's no max value
        /// Note: RPGAttribute does not use it, but it's left here for the inherotors usage
        /// </summary>
        public readonly float maxValue;

        /// <summary>
        /// The attribute math type
        /// </summary>
        protected AttributeType _type;

        /// <summary>
        /// Current attribute intrinsic value
        /// </summary>
        protected float _currentValue;

        /// <summary>
        /// Current calculated mods value
        /// </summary>
        protected float _modsValue;

        /// <summary>
        /// List of flatMods added to the attribute
        /// </summary>
        protected List<IAttributeMod> _flatMods;

        /// <summary>
        /// List of the percentMods added to the attribute
        /// </summary>
        protected List<IAttributeMod> _percentMods;
        #endregion


        #region Properties
        /// <summary>
        /// Getter for the Current Intrinsic Value of the attribute
        /// </summary>
        public float CurrentValue => _currentValue;

        /// <summary>
        /// Getter for the Current Calculated Mods value of the attribute
        /// </summary>
        public float ModsValue => _modsValue;

        /// <summary>
        /// Virtual Method that sums the Current Value and the Mods value, returning
        /// the final value of the attribute
        /// </summary>
        public virtual float Total => _currentValue + _modsValue;

        /// <summary>
        /// Getter for the FlatMods list of the attribute
        /// </summary>
        public IReadOnlyList<IAttributeMod> FlatMods => _flatMods;

        /// <summary>
        /// Getter for the PercentMods list of the attribute
        /// </summary>
        public IReadOnlyList<IAttributeMod> PercentMods => _percentMods;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates the attribute with default value as 0 and max value as -1
        /// </summary>
        /// <param name="t">AttributeType math type. Integer or Float</param>
        public RPGAttribute(AttributeType t)
        {
            _type = t;
            defaultValue = 0;
            maxValue = -1;
            _currentValue = 0;

            _flatMods = new List<IAttributeMod>();
            _percentMods = new List<IAttributeMod>();
        }


        /// <summary>
        /// Creates the attribute with a Math type, but with a default value
        /// </summary>
        /// <param name="t">AttributeType math type. Integer or Float</param>
        /// <param name="dfVal">The attribute default value</param>
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

        /// <summary>
        /// Creates the attribute with a Math type, a default value and max value
        /// </summary>
        /// <param name="t">AttributeType math type. Integer or Float</param>
        /// <param name="dfVal">The attribute default value</param>
        /// <param name="maxVal">The attribute max possible value</param>
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
        /// <summary>
        /// Returns the total value of the attribute, as a int (truncated float)
        /// </summary>
        /// <returns></returns>
        public int ValueAsInt() => (int)Total;
        
        /// <summary>
        /// Returns the total value of the atttribute, as a float. 
        /// </summary>
        /// <returns></returns>
        public float ValueAsFloat() => Total;

        public event Action onAttributeChanged = delegate { };
        #endregion


        #region Methods
        /// <summary>
        /// Function that defines how the ModsValue will be calculated from the
        /// FlatMods and PercentMods list
        /// </summary>
        /// <returns>The new ModsValue value</returns>
        public abstract float CalculateMods();

        /// <summary>
        /// Wrapper to fire the onAttributeChanged so child objects can call it
        /// </summary>
        protected void RaiseAttributeChanged() => onAttributeChanged?.Invoke();


        /// <summary>
        /// Adds a flat modifier to the Attribute from a float and updates the ModsValue.
        /// If the AttributeType is Integer, the float will be truncated to an int
        /// Fires onAttributeChanged
        /// </summary>
        /// <param name="flatVal">The value to be added</param>
        /// <returns>The Created IAttributeMod</returns>
        public IAttributeMod AddFlatModifier(int flatVal)
        {
            IAttributeMod mod = new FlatAttributeMod(flatVal);
            _flatMods.Add(mod);

            _modsValue = CalculateMods();
            RaiseAttributeChanged();

            return mod;
        }

        /// <summary>
        /// Adds a flat modifier to the Attribute from an integer and updates the ModsValue.
        /// Fires onAttributeChanged
        /// </summary>
        /// <param name="flatVal">The value to be added</param>
        /// <returns>The Created IAttributeMod</returns>
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

        /// <summary>
        /// Adds a Modifier that gets it's value from the CurrentValue RPGAttribute field
        /// and updates the ModsValue.
        /// If the Attribute Type is Integer, the calculated PrecentAttributeMod will be truncated
        /// to an integer. 
        /// Fires onAttributeChanged
        /// </summary>
        /// <param name="pct">The desired percent of the modifier</param>
        /// <returns>The Created IAttributeMod</returns>
        public IAttributeMod AddPercentModifier(float pct)
        {
            bool truncate = _type == AttributeType.Integer;
            IAttributeMod mod = new PercentAttributeMod(pct, AttrGetter, truncate);
            
            _percentMods.Add(mod);
            _modsValue = CalculateMods();
            RaiseAttributeChanged();
        
            return mod;
        }

        /// <summary>
        /// Removes a flat modifier from the Attribute.
        /// Updates the Mods value and fires onAttributeChanged if the modifier 
        /// were succesfully removed
        /// </summary>
        /// <param name="flatMod">The Modifier to be removed from the FlatMods</param>
        /// <returns>True if the modifier was removed. False otherwise</returns>
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

        /// <summary>
        /// Removes a percent modifier from the Attribute.
        /// Updates the Mods value and fires onAttributeChanged if the modifier 
        /// were succesfully removed
        /// </summary>
        /// <param name="pctMod">The Modifier to be removed from the PctMods</param>
        /// <returns>True if the modifier was removed. False otherwise</returns>
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
        /// <summary>
        /// Getter for the CurrentValue. Used so the PercentModifier can be correctly refreshed
        /// </summary>
        /// <returns>The current attribute intrinsic value</returns>
        protected virtual float AttrGetter() => _currentValue;

        /// <summary>
        /// Refreses all the RPGAttribute Modifiers
        /// </summary>
        protected void RefreshMods()
        {
            foreach(IAttributeMod flatMod in _flatMods)
                flatMod.RefreshValue();
            foreach(IAttributeMod pctMod in _percentMods)
                pctMod.RefreshValue();
        }
        #endregion
    }
}