using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Video;
using NativeGalleryNamespace;

public class UploadVideo : MonoBehaviour
{
    [SerializeField] bool isPlayTest = true;
    [SerializeField] private string storedVideoName = "myVideo.mp4";
    [SerializeField] private Button uploadButton;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImageDisplay;

    private void Awake()
    {
        uploadButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if(isPlayTest)
            PickVideoDesktop();
        else
            PickVideoMobile();
    }

    public void PickVideoDesktop()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", "*", false);
        if (paths.Length > 0)
        {
            File.Copy(paths[0], Path.Combine(Application.persistentDataPath, storedVideoName), true);
            PlayerPrefs.SetString("capturedVideo", storedVideoName);
            PlayerPrefs.Save();
            PlayVideo("file://" + Path.Combine(Application.persistentDataPath, storedVideoName));
        }
    }

    public void PickVideoMobile()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            if (path != null)
            {
                PlayerPrefs.SetString("capturedVideo", path);
                PlayerPrefs.Save();
                PlayVideo("file:///" + path);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }

    private void PlayVideo(string videoUrl)
    {
        if(videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
        
        videoPlayer.url = videoUrl;
        videoPlayer.Play();
    }

    private void RemoveVideo()
    {
        string fileName = PlayerPrefs.GetString("capturedVideo");
        PlayerPrefs.DeleteKey("capturedVideo");
        PlayerPrefs.Save();

        File.Delete(fileName);
    }

    private void LoadVideo()
    {
        string fileName = PlayerPrefs.GetString("capturedVideo");
        if (File.Exists(fileName))
        {
            PlayVideo("file:///" + fileName);
        }
    }
}