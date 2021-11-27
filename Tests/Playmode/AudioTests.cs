using System.Collections;
using System.Collections.Generic;
using INUlib.Core.Audio;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioTests
{
    #region Test Classes
    public class TestAudioController : AudioController
    {
        public TestAudioController(GameTracks t) : base(t)
        {
            
        }

        protected override AudioSource CreateSound(AudioData data)
        {
            GameObject go = new GameObject($"Sound_{data.Id}");
            return go.AddComponent<AudioSource>();
        }
    }

    public class TestAudioTracks : AudioTrack
    {
        public AudioData dataRef;
        public override IReadOnlyList<AudioData> Audios => new List<AudioData> {dataRef};
    }
    
    public class TestGameTracks : GameTracks
    {
        public AudioTrack audioTrackRef;
        public override AudioTrack BackgroundMusics => audioTrackRef;
        public override IReadOnlyList<AudioTrack> Collections => new List<AudioTrack> { audioTrackRef };
    }
    #endregion Test Classes
    
    
    #region Helper Methods

    public AudioController CreateTestAudioController()
    {
        var data = Substitute.For<AudioData>();
        data.Id.Returns("dummy");
        data.Loop.Returns(false);

        var audios = ScriptableObject.CreateInstance<TestAudioTracks>();
        audios.dataRef = data;

        var tracks = ScriptableObject.CreateInstance<TestGameTracks>();
        tracks.audioTrackRef = audios;

        return new TestAudioController(tracks);
    }
    #endregion Helper Methods
    
    
    #region Tests
    [UnityTest]
    public IEnumerator SoundPlayedIsInstantiated()
    {
        var controller = CreateTestAudioController();
        controller.PlaySound("dummy");
        
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(GameObject.Find("Sound_dummy") != null);
    }
    
    [UnityTest]
    public IEnumerator BGMIsInstantiated()
    {
        var controller = CreateTestAudioController();
        controller.PlayBackgroundMusic("dummy");
        
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(GameObject.Find("Sound_dummy") != null);
    }
    #endregion Tests
}
