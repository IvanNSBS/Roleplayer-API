using System;
using System.Collections.Generic;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Default controller for agents that can use the ability system
    /// Fully manages the abilities cooldown and their casting process
    /// </summary>
    public class AbilitiesController<TAbility, TCaster> 
           where TAbility : class, IAbility<TCaster> where TCaster : ICasterInfo
    {
        #region Fields
        protected TCaster _caster;
        protected TAbility[] _abilities;
        protected TAbility _casting;
        protected CastingState _castingState;
        protected CooldownHandler _cdHandler;
        protected CastHandler _castHandler;
        protected List<CastHandler> _activeAbilities;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the number of AbilitySlots assigned in the constructor
        /// </summary>
        public uint AbilitySlots {get; private set;}

        /// <summary>
        /// Returns the current cooldown handler that manages everything related to abilities cooldowns
        /// </summary>
        public CooldownHandler CooldownsHandler => _cdHandler;

        /// <summary>
        /// Getter for the AbilityController Caster Info
        /// </summary>
        public TCaster CasterInfo => _caster;

        /// <summary>
        /// Getter for the current actor casting state
        /// </summary>
        public CastingState CastingState => _castingState;

        public IReadOnlyList<CastHandler> ActiveAbilities => _activeAbilities;
        
        /// <summary>
        /// Whether or not to update cooldowns on the abilities controller Update function
        /// or manually from the user explicitly calling UpdateCooldowns function.
        /// </summary>
        public CooldownUpdateType CooldownUpdateType { get; set; }
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt, TCaster caster, Func<float, float, float> cdrCalcFunc = null)
        {
            AbilitySlots = slotAmnt;
            _caster = caster;
            _abilities = new TAbility[slotAmnt];
            _casting = null;
            _activeAbilities = new List<CastHandler>();
            _cdHandler = new CooldownHandler(_abilities, cdrCalcFunc);
        }
        #endregion


        #region Cast Methods
        /// <summary>
        /// Starts channeling the ability in the given slot
        /// If the channeling time is 0, the ability is unleashed
        /// instantly
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <returns>True if the ability started channeling. False otherwise</returns>
        public bool StartChanneling(uint slot)
        {
            if(!HasAbilityInSlot(slot) || !_abilities[slot].CanCast(_caster))
                return false;

            CastHandler hh;
            bool isMultipleChargesAbility = _cdHandler.GetCooldownInfo(slot).maxCharges > 1;
            bool hasEnoughCharges = _cdHandler.AbilityHasCharges(slot);
            
            bool notOnPrimaryCd = !_cdHandler.IsAbilityOnCd(slot);
            bool notOnCastPrevention = !_cdHandler.IsAbilityOnCastPrevention(slot);
            bool notCastingAnotherSpell = _casting == null;

            bool canCastMultipleCharges = isMultipleChargesAbility && (hasEnoughCharges || notOnPrimaryCd);
            bool canCastSingleCharge = !isMultipleChargesAbility && hasEnoughCharges && notOnPrimaryCd;

            if((hh = GetActiveHandler(GetAbility(slot))) != null && notCastingAnotherSpell)
            {
                _castingState = CastingState.Channeling;
                _casting = (TAbility)hh.Parent;
                hh.OnAnotherCastRequested();
            }
            else if((canCastSingleCharge || canCastMultipleCharges) && notOnCastPrevention && notCastingAnotherSpell)
            {
                _casting = _abilities[slot];
                _castingState = CastingState.Channeling;
                CastObjects castInfo = _casting.Cast(_caster, slot);

                var handler = new CastHandler(_casting, castInfo, GetCastingState);
                _castHandler = handler;
                _activeAbilities.Add(_castHandler);

                handler.AbilityBehaviour.Disable();
                handler.Timeline.ChannelingFinished_OverchannelingStarted += FinishChanneling;
                handler.Timeline.OverchannelingFinished_CastingStarted += FinishOverchannelling;
                handler.Timeline.CastFinished_ConcentrationStarted += FinishCastingAbility;
                handler.Timeline.ConcentrationFinished_RecoveryStarted += FinishConcentration;
                handler.Timeline.Timeline_And_Recovery_Finished += FinishRecovery;
                handler.AbilityBehaviour.NotifyDiscard += () => RemoveAbility(handler);
                handler.Timeline.UnleashAbility += () => _cdHandler.ConsumeCharges(slot, 1);

                if(_casting.StartCooldownPolicy == StartCooldownPolicy.AfterDiscard)
                    handler.AbilityBehaviour.NotifyDiscard += () => _cdHandler.PutOnCooldown(slot);

                if (_casting.StartCooldownPolicy == StartCooldownPolicy.AfterUnleash)
                    handler.Timeline.UnleashAbility += () => _cdHandler.PutOnCooldown(slot);
                
                handler.SetupAbilityBehaviourTimelineCallbacks();
                
                // Updates cast handler with a deltaTime of 0 so instant spells(0 channeling and castTime)
                // might be cast on the same frame instead of the next
                _castHandler.Update(0f);
            }

            return true;
        }

        /// <summary>
        /// If set to true during Channeling, it will prepare to skip Overchanneling.
        /// If set to true during Overchanneling, it'll end it prematurely.
        /// Will have no effect if the casting state is not Channeling or Overchanneling
        /// </summary>
        /// <param name="skip">Whether or not to skip overchanneling</param>
        public void SkipOverchanneling(bool skip) => _castHandler.Timeline.SkipOverchanneling(skip);
        
        /// <summary>
        /// Cancels the ability being cast for the current spell, regardless of the current
        /// casting state and sets the current ability being cast to null
        /// </summary>
        public virtual void CancelCast() 
        {
            _castHandler?.AbilityBehaviour.OnCancelRequested(_castingState);
            _castHandler?.Timeline.JumpToStartRecoveryState();
        } 
        
        /// <summary>
        /// Force interrupts the current ability being cast,
        /// clearing which spell is currently being cast and notifying the ability behaviour of this event.
        /// </summary>
        public virtual void ForceInterruptCast() 
        {
            if (_castHandler != null)
            {
                _castHandler.AbilityBehaviour.OnForcedInterrupt();
            }
            
            _casting = null;
            _castHandler = null;
            _castingState = CastingState.None;
        } 

        /// <summary>
        /// Helper method to cast the ability after it is ready to be cast
        /// </summary>
        protected virtual void FinishChanneling()
        {
            _castingState = CastingState.OverChanneling;
        }

        protected virtual void FinishOverchannelling()
        {
            if(_casting.StartCooldownPolicy == StartCooldownPolicy.AfterChanneling)
                _cdHandler.PutOnCooldown(_casting);
            
            _castingState = CastingState.Casting;
        }

        /// <summary>
        /// Finishes casting the current ability, setting the Casted Ability to null and
        /// the casting state to None
        /// </summary>        
        protected void FinishCastingAbility()
        {
            if(_casting.StartCooldownPolicy == StartCooldownPolicy.AfterCasting)
                _cdHandler.PutOnCooldown(_casting);
            
            _castingState = CastingState.Concentrating;
        }

        public void FinishConcentration()
        {
            if(_casting.StartCooldownPolicy == StartCooldownPolicy.AfterConcentrating)
                _cdHandler.PutOnCooldown(_casting);
            
            _castingState = CastingState.CastRecovery;
        }

        protected void FinishRecovery()
        {
            _castingState = CastingState.None;
            _casting = null;
            _castHandler = null;
        }
        
        protected void RemoveAbility(CastHandler abilityObject) => _activeAbilities.Remove(abilityObject);
        #endregion


        #region Methods
        /// <summary>
        /// Function to run in the Update of a MonoBehaviour
        /// Updates each ability current CD
        /// </summary>
        /// <param name="deltaTime">How much time elapsed since the last frame</param>
        public virtual void UpdateCooldowns(float deltaTime) => _cdHandler.Update(deltaTime);

        /// <summary>
        /// Updates ability cooldowns if CooldownUpdateType is set to auto and calls each active ability
        /// update function
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(float deltaTime)
        {
            if (CooldownUpdateType == CooldownUpdateType.Auto)
                UpdateCooldowns(deltaTime);
            
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].Update(deltaTime);
        }

        /// <summary>
        /// Calls every active ability OnDrawGizmos function
        /// </summary>
        public void OnDrawGizmos()
        {
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].DrawGizmos();
        }
    
        /// <summary>
        /// Sets the ability on the given index. Setting it to null will clear the ability slot
        /// </summary>
        /// <param name="slot">Slot to set the ability</param>
        /// <param name="ability">The ability that is being added</param>
        public void SetAbility(uint slot, TAbility ability)
        {
            _abilities[slot] = ability;
            _cdHandler.SetAbility(slot, ability);
        }

        /// <summary>
        /// Retrieves an ability from the abilities slots
        /// </summary>
        /// <param name="slot">slot index</param>
        /// <returns>The ability in the slot. Null if no spell in slot or slot have invalid index</returns>
        public TAbility GetAbility(uint slot) 
        {
            if(HasAbilityInSlot(slot))
                return _abilities[slot];
        
            return null;
        }

        /// <summary>
        /// Gets the ability that is currently being cast
        /// </summary>
        /// <returns>The ability being cast. Null if no ability is being cast</returns>
        public TAbility GetCastingAbility() => _casting;
        
        /// <summary>
        /// Gets the CastHandler for the current ability being cast.
        /// </summary>
        /// <returns>
        /// The cast handler for the current ability being cast or null if no ability is being cast.
        /// </returns>
        public CastHandler GetCastHandler() => _castHandler;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to check if there's an ability in the given slot
        /// </summary>
        /// <param name="slot">The slot index</param>
        /// <returns>True if there's an ability in the slot. False othwerwise</returns>
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;

        /// <summary>
        /// Tries to find an active CastHandler that was instantiated from a certain IAbilityBase 
        /// </summary>
        /// <param name="ability">The IAbilityBase to check</param>
        /// <returns>
        /// The cast handler if there's an active cast handler for the given ability. Null otherwise
        /// </returns>
        public CastHandler GetActiveHandler(IAbilityBase ability) => _activeAbilities.Find(x => x.Parent == ability);
        
        /// <summary>
        /// Helper method to provide a getter function to pass to the cast handler
        /// </summary>
        /// <returns>The Controller cast state</returns>
        private CastingState GetCastingState() => CastingState;
        #endregion
    }
}