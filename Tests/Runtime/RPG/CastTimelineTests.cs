using NUnit.Framework;
using INUlib.RPG.AbilitiesSystem;

namespace Tests.Runtime.RPG
{
    public class CastTimelineTests
    {
        #region Fields
        private CastTimeline _castTimeline;
        #endregion
        
        #region Setup
        [SetUp]
        public void Setup()
        {
        }
        #endregion
        
        
        #region Methods
        [Test]
        public void Timeline_Properly_Starts()
        {
            int fired = 0;
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.TimelineStarted += () => fired++;
            _castTimeline.Start();
            
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Running, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Properly_Updates_When_Running()
        {
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            
            Assert.AreEqual(1f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Properly_Pauses()
        {
            int fired = 0;
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.TimelinePaused += () => fired++;
            _castTimeline.Start();
            _castTimeline.Pause();
            
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Paused, _castTimeline.state);
        }

        [Test]
        public void Timeline_Properly_Resets()
        {
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            _castTimeline.Reset();
            
            Assert.AreEqual(_castTimeline.ElapsedTime, 0f);
            Assert.AreEqual(TimelineState.Pending, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Paused()
        {
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.Start();
            _castTimeline.Pause();
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Pending()
        {
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_Right_After_Reset()
        {
            _castTimeline = new CastTimeline(1, 1, 1, AbilityCastType.FireAndForget);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            _castTimeline.Reset();
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.ElapsedTime);
        }

        [Test]
        [TestCase(1.0f)]
        [TestCase(0.2f)]
        [TestCase(0.1f)]
        [TestCase(3.56f)]
        [TestCase(5.32f)]
        public void Timeline_Properly_Fires_Channeling_Finished_and_Cast_Started(float channelingTime)
        {
            int calls = 0;
            _castTimeline = new CastTimeline(channelingTime, 1f, 1f, AbilityCastType.FireAndForget);
            _castTimeline.ChannelingFinished_CastStarted += () => calls++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(channelingTime * 0.01f);
            
            Assert.AreEqual(1, calls);
        }

        [Test]
        [TestCase(0.4f, 0.7f)]
        [TestCase(1.2f, 1.456f)]
        [TestCase(3.1f, 0.2223f)]
        [TestCase(6.4f, 2.2f)]
        [TestCase(2.32f, 3.245f)]
        public void Timeline_Properly_Fires_Cast_Finished_Recovery_Started(float channelingTime, float castTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, castTime, 1, AbilityCastType.FireAndForget);
            _castTimeline.CastFinished_RecoveryStarted += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(castTime * 0.01f);
            
            Assert.AreEqual(1, fired);
        }
        
        [Test]
        [TestCase(0.4f, 0.7f, 1.32f)]
        [TestCase(1.2f, 1.456f, 0.538f)]
        [TestCase(3.1f, 0.2223f, 2.79f)]
        [TestCase(6.4f, 2.2f, 0.5f)]
        [TestCase(2.32f, 3.245f, 0.773f)]
        public void Timeline_Properly_Fires_Timeline_And_Recovery_Finished(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, castTime, recoveryTime, AbilityCastType.FireAndForget);
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);
            
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Finished, _castTimeline.state);
        }

        [Test]
        [TestCase(0.4f, 0.7f, 1.32f)]
        [TestCase(1.2f, 1.456f, 0.538f)]
        [TestCase(3.1f, 0.2223f, 2.79f)]
        [TestCase(6.4f, 2.2f, 0.5f)]
        [TestCase(2.32f, 3.245f, 0.773f)]
        public void Timeline_Wont_Finish_After_Cast_If_It_Is_Concentration_Type(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, castTime, recoveryTime, AbilityCastType.Concentration);
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);
            
            Assert.AreEqual(0, fired);
            Assert.AreEqual(TimelineState.Running, _castTimeline.state);
        }

        
        [Test]
        [TestCase(0.4f, 0.7f, 1.32f)]
        [TestCase(1.2f, 1.456f, 0.538f)]
        [TestCase(3.1f, 0.2223f, 2.79f)]
        [TestCase(6.4f, 2.2f, 0.5f)]
        [TestCase(2.32f, 3.245f, 0.773f)]
        public void Timeline_Finishes_After_Finish_Callback_When_Is_Concentration_Type(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, 1f, 1f, AbilityCastType.Concentration);
            
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);

            _castTimeline.FinishTimeline();
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Finished, _castTimeline.state);
        }
        
        [Test]
        [TestCase(0.4f, 0.7f, 1.32f)]
        [TestCase(1.2f, 1.456f, 0.538f)]
        [TestCase(3.1f, 0.2223f, 2.79f)]
        [TestCase(6.4f, 2.2f, 0.5f)]
        [TestCase(2.32f, 3.245f, 0.773f)]
        public void Timeline_Wont_Repeat_Finish_Callback_When_Is_Concentration_Type(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, 1f, 1f, AbilityCastType.Concentration);
            
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);

            _castTimeline.FinishTimeline();
            _castTimeline.FinishTimeline();
            Assert.AreEqual(1, fired);
        }
        #endregion
    }
}