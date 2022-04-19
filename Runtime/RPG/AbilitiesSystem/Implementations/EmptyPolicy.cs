namespace INUlib.RPG.AbilitiesSystem
{
    public class EmptyPolicy : CastHandlerPolicy
    {
        public override void OnCastRequested(int requestAmount, CastingState state) { }
        public override void OnCancelRequested(CastingState state) { }
        protected override void OnUpdate() { }
    }
}