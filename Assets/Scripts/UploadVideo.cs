using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Video;
using NativeGalleryNamespace;

public class UploadVideo : MonoBehaviour
{
    [SerializeField] private bool isPlayTest = true;
    [SerializeField] private string storedVideoName = "myVideo.mp4";
    [SerializeField] private Button uploadButton;
    [SerializeField] private Button deleteButton; // add this line
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImageDisplay;

    private void Awake()
    {
        rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible at the start
        uploadButton.onClick.AddListener(OnButtonClick);
        
        deleteButton.onClick.AddListener(OnDeleteButtonClick); // add this line
        deleteButton.gameObject.SetActive(false); // Initially the delete button is hidden
    }
    private IEnumerator Start()
    {
        deleteButton.interactable = false;
        yield return StartCoroutine(StopAndClearVideo());
        deleteButton.interactable = true;
    }
    
    // Handles the delete button click
    private void OnDeleteButtonClick()
    {
        StartCoroutine(OnDeleteButtonClickRoutine());
    }
    private IEnumerator OnDeleteButtonClickRoutine()
    {
        yield return StartCoroutine(StopAndClearVideo());
        OnButtonClick();
    }

    // Cleans up the video, and deactivates the deletion button

    public void OnButtonClick()
    {
        StartCoroutine(OnClickRoutine());
    }

    private IEnumerator OnClickRoutine()
    {
        yield return StartCoroutine(StopAndClearVideo());

        if (isPlayTest)
        {
            PickVideoDesktop();
        }
        else
        {
            PickVideoMobile();
        }
    }

    private IEnumerator StopAndClearVideo()
    {
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
    
        videoPlayer.clip = null;
    
        string oldVideoPath = Path.Combine(Application.persistentDataPath, storedVideoName);
        if (File.Exists(oldVideoPath))
        {
            File.Delete(oldVideoPath);
            while (File.Exists(oldVideoPath))
            {
                // the deletion hasn't hit the disk yet, retry next frame
                yield return null;
            }
        }
    }

    public void PickVideoDesktop()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", "*", false);
        if(paths.Length > 0)
        {
            File.Copy(paths[0], Path.Combine(Application.persistentDataPath, storedVideoName), true);
            PlayerPrefs.SetString("capturedVideo", storedVideoName);
            PlayerPrefs.Save();
            StartCoroutine(DelayPlayVideo("file://" + Path.Combine(Application.persistentDataPath, storedVideoName)));
        }
    }

    public void PickVideoMobile()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            if(path != null)
            {
                PlayerPrefs.SetString("capturedVideo", path);
                PlayerPrefs.Save();
                StartCoroutine(DelayPlayVideo("file://" + path));
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }

    private IEnumerator DelayPlayVideo(string videoUrl)
    {
        yield return new WaitForSeconds(0.1f);
        PlayVideo(videoUrl);
    }

    private void PlayVideo(string videoUrl)
    {
        if(videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
        
        videoPlayer.url = videoUrl;
        videoPlayer.Play();

        rawImageDisplay.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true); // set delete button active when video is loaded// Make the RawImage visible when video is ready to play
    }

    public void CleanUpVideo()
    {
        deleteButton.interactable = false; // Disable delete button during cleanup

        StartCoroutine(StopAndClearVideo());

        if (PlayerPrefs.HasKey("capturedVideo"))
        {
            PlayerPrefs.DeleteKey("capturedVideo");
            PlayerPrefs.Save();
        }

        rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible when older video is removed

        deleteButton.interactable = true;  // Re-enable delete button after cleanup
    }
    
    private void RemoveVideo()
    {
        string fileName = PlayerPrefs.GetString("capturedVideo");
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        PlayerPrefs.DeleteKey("capturedVideo");
        videoPlayer.clip = null;

        rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible when video file is removed
    }

    private void LoadVideo()
    {
        string fileName = Path.Combine(Application.persistentDataPath, PlayerPrefs.GetString("capturedVideo"));
        if (File.Exists(fileName))
        {
            PlayVideo("file://" + fileName);
        }
    }
    
    private IEnumerator WaitForFileToDisappear(string path)
    {
        while (File.Exists(path))
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}