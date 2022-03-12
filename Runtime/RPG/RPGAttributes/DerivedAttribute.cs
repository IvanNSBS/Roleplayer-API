using System.Collections.Generic;
using System.Linq;

namespace INUlib.RPG.RPGAttributes
{
    public abstract class DerivedAttribute : RPGAttribute
    {
        #region Fields
        private IAttribute[] _parents;
        #endregion


        #region Properties
        public IReadOnlyList<IAttribute> Parents => _parents?.ToList();
        #endregion


        #region Constructors
        public DerivedAttribute(AttributeType t) : base(t) { }
        public DerivedAttribute(AttributeType t, float dfVal) : base(t, dfVal) { }
        public DerivedAttribute(AttributeType t, float dfVal, float maxVal) : base(t, dfVal, maxVal) { }
        #endregion


        #region Methods
        public abstract void UpdateAttribute();
        #endregion


        #region Helper Methods
        protected void LinkParents(IAttribute parent, params IAttribute[] others)
        {
            UnlinkParents();
            _parents = new IAttribute[others.Length + 1];
            
            _parents[0] = parent;
            for(int i = 1; i < others.Length + 1; i++)
                _parents[i] = others[i-1];

            foreach(var attr in _parents)
                attr.onAttributeChanged += UpdateAttribute;

            UpdateAttribute();
        }

        protected void UnlinkParents()
        {
            if(_parents == null)
                return;

            foreach(var attr in _parents)
                attr.onAttributeChanged -= UpdateAttribute;

            _parents = null;
            UpdateAttribute();
        }
        #endregion
    }
}