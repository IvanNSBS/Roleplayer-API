
using System;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedAttribute : IAttribute
    {
        #region Fields
        private IAttribute[] _parents;
        #endregion


        #region Constructors
        ~DerivedAttribute() => UnlinkParents();
        #endregion
    
        
        #region Methods
        public event Action onAttributeChanged = delegate { };

        public abstract int AsInt();
        public abstract float AsFloat();

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
            onAttributeChanged.Invoke();
        }

        public abstract void OnParentChanged();
        #endregion
    }
}