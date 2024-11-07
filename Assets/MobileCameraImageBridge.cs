using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MobileCameraImageBridge : MonoBehaviour
{
    [SerializeField] private Button deleteButton;

    [SerializeField] private GameObject popupGameObject;
    //private static string debugText;
    //[SerializeField] private TMP_Text debugTextObject;
    private static bool wasImageLoaded;
    private static int orientation;

 #if UNITY_WEBGL && !UNITY_EDITOR
    
    [DllImport("__Internal")]
    private static extern void OpenCamera(MobileCameraCallback callback, OrientationCallback orientationCallback);

    delegate void MobileCameraCallback(string imageData);
    delegate void OrientationCallback(int orientation);

    [AOT.MonoPInvokeCallback(typeof(MobileCameraCallback))]
    public static void OnImageReceived(string imageData)
    {
       //debugText = "Received image: " + imageData.Length;
        byte[] bytes = Convert.FromBase64String(imageData);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        UploadImage uploadImage = FindObjectOfType<UploadImage>();
        wasImageLoaded = true;
        uploadImage.DisplayImage(texture, orientation);
    }
    [AOT.MonoPInvokeCallback(typeof(OrientationCallback))]
    public static void OnOrientationReceived(int picOrientation)
    {
        //debugText = "Received orientation: " + picOrientation;
        orientation = picOrientation;
        EventManager.TextureRecieved.Invoke();
        //Debug.Log("Received orientation: " + picOrientation);
    }
#endif

    private void Start()
    {
        deleteButton.onClick.AddListener(DeleteImage);
        deleteButton.onClick.Invoke();
        EventManager.TextureRecieved.AddListener(OnTextureRecieved);
    }

    private void OnTextureRecieved()
    {
        //debugTextObject.text = debugText;
    }

    public void RequestImage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenCamera(OnImageReceived, OnOrientationReceived);
#else
        PickImageAndDisplayFromExplorer();
#endif
        StartCoroutine(PopupAfterImageLoading());
        EventManager.AssignmentCompleted.Invoke();
        
    }

    private IEnumerator PopupAfterImageLoading()
    {
        float timer = 0;
        while (wasImageLoaded == false)
        {
            timer += Time.deltaTime;
            if (timer >= 5) break;
            yield return new WaitForEndOfFrame();
        }
        popupGameObject.SetActive(true);
    }

    public void DeleteImage()
    {
        UploadImage uploadImage = FindObjectOfType<UploadImage>();
        uploadImage.RemoveImage();
        EventManager.AssignmentNotCompleted.Invoke();
    }
    private void PickImageAndDisplayFromExplorer()
    {
        
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image File", "", "jpg,png,bmp", false);
        if (paths.Length > 0)
        {
            // Create a Texture2D from the selected image
            Texture2D texture = LoadTexture(paths[0]);
            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + paths[0]);
                return;
            }

            // Create a new readable texture as a copy of the original
            Texture2D readableTexture =
                new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
            if (readableTexture.mipmapCount > 1)
                Graphics.CopyTexture(texture, readableTexture);
            else
                Graphics.CopyTexture(texture, 0, 0, readableTexture, 0, 0);

            // Save image to a file
            byte[] imgData = readableTexture.EncodeToPNG();
            //debugTextObject.text = "Received image: " + imgData.Length;
            string fileName = "myImage.png";
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), imgData);
            PlayerPrefs.SetString("capturedImage", fileName);

            // Display the image
            UploadImage uploadImage = FindObjectOfType<UploadImage>();
            wasImageLoaded = true;
            uploadImage.DisplayImage(readableTexture, 1);
        }

    }


    private Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }

        return tex;
    }
}
