
using System;
using System.Collections.Generic;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class ModifiableAttribute<T> : IAttribute where T : IComparable
    {
        #region Fields
        protected T _modifiers;
        protected List<IAttributeModifier<T>> _flatMods;
        protected List<IAttributeModifier<T>> _percentMods;
        #endregion


        #region Properties
        public T Modifiers => _modifiers;
        public IReadOnlyList<IAttributeModifier<T>> FlatMods => _flatMods;
        public IReadOnlyList<IAttributeModifier<T>> PercentMods => _percentMods;
        #endregion


        #region Constructor
        public ModifiableAttribute() 
        {
            _flatMods = new List<IAttributeModifier<T>>();
            _percentMods = new List<IAttributeModifier<T>>();
        } 
        #endregion


        #region Methods
        public event Action<T> onModifiersChanged = delegate { };
        public event Action onAttributeChanged = delegate { };

        public virtual IAttributeModifier<T> AddFlatModifier(T amount)
        {
            IAttributeModifier<T> mod = new AttributeModifier<T>(amount); 
            _flatMods.Add(mod);

            _modifiers = CalculateModifiers();
            onModifiersChanged?.Invoke(_modifiers);
            OnAttributeChanged();
            return mod;
        }

        public virtual IAttributeModifier<T> AddPercentModifier(float pct)
        {
            IAttributeModifier<T> mod = new AttributeModifier<T>(Scale(pct));
            _percentMods.Add(mod);
            
            _modifiers = CalculateModifiers();
            onModifiersChanged?.Invoke(_modifiers);
            OnAttributeChanged();
            return mod;
        }

        public virtual bool RemoveFlatModifier(IAttributeModifier<T> mod)
        {
            bool removed = _flatMods.Remove(mod);
            if(removed) 
            {
                _modifiers = CalculateModifiers();
                onModifiersChanged?.Invoke(_modifiers);
                OnAttributeChanged();
            }   

            return removed;
        }

        public virtual bool RemovePercentModifier(IAttributeModifier<T> mod)
        {
            bool removed = _percentMods.Remove(mod);
            if(removed)
            {
                _modifiers = CalculateModifiers();
                onModifiersChanged?.Invoke(_modifiers);
                OnAttributeChanged();
            }

            return removed;
        }

        protected void OnAttributeChanged() => onAttributeChanged?.Invoke();
        #endregion


        #region Interface Methods
        public abstract int AsInt();
        public abstract float AsFloat();
        protected abstract T Scale(float b);
        protected abstract T Zero();

        protected virtual T CalculateModifiers() => Zero();
        #endregion
    }
}