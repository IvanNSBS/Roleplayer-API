namespace INUlib.Serialization.ManagedTweens
{
    /// <summary>
    /// Interface for a ManagedSequence target.
    /// The target is what needs to be reset between each
    /// ManagedSequence play
    /// </summary>
    public interface IManagedTarget
    {
        /// <summary>
        /// Function that resets the target to the default state.
        /// The default state is the state before any animation happens
        /// /// </summary>
        void Reset();
    }
}