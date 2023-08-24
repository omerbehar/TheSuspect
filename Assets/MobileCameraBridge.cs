using System;
using System.IO;
using System.Runtime.InteropServices;
using DefaultNamespace;
using SFB;
using UnityEngine;

public class MobileCameraBridge : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenCamera(MobileCameraCallback callback);

    delegate void MobileCameraCallback(string imageData);
    [AOT.MonoPInvokeCallback(typeof(MobileCameraCallback))]

    private static void OnImageReceived(string imageData)
    {
        byte[] bytes = Convert.FromBase64String(imageData);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        UploadImage uploadImage = FindObjectOfType<UploadImage>();
        uploadImage.DisplayImage(texture);
    }
#endif
    public void RequestImage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenCamera(OnImageReceived);
#else
        PickImageAndDisplayFromExplorer();
#endif
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
            uploadImage.DisplayImage(readableTexture);
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
