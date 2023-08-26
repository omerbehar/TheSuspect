using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Screens.Bases;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Networking;
using UnityEngine.Video;

public class UploadVideo : ScreenBaseWithTimer
{
    [SerializeField] private bool isPlayTest = true;
    [SerializeField] private string storedVideoName = "myVideo.mp4";
    [SerializeField] private Button uploadButton;
    [SerializeField] private Button deleteButton; // add this line
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImageDisplay;

    private static string _path;
#if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void CaptureVideo(CaptureVideoCallback callback);
    
        delegate void CaptureVideoCallback(string videoData);
        [AOT.MonoPInvokeCallback(typeof(CaptureVideoCallback))]
        public static void OnVideoReceived(string videoData)
        {
            // Debug.Log("Video captured: " + url);
            // _path = url;
            // EventManager.VideoCaptured.Invoke();
            try
            {
                byte[] bytes = Convert.FromBase64String(videoData);
                Debug.Log($"video size: {bytes.Length}");
                _path = Path.Combine(Application.persistentDataPath, "myVideo.mp4");
                PlayerPrefs.SetString("capturedVideo", _path);
                File.WriteAllBytes(_path, bytes);
            
                if (EventManager.VideoCaptured != null)
                {
                    EventManager.VideoCaptured.Invoke();
                }
            }
            catch (FormatException e)
            {
                Debug.LogError("Base64 string format is invalid: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred: " + e.Message);
            }
        }
#endif
    // IEnumerator LoadVideoFromURL(string url)
    // {
    //     UnityWebRequest www = UnityWebRequest.Get(url);
    //     DownloadHandlerFile downloadVideo = new DownloadHandlerFile(url);
    //     //DownloadHandlerVideoClip downloadVideo = new DownloadHandlerVideoClip(url);
    //     www.downloadHandler = downloadVideo;
    //
    //     yield return www.SendWebRequest();
    //
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //     }
    //     else
    //     {
    //         VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
    //         videoPlayer.clip = downloadVideo.GetContent(www);
    //     }
    // }

    public void RequestVideo()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CaptureVideo(OnVideoReceived);
#else
            
        PickVideoDesktop();
#endif
    }

    private void Awake()
    {
        EventManager.VideoCaptured.AddListener(OnVideoCaptured);
        //rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible at the start
        uploadButton.onClick.AddListener(RequestVideo);
        
        deleteButton.onClick.AddListener(OnDeleteButtonClick); // add this line
        deleteButton.gameObject.SetActive(false); // Initially the delete button is hidden
        // videoPlayer.url = "blob:http://localhost:65490/574a03ed-0663-412e-ae99-069ffb71e4e9.mp4";
        // Debug.Log("Video URL: " + videoPlayer.url);
        // videoPlayer.enabled = true;
        // //videoPlayer.gameObject.SetActive(true);
        // videoPlayer.prepareCompleted += Prepared;
        // videoPlayer.Prepare();
        //videoPlayer.Play();
    }

    private void OnVideoCaptured()
    {
        StartCoroutine(DelayPlayVideo("file://" + _path));
    }

    private IEnumerator Start()
    {
        base.Start();
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
        videoPlayer.enabled = true;
        videoPlayer.gameObject.SetActive(true);
        Debug.Log("Playing video: " + videoUrl);
        videoPlayer.url = videoUrl;
        videoPlayer.prepareCompleted += Prepared;
        videoPlayer.errorReceived += HandleError;

        videoPlayer.Prepare();

        rawImageDisplay.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true); // set delete button active when video is loaded// Make the RawImage visible when video is ready to play
    }

    public void Prepared(VideoPlayer vp)
    {
        vp.Play();
    }
    void HandleError(VideoPlayer vp, string errorMessage)
    {
        Debug.LogError("Error: " + errorMessage);
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

        //rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible when older video is removed

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

        //rawImageDisplay.gameObject.SetActive(false); // Make the RawImage not visible when video file is removed
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