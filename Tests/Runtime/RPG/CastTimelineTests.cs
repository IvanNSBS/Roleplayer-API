using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;
using System.Linq;

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
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Start();
            
            Assert.AreEqual(TimelineState.Running, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Properly_Updates_When_Running()
        {
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            
            Assert.AreEqual(1f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Properly_Pauses()
        {
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Start();
            _castTimeline.Pause();
            
            Assert.AreEqual(TimelineState.Paused, _castTimeline.state);
        }

        [Test]
        public void Timeline_Properly_Resets()
        {
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            _castTimeline.Reset();
            
            Assert.AreEqual(_castTimeline.ElapsedTime, 0f);
            Assert.AreEqual(TimelineState.Pending, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Paused()
        {
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Start();
            _castTimeline.Pause();
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Pending()
        {
            _castTimeline = new CastTimeline(1, 1);
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.ElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_Right_After_Reset()
        {
            _castTimeline = new CastTimeline(1, 1);
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
        public void Timeline_Properly_Fires_Channeling_Finished_and_Recovery_Started(float channelingTime)
        {
            int recovery = 0;
            int channeling = 0;
            _castTimeline = new CastTimeline(channelingTime, 1);
            _castTimeline.ChannelingFinished += () => channeling++;
            _castTimeline.RecoveryStarted += () => recovery++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(channelingTime * 0.01f);
            
            Assert.AreEqual(2, recovery + channeling);
        }
        
        [Test]
        [TestCase(0.4f, 0.7f)]
        [TestCase(1.2f, 1.456f)]
        [TestCase(3.1f, 0.2223f)]
        [TestCase(6.4f, 2.2f)]
        [TestCase(2.32f, 3.245f)]
        public void Timeline_Properly_Fires_Recovery_Finished(float channelingTime, float recoveryTime)
        {
            int fired = 0;
            _castTimeline = new CastTimeline(channelingTime, recoveryTime);
            _castTimeline.RecoveryFinished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);
            
            Assert.AreEqual(1, fired);
        }
        
        [Test]
        [TestCase(0.4f, 0.7f)]
        [TestCase(1.2f, 1.456f)]
        [TestCase(3.1f, 0.2223f)]
        [TestCase(6.4f, 2.2f)]
        [TestCase(2.32f, 3.245f)]
        public void Timeline_Properly_Finishes(float channelingTime, float recoveryTime)
        {
            _castTimeline = new CastTimeline(channelingTime, recoveryTime);
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(recoveryTime);
            
            Assert.AreEqual(TimelineState.Finished, _castTimeline.state);
        }
        #endregion
    }
}