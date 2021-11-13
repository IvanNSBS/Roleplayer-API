using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Audio.Settings
{
    /// <summary>
    /// Class responsible for controlling a toggle and a slider linked to a Unity AudioMixer channel
    /// </summary>
    public class GroupSettingsView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private Toggle m_toggleMute;
        [SerializeField] private Slider m_volumeSlider;
        #endregion Inspector Fields
        
        #region Fields
        private string m_groupName;
        private AudioMixerController m_controller;
        #endregion Fields
        
        
        #region Methods
        public void SetGroupSettings(AudioMixerController controller, string groupName)
        {
            m_groupName = groupName;
            m_controller = controller;
            
            if(groupName == null || controller == null) 
                RemoveListeners();
            else
                AddListeners();
        }
        #endregion Methods
        
        
        #region Helper Methods
        private void RemoveListeners()
        {
            m_toggleMute.onValueChanged.RemoveAllListeners();
            m_volumeSlider.onValueChanged.RemoveAllListeners();
        }

        private void AddListeners()
        {
            m_toggleMute.isOn = m_controller.GetGroupData(m_groupName).Mute;
            m_volumeSlider.value = m_controller.GetGroupData(m_groupName).Volume;
            
            m_toggleMute.onValueChanged.AddListener( (val) => m_controller.SetMute(m_groupName, val));
            m_volumeSlider.onValueChanged.AddListener( (val) => m_controller.SetVolume(m_groupName, val));
        }
        #endregion Helper Methods
    }
}