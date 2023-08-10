using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;

namespace DefaultNamespace
{
    public class UploadImage : MonoBehaviour
    {
        public RawImage rawImageDisplay;
        public Rect imageBounds = new Rect(0, 0, 1024, 1024); // Define the position and size of image bounds.
        public bool isPlayTest = true;

        void Start()
        {
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Start with a transparent image.

#if PLAYTEST
            if(isPlayTest)
                 PickImageAndDisplayFromExplorer();
            else
                 LoadImage();
#else
            LoadImage();
#endif
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
            NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
            {
                Debug.Log("Image path: " + path);
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
        }

        public void RemoveImage()
        {
            rawImageDisplay.texture = null;
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Switch back to fully transparent.

            PlayerPrefs.DeleteKey("capturedImage");
        }

        private void DisplayImage(Texture2D capturedImage)
        {
            rawImageDisplay.texture = ResizeTexture(capturedImage, (int)imageBounds.width, (int)imageBounds.height);
            rawImageDisplay.rectTransform.sizeDelta = new Vector2(imageBounds.width, imageBounds.height);
            rawImageDisplay.color = new Color(1, 1, 1, 1);
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
                EventManager.OnAssignmentCompleted?.Invoke();
            }
        }
    }
}