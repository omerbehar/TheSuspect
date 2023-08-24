using System;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    public class UploadImage : MonoBehaviour
    {
        [SerializeField] private TMP_Text debugText;
        [SerializeField] RawImage rawImageDisplay;
        [SerializeField] RawImage imageBorderDisplay;
        [SerializeField] private int horizontalWidth = 1024; // Your desired width for horizontal images
        [SerializeField] private int horizontalHeight = 512; // Your desired height for horizontal images
        [SerializeField] private int verticalWidth = 512; // Your desired width for vertical images
        [SerializeField] private int verticalHeight = 1024; // Your desired height for vertical images
        [SerializeField] Rect imageBounds = new Rect(0, 0, 1024, 1024); // Define the position and size of image bounds.
        [SerializeField] bool isPlayTest = true;
        [SerializeField] private GameObject whenUploadIcons;
        [SerializeField] private GameObject whenNoUploadIcons;
        private WebCamDevice[] devices;
        private static Texture2D tex;

        void Start()
        {
            EventManager.TextureRecieved.AddListener(OnTextureReceived);
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Start with a transparent image.
            CallShowAlert();
            // yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            // if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            // {
            //     Debug.Log("webcam found");
            //         debugText.text = "webcam found";
            //     devices = WebCamTexture.devices;
            //     for (int cameraIndex = 0; cameraIndex < devices.Length; ++cameraIndex)
            //     {
            //         Debug.Log("devices[cameraIndex].name: ");
            //         Debug.Log(devices[cameraIndex].name);
            //         Debug.Log("devices[cameraIndex].isFrontFacing");
            //         Debug.Log(devices[cameraIndex].isFrontFacing);
            //     }
            // }
            // else
            // {
            //     Debug.Log("no webcams found");
            //     debugText.text = "no webcams found";
            // }
#if UNITY_WEBGL
            isPlayTest = false;
#endif

#if PLAYTEST
            if(isPlayTest)
                Debug.Log("Playtest mode is on.");
                 //PickImageAndDisplayFromExplorer();
            else
                 LoadImage();
#else
            LoadImage();
#endif
        }

        private void OnTextureReceived()
        {
            DisplayImage(tex);
        }
// #if UNITY_WEBGL && !UNITY_EDITOR
//         [DllImport("__Internal")]
//         private static extern void showAlert(string message);
//         [DllImport("__Internal")]
//         private static extern void CaptureImageFromCamera();
//         [DllImport("__Internal")]
//         private static extern void ReceiveImageFromJS(int stringPointer);
//         [DllImport("__Internal")]
//         private static extern void BindWebGLTexture(int texture);
//
//
// #endif
        public void CallShowAlert() {
// #if UNITY_WEBGL && !UNITY_EDITOR
//             showAlert("Hello from Unity!");
// #endif
        }
        //
        // [DllImport("__Internal")]
        // private static extern void OpenNativeCamera();
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void ReceiveImageFromJS(string base64Image)
        {
            // Convert the base64Image to a Texture2D, etc.
            // You can use this method: ConvertFromBase64ToTexture2D(base64Image);
            tex = ConvertFromBase64ToTexture2D(base64Image);
            //trigger event to display image
            EventManager.TextureRecieved.Invoke();
        }

        static Texture2D ConvertFromBase64ToTexture2D(string base64)
        {
            //debugText.text = "ReceiveImage";
            Texture2D tex = new Texture2D(2, 2);
            byte[] imageBytes = Convert.FromBase64String(base64);
            tex.LoadImage(imageBytes);
            return tex;
        }
        public void ReceiveImage(string base64Image) {
            debugText.text = "ReceiveImage";
            Texture2D texture = new Texture2D(2, 2);
            byte[] byteArr = System.Convert.FromBase64String(base64Image.Split(',')[1]);
            texture.LoadImage(byteArr);
        
            // Now you can use this texture as needed in Unity
            // For instance, apply the texture to a RawImage on the canvas
            DisplayImage(texture);
        }
        public void PickImageAndDisplayFromExplorer()
        {
#if PLAYTEST
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
                Texture2D readableTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
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
                DisplayImage(readableTexture);
            }
#endif
        
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

        public void PickImageAndDisplay(int maxSize)
        {
// #if UNITY_WEBGL && !UNITY_EDITOR
//             CaptureImageFromCamera();
//             var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
//             BindWebGLTexture(texture.GetNativeTexturePtr());
// #endif
            NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
            {
                Debug.Log("Image path: " + path);
                debugText.text = "Image path: " + path;
                if (path != null)
                {
                    // Create a Texture2D from the captured image
                    Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                    if (texture != null)
                    {
                        // Create a new readable texture as a copy of the original
                        Texture2D readableTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
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
                        DisplayImage(readableTexture);
                    }
                    else
                    {
                        Debug.Log("Couldn't load texture from " + path);
                    }
                }
            }, maxSize);

            Debug.Log("Permission result: " + permission);
            debugText.text = "Permission result: " + permission;
        }

        public void RemoveImage()
        {
            rawImageDisplay.texture = null;
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Switch back to fully transparent.

            PlayerPrefs.DeleteKey("capturedImage");
        }

        public void DisplayImage(Texture2D capturedImage)
        {
            rawImageDisplay.texture = capturedImage;

            // Calculate aspect ratio and set new dimensions
            float imageAspect = (float)capturedImage.width / capturedImage.height;
            float areaAspect = imageBounds.width / imageBounds.height;
            Debug.Log($"imageAspect: {imageAspect}, areaAspect: {areaAspect}");
            float scaleFactor;
            if (imageAspect < areaAspect)
            {
                scaleFactor = imageBounds.height / capturedImage.height;
            }
            else
            {
                scaleFactor = imageBounds.width / capturedImage.width;
            }
            Debug.Log($"scaleFactor: {scaleFactor}");
            int width = Mathf.RoundToInt(capturedImage.width * scaleFactor);
            int height = Mathf.RoundToInt(capturedImage.height * scaleFactor);
            
            // Set the size:
            rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            float borderThickness = 10f;  // Set your border's thickness.

            // Adjust the border size
            imageBorderDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + borderThickness);
            imageBorderDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + borderThickness);

            // Centering the image
            rawImageDisplay.rectTransform.anchoredPosition = Vector2.zero;
            imageBorderDisplay.rectTransform.anchoredPosition = Vector2.zero;
            
          
            imageBorderDisplay.color = new Color(1, 1, 1, 1);
            rawImageDisplay.color = new Color(1, 1, 1, 1);
            ResizeAndDisplayImage(capturedImage);

            whenNoUploadIcons.SetActive(true);
            whenUploadIcons.SetActive(false);
        }
        
       private void ResizeAndDisplayImage(Texture2D capturedImage)
{
    
    // get the image's original dimensions
    float originalWidth = capturedImage.width;
    float originalHeight = capturedImage.height;

    // calculate the aspect ratio
    float originalAspectRatio = originalWidth / originalHeight;

    float newWidth = 0, newHeight = 0;

    // if the image is horizontal
    if (originalWidth > originalHeight)
    {
        // if the original width is greater than max horizontal width
        // set newWidth to max width and scale height accordingly
        if (originalWidth > horizontalWidth)
        {
            newWidth = horizontalWidth;
            newHeight = (int)(newWidth / originalAspectRatio);
        }
        else
        {
            newWidth = (int)originalWidth;
            newHeight = (int)originalHeight;
        }

        // Reset the position for horizontal image
        rawImageDisplay.rectTransform.anchoredPosition = new Vector2(rawImageDisplay.rectTransform.anchoredPosition.x, 0);
    }
    // else if the image is vertical
    else if (originalWidth < originalHeight)
    {
        // if the original height is greater than max vertical height
        // set newHeight to max height and scale width accordingly
        if (originalHeight > verticalHeight)
        {
            newHeight = verticalHeight;
            newWidth = (int)(newHeight * originalAspectRatio);
        }
        else
        {
            newWidth = (int)originalWidth;
            newHeight = (int)originalHeight;
        }

        // Adjust the position for vertical image
        rawImageDisplay.rectTransform.anchoredPosition = new Vector2(rawImageDisplay.rectTransform.anchoredPosition.x, -60);
    }

    // Set the new dimensions:
    rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

    // Display the image after resizing
    rawImageDisplay.texture = capturedImage;
}
        private void ResizeImage(Texture2D source, int targetWidth, int targetHeight)
        {
            RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
            RenderTexture.active = rt;

            Graphics.Blit(source, rt);

            Texture2D result = new Texture2D(targetWidth, targetHeight);
            result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            rawImageDisplay.texture = result;
        }

        private Texture2D CropTexture(Texture2D source, int xOffset, int yOffset, int width, int height)
        {
            // Check if the cropping area exceeds the source texture's dimensions
            if (xOffset + width > source.width)
            {
                width = source.width - xOffset;
            }

            if (yOffset + height > source.height)
            {
                height = source.height - yOffset;
            }

            Color[] c = source.GetPixels(xOffset, yOffset, width, height);
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(c);
            result.Apply();
    
            return result;
        }
        public Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D target = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] pixelsTop = source.GetPixels();
            Color[] pixelsBottom = new Color[targetWidth * targetHeight];
            
            for (int y = 0; y < targetHeight; y++)
            for (int x = 0; x < targetWidth; x++)
            {
                int sourceIndex = (targetHeight - y - 1) * source.width + x;
                int targetIndex = y * targetWidth + x;
                if (sourceIndex < pixelsTop.Length && targetIndex < pixelsBottom.Length)
                    pixelsBottom[targetIndex] = pixelsTop[sourceIndex];
            }
                
            target.SetPixels(pixelsBottom);
            target.Apply();
            return target;
        }

        private void LoadImage()
        {
            string fileName = PlayerPrefs.GetString("capturedImage");
            if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
            {
                byte[] imgData = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, fileName));
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imgData);

                DisplayImage(tex);
                EventManager.AssignmentCompleted?.Invoke();
            }
        }
    }
}