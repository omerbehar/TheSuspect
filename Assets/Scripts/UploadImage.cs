using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    public class UploadImage : MonoBehaviour
    {
        [SerializeField] RawImage rawImageDisplay;
        [SerializeField] RawImage imageBorderDisplay;
        [SerializeField] private int horizontalWidth = 1024; // Your desired width for horizontal images
        [SerializeField] private int horizontalHeight = 512; // Your desired height for horizontal images
        [SerializeField] private int verticalWidth = 512; // Your desired width for vertical images
        [SerializeField] private int verticalHeight = 1024; // Your desired height for vertical images
        [SerializeField] Rect imageBounds = new Rect(0, 0, 327, 168); // Define the position and size of image bounds.
        [SerializeField] bool isPlayTest = true;
        [SerializeField] private GameObject whenUploadIcons;
        [SerializeField] private GameObject whenNoUploadIcons;
        private WebCamDevice[] devices;
        private static Texture2D tex;

        void Start()
        {
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Start with a transparent image.
            LoadImage();
        }

        public void RemoveImage()
        {
            rawImageDisplay.texture = null;
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Switch back to fully transparent.
            imageBorderDisplay.color = new Color(1, 1, 1, 0); // Switch back to fully transparent.
            PlayerPrefs.DeleteKey("capturedImage");
            whenNoUploadIcons.SetActive(false);
            whenUploadIcons.SetActive(true);

        }

        public void DisplayImage(Texture2D capturedImage)
        {
            rawImageDisplay.texture = capturedImage;

            // Calculate aspect ratio and set new dimensions
            float imageAspect = (float)capturedImage.width / capturedImage.height;
            float areaAspect = imageBounds.width / imageBounds.height;
            float scaleFactor;
            if (imageAspect < areaAspect)
            {
                scaleFactor = imageBounds.height / capturedImage.height;
            }
            else
            {
                scaleFactor = imageBounds.width / capturedImage.width;
            }
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
            
          
            //imageBorderDisplay.color = new Color(1, 1, 1, 1);
            rawImageDisplay.color = new Color(1, 1, 1, 1);
            whenNoUploadIcons.SetActive(true);
            whenUploadIcons.SetActive(false);
            ResizeAndDisplayImage(capturedImage);

           
        }
        
        private void ResizeAndDisplayImage(Texture2D capturedImage)
        {
            // crop the image to desired aspect ratio (here 1:1 is used as an example)
            int croppedSize = Mathf.Min(capturedImage.width, capturedImage.height);
            capturedImage = CropTexture(capturedImage, 0, 0, croppedSize, croppedSize);
    
            // get the image's original dimensions
            float originalWidth = capturedImage.width;
            float originalHeight = capturedImage.height;

            // calculate the aspect ratio
            float originalAspectRatio = originalWidth / originalHeight;

            int newWidth = 0, newHeight = 0;

            // if the image is horizontal
            if (originalWidth > originalHeight)
            {
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
            }
            else if (originalWidth < originalHeight)
            {
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
            }

            // Resize the Texture
            capturedImage = ResizeImage(capturedImage, newWidth, newHeight);

            // Set the new dimensions:
            rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            // Display the image
            rawImageDisplay.texture = capturedImage;

            // Place the image in the center of the parent frame
            rawImageDisplay.rectTransform.anchoredPosition = Vector2.zero;
        }
        private Texture2D ResizeImage(Texture2D sourceTex, int targetWidth, int targetHeight)
        {
            if (sourceTex.width != targetWidth || sourceTex.height != targetHeight)
            {
                RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
                rt.filterMode = FilterMode.Point;
                RenderTexture.active = rt;
                Graphics.Blit(sourceTex, rt);
                Texture2D result = new Texture2D(targetWidth, targetHeight, sourceTex.format, false);
                result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                result.Apply();
                RenderTexture.active = null;
                return result;
            }
            else
            {
                return sourceTex;
            }
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