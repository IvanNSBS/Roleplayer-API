using NUnit.Framework;
using INUlib.RPG.AbilitiesSystem;
using UnityEngine;
using UnityEngine.UI;

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
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.TimelineStarted += () => fired++;
            _castTimeline.Start();
            
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Running, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Properly_Updates_When_Running()
        {
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            
            Assert.AreEqual(1f, _castTimeline.TotalElapsedTime);
        }
        
        [Test]
        public void Timeline_Properly_Pauses()
        {
            int fired = 0;
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.TimelinePaused += () => fired++;
            _castTimeline.Start();
            _castTimeline.Pause();
            
            Assert.AreEqual(1, fired);
            Assert.AreEqual(TimelineState.Paused, _castTimeline.state);
        }

        [Test]
        public void Timeline_Properly_Resets()
        {
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            _castTimeline.Reset();
            
            Assert.AreEqual(_castTimeline.TotalElapsedTime, 0f);
            Assert.AreEqual(TimelineState.Pending, _castTimeline.state);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Paused()
        {
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            _castTimeline.Pause();
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.TotalElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_If_Pending()
        {
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.TotalElapsedTime);
        }
        
        [Test]
        public void Timeline_Wont_Update_Right_After_Reset()
        {
            TimelineData data = new TimelineData(1, 1, 1, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            _castTimeline.Update(1f);
            _castTimeline.Reset();
            _castTimeline.Update(1f);
            Assert.AreEqual(0f, _castTimeline.TotalElapsedTime);
        }

        [Test]
        [TestCase(1.0f)]
        [TestCase(0.2f)]
        [TestCase(0.1f)]
        [TestCase(3.56f)]
        [TestCase(5.32f)]
        public void Timeline_Properly_Fires_Channeling_Finished_and_Overchannelling_Started(float channelingTime)
        {
            int calls = 0;
            TimelineData data = new TimelineData(channelingTime, 0f ,1f, 1f, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.ChannelingFinished_OverchannelingStarted += () => calls++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(channelingTime * 0.01f);
            
            Assert.AreEqual(1, calls);
        }
        
        [Test]
        [TestCase(1.0f, 0.45f)]
        [TestCase(0.2f, 1.375f)]
        [TestCase(0.1f, 0.2234f)]
        [TestCase(3.56f, 3.2f)]
        [TestCase(5.32f, 0.882f)]
        public void Timeline_Properly_Fires_Overchannelling_Finished_and_Cast_Started(float channelingTime, float overChannellingTime)
        {
            int calls = 0;
            TimelineData data = new TimelineData(channelingTime, overChannellingTime ,1f, 1f, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.OverchannelingFinished_CastingStarted += () => calls++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(overChannellingTime);
            _castTimeline.Update(overChannellingTime * 0.01f);
            
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
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, 1, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.CastFinished_ConcentrationStarted += () => fired++;
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
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, recoveryTime, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
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
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, recoveryTime, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
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
        [TestCase(0.4f, 0.5f, 1.32f)]
        [TestCase(1.2f, 1.05f, 0.538f)]
        [TestCase(3.1f, 2.43f, 2.79f)]
        [TestCase(6.4f, 0.334f, 0.5f)]
        [TestCase(2.32f, 1.785f, 0.773f)]
        public void Timeline_Finishes_After_Recovery_Time(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, recoveryTime, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            
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
        [TestCase(0.4f, 0.5f, 1.32f)]
        [TestCase(1.2f, 1.05f, 0.538f)]
        [TestCase(3.1f, 2.43f, 2.79f)]
        [TestCase(6.4f, 0.334f, 0.5f)]
        [TestCase(2.32f, 1.785f, 0.773f)]
        public void Timeline_Finishes_After_Recovery_Time_When_Concentration_Finishes(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, recoveryTime, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.FinishConcentration();
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
        public void Timeline_Wont_Finish_Before_Conditions_Are_Met_If_Is_Concentration(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, recoveryTime, 0f, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.ConcentrationFinished_RecoveryStarted += () => fired++;
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);
            _castTimeline.Update(999);

            Assert.AreEqual(0, fired);
        }
        
        [Test]
        [TestCase(0.4f, 0.432f)]
        [TestCase(1.2f, 1.559f)]
        [TestCase(3.1f, 2.987f)]
        [TestCase(6.4f, 0.875f)]
        public void Timeline_Finishes_Right_After_Cast_If_Is_Concentration_And_0_Recovery_Time(float channelingTime, float castTime)
        {
            int castFinishedFired = 0;
            int timelineFinishedFired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, castTime, 0, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.ConcentrationFinished_RecoveryStarted += () => castFinishedFired++;
            _castTimeline.Timeline_And_Recovery_Finished += () => timelineFinishedFired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.FinishConcentration();

            Assert.AreEqual(1, castFinishedFired, $"Concentration Finished expected to be called once but was called {castFinishedFired}");
            Assert.AreEqual(1, timelineFinishedFired, $"TimelineFinished expected to be called once but was called {timelineFinishedFired}");
        }
        
        [Test]
        [TestCase(0.4f, 1.32f)]
        [TestCase(1.2f, 0.538f)]
        [TestCase(3.1f, 2.79f)]
        [TestCase(6.4f, 0.5f)]
        [TestCase(2.32f, 0.773f)]
        public void Timeline_Wont_Finish_Until_After_Recovery_If_Is_Concentration(float channelingTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, 1f, recoveryTime, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.CastFinished_ConcentrationStarted += () => fired++;
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(1f);
            
            _castTimeline.FinishConcentration();
            Assert.AreEqual(1, fired, $"Expected CastFinished_Recovery_Started to be fired but it wasn't");
            
            _castTimeline.Update(recoveryTime);
            
            Assert.AreEqual(2, fired, "TimelineFinished callback was not called.");
            Assert.AreEqual(
                TimelineState.Finished, 
                _castTimeline.state, 
                $"Timeline State was Incorrect: {_castTimeline.state}"
            );
        }
        
        [Test]
        [TestCase(0.4f, 1.32f)]
        [TestCase(1.2f, 0.538f)]
        [TestCase(3.1f, 2.79f)]
        [TestCase(6.4f, 0.5f)]
        [TestCase(2.32f, 0.773f)]
        public void Timeline_Wont_Repeat_TimelineFinished_Callback_When_Is_Concentration_Type(float channelingTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, 1f, recoveryTime, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.Timeline_And_Recovery_Finished += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(1f);
            _castTimeline.FinishConcentration();
            
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);

            _castTimeline.FinishConcentration();
            
            Assert.AreEqual(1, fired);
        }
        
        [Test]
        [TestCase(0.4f, 0.05f, 1.32f)]
        [TestCase(1.2f, 1.34f, 0.538f)]
        [TestCase(3.1f, 2.223f, 2.79f)]
        [TestCase(6.4f, 0.72f, 0.5f)]
        [TestCase(2.32f, 0.994f, 0.773f)]
        public void Timeline_Wont_Repeat_CastFinished_Callback_When_Is_Concentration_Type(float channelingTime, float castTime, float recoveryTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(channelingTime, 0f, 1f, recoveryTime, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.CastFinished_ConcentrationStarted += () => fired++;
            _castTimeline.Start();
            _castTimeline.Update(channelingTime);
            _castTimeline.Update(castTime);
            _castTimeline.FinishConcentration();
            
            _castTimeline.Update(recoveryTime);
            _castTimeline.Update(recoveryTime * 0.01f);

            _castTimeline.FinishConcentration();
            
            Assert.AreEqual(1, fired);
        }

        [Test]
        [TestCase(CastingState.Channeling)]
        [TestCase(CastingState.OverChanneling)]
        [TestCase(CastingState.Casting)]
        [TestCase(CastingState.Concentrating)]
        [TestCase(CastingState.CastRecovery)]
        public void Timeline_Properly_Updates_Clbk_State(CastingState expected)
        {
            TimelineData data = new TimelineData(1f, 1f, 1f, 1f, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();

            if (expected == CastingState.Channeling)
            {
                _castTimeline.Update(0f);
            }

            if (expected == CastingState.OverChanneling)
            {
                _castTimeline.Update(1f);
            }
            if (expected == CastingState.Casting)
            {
                _castTimeline.Update(1f);
                _castTimeline.Update(1f);
            }
            if (expected == CastingState.Concentrating)
            {
                _castTimeline.Update(1f);
                _castTimeline.Update(1f);
                _castTimeline.Update(1f);
            }
            if (expected == CastingState.CastRecovery)
            {
                _castTimeline.Update(1f);
                _castTimeline.Update(1f);
                _castTimeline.Update(1f);
                _castTimeline.FinishConcentration();
            }
            
            Assert.AreEqual(expected, _castTimeline.clbkState);
        }

        [Test]
        [TestCase(0.540f)]
        [TestCase(1.350f)]
        [TestCase(2.289f)]
        [TestCase(0.223f)]
        [TestCase(100f)]
        [TestCase(-100f)]
        public void Timeline_Properly_Fires_Unleash_Spell_Event(float unleashDuringCastTime)
        {
            int fired = 0;
            TimelineData data = new TimelineData(1f, 2f, 3f, 1f, unleashDuringCastTime, AbilityCastType.FireAndForget);
            unleashDuringCastTime = data.unleashDuringCastTime;
            
            _castTimeline = new CastTimeline(data);
            _castTimeline.UnleashAbility += () => fired++;
            
            _castTimeline.Start();
            
            _castTimeline.Update(1f); 
            _castTimeline.Update(2f); 
            _castTimeline.Update(unleashDuringCastTime);
            _castTimeline.Update(3f - unleashDuringCastTime); 
            _castTimeline.Update(1f); 
            
            Assert.AreEqual(1, fired, $"Expected Unleash ability to be called once but it was called {fired}");
        }

        [Test]
        public void Concentration_Spell_With_Zero_Cast_Time_Wont_Skip_Concentration_When_Going_To_Next_Step_In_The_Same_Frame()
        {
            TimelineData data = new (2f, 1f, 0f, 3f, 0, AbilityCastType.Concentration);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.Start();
            
            _castTimeline.Update(2f); 
            _castTimeline.Update(1f); 
            _castTimeline.Update(1.2f);
            
            Assert.AreEqual(CastingState.Concentrating, _castTimeline.clbkState);
        }

        [Test]
        public void Overchanneling_Is_Properly_Skipped_If_Set_To_Skip_Overchanneling()
        {
            TimelineData data = new (2f, 100f, 1f, 3f, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            _castTimeline.SkipOverchanneling(true);
            
            _castTimeline.Update(2f); 
            
            Assert.AreEqual(CastingState.Casting, _castTimeline.clbkState);
        }

        [Test]
        public void Overchanneling_Is_Ended_Prematurely_When_Skipped()
        {
            TimelineData data = new (2f, 100f, 1f, 3f, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            
            _castTimeline.Update(2f);
            _castTimeline.Update(0.1f);
            _castTimeline.SkipOverchanneling(true);
            
            Assert.AreEqual(CastingState.Casting, _castTimeline.clbkState);
        }

        [Test]
        public void Timeline_Properly_Jumps_To_Recovery_Start()
        {
            TimelineData data = new (2f, 100f, 1f, 3f, 0, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.Start();
            
            _castTimeline.Update(0.01f);
            _castTimeline.JumpToStartRecoveryState();
            
            Assert.AreEqual(CastingState.CastRecovery, _castTimeline.clbkState);
            Assert.AreEqual(0f, _castTimeline.CurrentStateElapsedTime);
        }
        
        [Test]
        public void Unleash_Callbacks_Are_Fired_In_Order()
        {
            int x = 1;
            
            TimelineData data = new (0f, 0f, 0f, 3f, 0.1f, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.UnleashAbility += () => x = 2;
            _castTimeline.UnleashAbility += () => x *= 5;
            
            _castTimeline.Start();
            _castTimeline.Update(0);
            
            
            Assert.AreEqual(
                10, x, 
                "Expected value is 10. If value was 2 then it fired in the wrong order. If it was 1, no event was fired"
            );
        }

        [Test]
        public void Timeline_ElapsedTime_Is_Perfectly_Linked_With_CurrentStateElapsedTime()
        {
            float channelingTime = 1f;
            float overChannelingTime = 2f;
            TimelineData data = new (channelingTime, overChannelingTime, 0, 0, 0.1f, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            
            _castTimeline.Start();
            _castTimeline.Update(channelingTime + overChannelingTime*0.5f);

            float expected = _castTimeline.TotalElapsedTime;
            float currentState = _castTimeline.CurrentStateElapsedTime;
            Assert.AreEqual(expected,  currentState + channelingTime);
        }

        [Test]
        public void Timeline_Will_Fire_All_Callbacks_And_Finish_With_A_Single_Update_If_DeltaTime_Is_High_Enough_For_FireAnd_Forget_Abilities()
        {
            float times = 1f;
            int x = 0;
            
            TimelineData data = new (times, times, times, times, 0.1f, AbilityCastType.FireAndForget);
            _castTimeline = new CastTimeline(data);
            _castTimeline.ChannelingFinished_OverchannelingStarted += () => x++;
            _castTimeline.OverchannelingFinished_CastingStarted += () => x++;
            _castTimeline.UnleashAbility += () => x++;
            _castTimeline.CastFinished_ConcentrationStarted += () => x++;
            _castTimeline.Timeline_And_Recovery_Finished += () => x++;
            
            _castTimeline.Start();
            _castTimeline.Update(times * 5);
            
            Assert.AreEqual(5, x, "Not all events were fired during the single update");
        }
        #endregion
    }
}