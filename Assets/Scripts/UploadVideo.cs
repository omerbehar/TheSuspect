using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DataLayer;
using Screens.Bases;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class UploadVideo : ScreenBaseWithTimer
{
    [SerializeField] private bool isPlayTest = true;
    [SerializeField] private string storedVideoName = "myVideo.mp4";
    [SerializeField] private Button uploadButton;
    [SerializeField] private Button deleteButton; // add this line
    [SerializeField] private VideoPlayer hintVideoPlayer;
    //[SerializeField] private RawImage rawImageDisplay;
    private string serverURL = "https://icl.eitangames.co.il/uploadVideos.php"; 
    private static byte[] bytes;
    private static string _path;
    [SerializeField] private GameObject userVideoPlayer;
    [SerializeField] private VideoPlayer userVideoPlayerComponent;
    [SerializeField] private RawImage rawImage;
    private string videoPath = "https://icl.eitangames.co.il/userVideos/";
    //private static TMP_Text _debugText;
    [SerializeField] private TMP_Text debugText;
#if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void CaptureVideo(CaptureVideoCallback callback, CaptureVideoError errorCallback);
        delegate void CaptureVideoError(int errorData);
        delegate void CaptureVideoCallback(string videoData);
        [AOT.MonoPInvokeCallback(typeof(CaptureVideoCallback))]
        public static void OnVideoReceived(string videoData)
        {
            // Debug.Log("Video captured: " + url);
            // _path = url;
            // EventManager.VideoCaptured.Invoke();
            try
            {
                bytes = Convert.FromBase64String(videoData);
                //_debugText.text = $"video size: {bytes.Length}";
                Debug.Log($"video size: {bytes.Length}");
                // _path = Path.Combine(Application.persistentDataPath, "myVideo.mp4");
                // PlayerPrefs.SetString("capturedVideo", _path);
                // File.WriteAllBytes(_path, bytes);
                //
                if (EventManager.VideoCaptured != null)
                {
                    EventManager.VideoCaptured.Invoke();
                }
            }
            catch (FormatException e)
            {
                //_debugText.text = "Base64 string format is invalid: " + e.Message;
                Debug.LogError("Base64 string format is invalid: " + e.Message);
            }
            catch (Exception e)
            {
                //_debugText.text = "An error occurred: " + e.Message;
                Debug.LogError("An error occurred: " + e.Message);
            }
        }
        [AOT.MonoPInvokeCallback(typeof(CaptureVideoError))]
        public static void HandleError(int pointer)
        {
            string message = Marshal.PtrToStringAnsi(new IntPtr(pointer));
            //_debugText.text = "Error from JavaScript: " + message;
            Debug.LogError("Error from JavaScript: " + message);
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
        uploadButton.onClick.RemoveListener(RequestVideo);
        uploadButton.onClick.AddListener(OnPlayVideoCLicked);
#if UNITY_WEBGL && !UNITY_EDITOR
        CaptureVideo(OnVideoReceived, HandleError);
#else
        PickVideoDesktop();
        StartCoroutine(UploadVideoToServer());
#endif
    }

    private void OnPlayVideoCLicked()
    {
        userVideoPlayer.SetActive(true);
        userVideoPlayerComponent.url = videoPath;
    }

    IEnumerator UploadVideoToServer()
    {
        //go down a line and add this line
        string fileName = string.IsNullOrEmpty(Data.guid) ? "noGuid.mp4" : $"{Data.guid}.mp4";
        videoPath += fileName;
        List<IMultipartFormSection> form = new() { new MultipartFormFileSection("video", bytes, fileName, "video/mp4") };

        UnityWebRequest request = UnityWebRequest.Post(serverURL, form);


        yield return request.SendWebRequest();
        //debugText.text += $"\nServer Response: {request.downloadHandler.text}";
        Debug.Log($"Server Response: {request.downloadHandler.text}");

        if (request.result != UnityWebRequest.Result.Success)
        {
            //_debugText.text += $"\nError uploading video: {request.error}";
            Debug.LogError("Error uploading video: " + request.error);
        }
        else
        {
            //_debugText.text += "\nVideo upload successful!";
            Debug.Log("Video upload successful!");
        }
        EventManager.VideoUploaded.Invoke();
    }

    private void Awake()
    {
        EventManager.VideoCaptured.AddListener(OnVideoCaptured);
        EventManager.VideoUploaded.AddListener(OnVideoUploaded);
        //_debugText = debugText;
        //_debugText.text = "test";
        rawImage.gameObject.SetActive(false);
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

    private void OnVideoUploaded()
    {
        VideoPlayer videoPlayer = GetComponentInChildren<VideoPlayer>();
        videoPlayer.errorReceived += OnVideoPlayerError;

        videoPlayer.url = videoPath;
        Debug.Log("after url");
        videoPlayer.Prepare();
        Debug.Log("after prepare");
        StartCoroutine(WaitForVideoPrepared());
    }

    private void OnVideoPlayerError(VideoPlayer source, string message)
    {
        //_debugText.text += $"\nVideoPlayer Error: {message}";
        Debug.LogError($"VideoPlayer Error: {message}");
    }

    IEnumerator WaitForVideoPrepared()
    {
        VideoPlayer videoPlayer = GetComponentInChildren<VideoPlayer>();
        int maxRetries = 3;
        int retries = 0;

        while (!videoPlayer.isPrepared && retries < maxRetries)
        {
            videoPlayer.Prepare();
            yield return new WaitForSeconds(0.5f); // Wait 2 seconds before checking again
            retries++;
        }

        // Debug.Log("before while");
        // int i = 0;
        // int maxWait = 10;
        // while (!videoPlayer.isPrepared)
        // {
        //     i++;
        //     if(i > maxWait)
        //     {
        //         Debug.LogError("Max wait reached. Exiting loop.");
        //         yield break;
        //     }
        //     Debug.Log($"in while {i}");
        //     yield return new WaitForSeconds(0.5f);
        // }
        Debug.Log("after while");
        // Calculate aspect ratio
        //rawImageDisplay.texture = videoPlayer.texture;
        videoPlayer.Play();
        Debug.Log("after play");
        videoPlayer.Pause();
        Debug.Log("after pause");
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        Debug.Log("after aspect ratio");
        rawImage.gameObject.SetActive(true);
        Debug.Log("after set active");
    }

    private void OnVideoCaptured()
    {
        StartCoroutine(UploadVideoToServer());
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
        //OnButtonClick();
    }

    // Cleans up the video, and deactivates the deletion button

    // public void OnButtonClick()
    // {
    //     StartCoroutine(OnClickRoutine());
    // }

    // private IEnumerator OnClickRoutine()
    // {
    //     yield return StartCoroutine(StopAndClearVideo());
    //
    //     if (isPlayTest)
    //     {
    //         PickVideoDesktop();
    //     }
    //     else
    //     {
    //         PickVideoMobile();
    //     }
    // }

    private IEnumerator StopAndClearVideo()
    {
        if(hintVideoPlayer.isPlaying)
        {
            hintVideoPlayer.Stop();
        }
    
        hintVideoPlayer.clip = null;
    
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
            bytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, storedVideoName));
            //StartCoroutine(DelayPlayVideo("file://" + Path.Combine(Application.persistentDataPath, storedVideoName)));
        }
    }

    // public void PickVideoMobile()
    // {
    //     NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
    //     {
    //         if(path != null)
    //         {
    //             PlayerPrefs.SetString("capturedVideo", path);
    //             PlayerPrefs.Save();
    //             StartCoroutine(DelayPlayVideo("file://" + path));
    //         }
    //     }, "Select a video");
    //
    //     Debug.Log("Permission result: " + permission);
    // }
    //
    // private IEnumerator DelayPlayVideo(string videoUrl)
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     PlayVideo(videoUrl);
    // }

    private void PlayVideo(string videoUrl)
    {
        if(hintVideoPlayer == null)
            hintVideoPlayer = GetComponent<VideoPlayer>();
        hintVideoPlayer.enabled = true;
        hintVideoPlayer.gameObject.SetActive(true);
        Debug.Log("Playing video: " + videoUrl);
        hintVideoPlayer.url = videoUrl;
        hintVideoPlayer.prepareCompleted += Prepared;
        hintVideoPlayer.errorReceived += HandleError;

        hintVideoPlayer.Prepare();

        //rawImageDisplay.gameObject.SetActive(true);
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
        hintVideoPlayer.clip = null;

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