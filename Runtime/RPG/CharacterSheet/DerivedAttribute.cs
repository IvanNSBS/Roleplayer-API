
using System;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedAttribute<T> : RPGAttribute<T> where T : IComparable
    {
        #region Fields
        private IAttribute[] _parents;
        #endregion


        #region Constructors
        ~DerivedAttribute() => UnlinkParents();
        #endregion
    
        
        #region Methods
        protected virtual void UnlinkParents()
        {
            if(_parents == null)
                return;

            foreach(var p in _parents)
                p.onAttributeChanged -= ApplyChanges;
        }

        protected void LinkParents(params IAttribute[] parents)
        {
            UnlinkParents();
            _parents = parents;

            foreach(var p in _parents)
                p.onAttributeChanged += ApplyChanges;
        }

        private void ApplyChanges()
        {
            OnParentChanged();
            OnAttributeChanged();
        }

        public abstract void OnParentChanged();
        #endregion
    }
}