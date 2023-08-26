using UnityEngine;
using UnityEngine.UI;

namespace Screens.Bases
{
    public class Screen2_3 : ScreenBase
    {
        [SerializeField] private Button mapButton;
        [SerializeField] private Button carButton;
        [SerializeField] private GameObject mapPopup;
        [SerializeField] private GameObject carPopup;
        [SerializeField] private Button mapCloseButton;
        [SerializeField] private Button carCloseButton;
        protected override void Start()
        {
            base.Start();
            NextButton.interactable = true;
            InitButtonListeners();
        }

        private void InitButtonListeners()
        {
            mapButton.onClick.AddListener(OnMapButtonClicked);
            carButton.onClick.AddListener(OnCarButtonClicked);
            mapCloseButton.onClick.AddListener(OnMapCloseButtonClicked);
            carCloseButton.onClick.AddListener(OnCarCloseButtonClicked);
        }

        private void OnCarCloseButtonClicked()
        {
            carPopup.SetActive(false);
        }

        private void OnMapCloseButtonClicked()
        {
            mapPopup.SetActive(false);
        }

        private void OnCarButtonClicked()
        {
            carPopup.SetActive(true);
        }

        private void OnMapButtonClicked()
        {
            mapPopup.SetActive(true);
        }
    }
}