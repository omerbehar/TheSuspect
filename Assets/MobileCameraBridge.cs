using System;
using System.Runtime.InteropServices;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MobileCameraBridge : MonoBehaviour
{
    //private UploadImage uploadImage;
    [DllImport("__Internal")]
    private static extern void OpenCamera(MobileCameraCallback callback);

    delegate void MobileCameraCallback(string imageData);

    // private void Start()
    // {
    //     uploadImage = FindObjectOfType<UploadImage>(); //fix this
    // }

    public void RequestImage()
    {
        OpenCamera(OnImageReceived);
    }

    [AOT.MonoPInvokeCallback(typeof(MobileCameraCallback))]
    private static void OnImageReceived(string imageData)
    {
        // Here you can convert the image data (base64) into a texture and use it in Unity.
        byte[] bytes = System.Convert.FromBase64String(imageData);
        Debug.Log("Received image data: " + bytes.Length);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        Debug.Log("Texture size: " + texture.width + "x" + texture.height);
        // For demonstration purposes, we're setting the texture to the main material
        // You should process this texture as needed in your game.
        // For example, you can set the texture to a RawImage component.
        //GameObject imageObject = GameObject.FindGameObjectWithTag("ImageObject");
        UploadImage uploadImage = FindObjectOfType<UploadImage>();
        uploadImage.DisplayImage(texture);
        // Debug.Log("Image object: " + imageObject);
        // if (imageObject != null)
        // {
        //     imageObject.GetComponent<RawImage>().texture = texture;
        //     Debug.Log("Image object texture: " + imageObject.GetComponent<RawImage>().texture);
        // }
    }
}