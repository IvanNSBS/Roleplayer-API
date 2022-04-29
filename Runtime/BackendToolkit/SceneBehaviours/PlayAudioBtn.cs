using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using INUlib.BackendToolkit.Audio;

namespace INUlib.BackendToolkit.SceneBehaviours
{
    /// <summary>
    /// Plays an Audio on click
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PlayAudioBtn : SceneBehaviour, IPointerEnterHandler
    {
        #region Inspector Fields
        [SerializeField] private string _clickSoundId;
        [SerializeField] private string _hoverSoundId;
        #endregion

        #region Fields
        [Locate] private AudioController _audioCtrl;
        private Button _btn;
        #endregion

        #region Properties
        public string ClickSoundId => _clickSoundId;
        public string HoverSoundId => _hoverSoundId;
        #endregion


        #region Methods
        protected override void Awake()
        {
            base.Awake();

            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(() => {
                if(!string.IsNullOrEmpty(_clickSoundId) && _btn.interactable)
                    _audioCtrl.PlaySound(_clickSoundId, true);
            });
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if(!string.IsNullOrEmpty(_hoverSoundId) && _btn.interactable)
                _audioCtrl.PlaySound(_hoverSoundId, true);
        }
        #endregion

    }
}