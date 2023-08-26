using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Screens
{
    public class WatchVideo : MonoBehaviour
    {
        [SerializeField] private Button watchVideoButton;
        [SerializeField] private GameObject videoPopup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playButton;
        [SerializeField] private VideoPlayer videoPlayer;
        private void Awake()
        {
            watchVideoButton.onClick.AddListener(OnWatchVideoButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            closeButton.onClick.Invoke();
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            playButton.gameObject.SetActive(false);
            videoPlayer.Play();
        }

        private void OnCloseButtonClicked()
        {
            Debug.Log("Close button clicked");
            videoPopup.SetActive(false);
        }

        private void OnWatchVideoButtonClicked()
        {
            videoPopup.SetActive(true);
        }
    }
}