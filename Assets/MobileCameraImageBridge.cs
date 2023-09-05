using System;
using System.IO;
using System.Runtime.InteropServices;
using DefaultNamespace;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MobileCameraImageBridge : MonoBehaviour
{
    [SerializeField] private Button deleteButton;
    private static string debugText;
    //[SerializeField] private TMP_Text debugTextObject;

    private static int orientation;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void getOrientation(OrientationCallback2 callback);
    
    [DllImport("__Internal")]
    private static extern void OpenCamera(MobileCameraCallback callback, OrientationCallback orientationCallback);

    delegate void MobileCameraCallback(string imageData);
    delegate void OrientationCallback(int orientation);
    delegate void OrientationCallback2(int orientation);

    [AOT.MonoPInvokeCallback(typeof(MobileCameraCallback))]
    public static void OnImageReceived(string imageData)
    {
        byte[] bytes = Convert.FromBase64String(imageData);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        UploadImage uploadImage = FindObjectOfType<UploadImage>();
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
    [AOT.MonoPInvokeCallback(typeof(OrientationCallback2))]
    public static void OnOrientationReceived2(int orientation)
    {
        //debugText = "Received orientation2: " + orientation;
        EventManager.TextureRecieved.Invoke();
        //Debug.Log("Received orientation2: " + orientation);
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
        //getOrientation(OnOrientationReceived2);
#else
        PickImageAndDisplayFromExplorer();
#endif
        EventManager.AssignmentCompleted.Invoke();
        
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
            string fileName = "myImage.png";
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), imgData);
            PlayerPrefs.SetString("capturedImage", fileName);

            // Display the image
            UploadImage uploadImage = FindObjectOfType<UploadImage>();
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
