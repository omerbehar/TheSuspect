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
    
    private void OnDeleteButtonClick()
    {
        CleanUpVideo();
    }

    public void OnButtonClick()
    {
        if(isPlayTest)
        {
            StopAndClearVideo();
            PickVideoDesktop();
        }
        else
        {
            StopAndClearVideo();
            PickVideoMobile();
        }
    }

    private void StopAndClearVideo()
    {
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        string oldVideoPath = Path.Combine(Application.persistentDataPath, storedVideoName);
        if(File.Exists(oldVideoPath))
        {
            File.Delete(oldVideoPath);
        }

        videoPlayer.clip = null;
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
        if(videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        string oldVideoPath = Path.Combine(Application.persistentDataPath, storedVideoName);
        if (File.Exists(oldVideoPath))
        {
            File.Delete(oldVideoPath);
        }

        if (PlayerPrefs.HasKey("capturedVideo"))
        {
            PlayerPrefs.DeleteKey("capturedVideo");
            PlayerPrefs.Save();
        }

       
        videoPlayer.clip = null;

        rawImageDisplay.gameObject.SetActive(false); 
        deleteButton.gameObject.SetActive(false); // set delete button inactive after cleaning up// Make the RawImage not visible when older video is removed
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
        string fileName = PlayerPrefs.GetString("capturedVideo");
        if (File.Exists(fileName))
        {
            PlayVideo("file://" + fileName);
        }
        else
        {
            rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible if fileName does not exist
        }
    }
}