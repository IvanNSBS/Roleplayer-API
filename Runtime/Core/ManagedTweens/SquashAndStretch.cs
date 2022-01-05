
using DG.Tweening;
using UnityEngine;

namespace INUlib.Core.ManagedTweens
{
    /// <summary>
    /// Managed Sequence that implements a Squash and Stretch
    /// Animation
    /// </summary>
    public class SquashAndStretch : ManagedSequence
    {
        #region Fields
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a Squash and Stretch animation from a ManagedTransform.
        /// How much energy the squash and stretch will lose will be calculated
        /// from the (initial) strength and bounces parameter
        /// </summary>
        /// <param name="tgt">Squash and Stretch Transform from ManagedTransform</param>
        /// <param name="bounces">How many bounces. A bounce is squash+stretch</param>
        /// <param name="strength">How strong the first bounce is.</param>
        /// <param name="falloff">How much strength, in percent, is lost with each squash or stretch</param>
        /// <param name="overshoot">Outback Ease overshoot</param>
        /// <param name="stretchFirst">Whether or not to stretch or squash first</param>
        public SquashAndStretch(ManagedTransform tgt, uint bounces, float bounceDuration, float strength, 
                                float falloff=0.1f, bool stretchFirst=false, Ease ease=Ease.OutBack,
                                float overshoot=2.3f):base(tgt)
        {
            Initialize(tgt.Transform, bounces, bounceDuration, strength, falloff, stretchFirst, ease, overshoot);
        }

        /// <summary>
        /// Creates a Squash and Stretch animation.
        /// How much energy the squash and stretch will lose will be calculated
        /// from the (initial) strength and bounces parameter
        /// </summary>
        /// <param name="tgt">Squash and Stretch Target Transform</param>
        /// <param name="bounces">How many bounces. A bounce is squash+stretch</param>
        /// <param name="strength">How strong the first bounce is.</param>
        /// <param name="falloff">How much strength, in percent, is lost with each squash or stretch</param>
        /// <param name="overshoot">Outback Ease overshoot</param>
        /// <param name="stretchFirst">Whether or not to stretch or squash first</param>
        public SquashAndStretch(Transform tgt, uint bounces, float bounceDuration, float strength, 
                                float falloff=0.1f, bool stretchFirst=false, Ease ease=Ease.OutBack,
                                float overshoot=2.3f):base(tgt)
        {
            Initialize(tgt, bounces, bounceDuration, strength, falloff, stretchFirst, ease, overshoot);
        }
        #endregion


        #region Helper Methods
        private void Initialize(Transform tgt, uint bounces, float bounceDuration, float strength, 
                                float falloff, bool stretchFirst, Ease ease, float overshoot)
        {
            float currentStr = strength;
            for(int i = 0; i < bounces*2; i++)
            {
                Vector3 val;
                bool order = stretchFirst ? i%2 != 0 : i%2 == 0;

                if(order)
                    val = new Vector3(1 + currentStr, 1 - currentStr, 1);
                else
                    val = new Vector3(1 - currentStr, 1 + currentStr, 1);

                Append(tgt.DOScale(val, bounceDuration*0.5f).SetEase(ease, overshoot));
                currentStr *= 1-falloff;
            }

            Append(tgt.DOScale(Vector3.one, bounceDuration*0.5f).SetEase(ease, overshoot));
        }
        #endregion
    }
}