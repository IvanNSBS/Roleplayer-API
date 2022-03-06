
using System;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedAttribute<T> : Attribute<T> where T : IComparable
    {
        #region Fields
        private Attribute<IComparable>[] _parents;
        #endregion


        #region Constructors
        public DerivedAttribute(T defaultVal) : base(defaultVal) { }
        ~DerivedAttribute() => UnlinkParents();
        #endregion
    
        
        #region Methods
        protected virtual void UnlinkParents()
        {
            if(_parents == null)
                return;

            foreach(var p in _parents)
            {
                p.onValueChanged -= UpdateValue;
                p.onModifiersChanged -= UpdateValue;
            }
        }

        protected void LinkParents(params Attribute<IComparable>[] parents)
        {
            UnlinkParents();
            _parents = parents;
            foreach(var p in _parents)
            {
                p.onValueChanged += UpdateValue;
                p.onModifiersChanged += UpdateValue;
            }
        }

        private void UpdateValue(IComparable t) => OnParentChanged();
        public abstract void OnParentChanged();
        #endregion
    }
}