using System.Collections.Generic;
using System.Linq;
using INUlib.Utils.Math;

namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// DerivedAttributes are attributes that have their CurrentValue derived from
    /// parent attributes.
    /// The CurrentValue will be set whenever one of the parent attributes change
    /// </summary>
    public abstract class DerivedAttribute : RPGAttribute
    {
        #region Fields
        private IAttribute[] _parents;
        #endregion


        #region Properties
        /// <summary>
        /// Getter for the parent list of the DerivedAttribute
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IAttribute> Parents => _parents?.ToList();
        #endregion


        #region Constructors
        /// <summary>
        /// Constructor that sets the DerivedAttribute Type only
        /// </summary>
        /// <param name="t">The attribute type</param>
        public DerivedAttribute(AttributeType t) : base(t) { }

        /// <summary>
        /// Constructor that sets the DerivedAttribute Type and default value
        /// </summary>
        /// <param name="t">The attribute type</param>
        /// <param name="dfVal">The attribute defaultValue</param>
        public DerivedAttribute(AttributeType t, float dfVal, float minVal) : base(t, dfVal, minVal) { } 
        
        /// <summary>
        /// Constructor that sets the DerivedAttribute Type, default and max value
        /// </summary>
        /// <param name="t">The attribute type</param>
        /// <param name="dfVal">The attribute defaultValue</param>
        /// <param name="maxVal">The attribute maxVal</param>
        public DerivedAttribute(AttributeType t, float dfVal, float minVal, float maxVal) : base(t, dfVal, minVal, maxVal) { }
        #endregion


        #region Methods
        /// <summary>
        /// Method that defines how the currentValue will be updated, given the list of parents
        /// </summary>
        /// <returns>The updated attribute currentValue</returns>
        protected abstract float UpdateAttribute();

        /// <summary>
        /// Applies the UpdateAttribute to the _currentValue field
        /// </summary>
        protected virtual void ApplyUpdate() 
        {
            float updated = UpdateAttribute();
            if(maxValue > 0)
                _currentValue = INUMath.Clamp(updated, minValue, maxValue);
            else 
                _currentValue = updated < minValue ? minValue : updated;
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Links the Attribute parents, listening to their onAttributeChanged
        /// to apply the UpdateAttribute accordingly and immediately Applies the Update.
        /// </summary>
        /// <param name="parent">The required parent to link</param>
        /// <param name="others">Params to link N other parents</param>
        public virtual void LinkParents(IAttribute parent, params IAttribute[] others)
        {
            UnlinkParents();
            _parents = new IAttribute[others.Length + 1];
            
            _parents[0] = parent;
            for(int i = 1; i < others.Length + 1; i++)
                _parents[i] = others[i-1];

            foreach(var attr in _parents)
                attr.onAttributeChanged += ApplyUpdate;

            ApplyUpdate();
        }

        /// <summary>
        /// Unlinks all parents, sets the Parent list to null and Applies the Attribute Update
        /// </summary>
        protected void UnlinkParents()
        {
            if(_parents == null)
                return;

            foreach(var attr in _parents)
                attr.onAttributeChanged -= ApplyUpdate;

            _parents = null;
            ApplyUpdate();
        }
        #endregion
    }
}