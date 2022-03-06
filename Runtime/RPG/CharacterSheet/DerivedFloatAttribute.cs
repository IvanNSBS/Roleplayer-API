
using System;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedFloatAttribute<T1> : DerivedAttribute<float> where T1 : Attribute<IComparable>
    {
        #region Fields
        protected T1 _parent;
        #endregion


        #region Constructors
        public DerivedFloatAttribute(float defaultVal, T1 parent) : base(defaultVal)
        {
            _parent = parent;
            LinkParents(_parent);
        } 

        ~DerivedFloatAttribute() => UnlinkParents();
        #endregion
    }

    public abstract class DerivedFloatAttribute<T1, T2> : DerivedAttribute<float> 
    where T1 : Attribute<IComparable> where T2 : Attribute<IComparable>
    {
        #region Fields
        protected T1 _parent1;
        protected T2 _parent2;
        #endregion


        #region Constructors
        public DerivedFloatAttribute(float defaultVal, T1 p1, T2 p2) : base(defaultVal)
        {
            _parent1 = p1;
            _parent2 = p2;
            LinkParents(_parent1, _parent2);
        } 

        ~DerivedFloatAttribute() => UnlinkParents();
        #endregion
    }

    public abstract class DerivedFloatAttribute<T1, T2, T3> : DerivedAttribute<float> 
    where T1 : Attribute<IComparable> where T2 : Attribute<IComparable> where T3 : Attribute<IComparable>
    {
        #region Fields
        protected T1 _parent1;
        protected T2 _parent2;
        protected T3 _parent3;
        #endregion


        #region Constructors
        public DerivedFloatAttribute(float defaultVal, T1 p1, T2 p2, T3 p3) : base(defaultVal)
        {
            _parent1 = p1;
            _parent2 = p2;
            _parent3 = p3;
            LinkParents(_parent1, _parent2, _parent3);
        } 

        ~DerivedFloatAttribute() => UnlinkParents();
        #endregion
    }
}