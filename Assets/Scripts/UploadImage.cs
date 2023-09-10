using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

    public void DisplayImage(Texture2D capturedImage, int orientation)
    {
        capturedImage = FixOrientation(capturedImage, orientation);
        rawImageDisplay.texture = capturedImage;
            
        // Calculate aspect ratio and set new dimensions
        float imageAspect = (float)capturedImage.width / capturedImage.height;
        float areaAspect = imageBounds.width / imageBounds.height;
        float scaleFactor = imageAspect < areaAspect
            ? imageBounds.height / capturedImage.height
            : imageBounds.width / capturedImage.width;
        // if (imageAspect < areaAspect)
        // {
        //     scaleFactor = imageBounds.height / capturedImage.height;
        // }
        // else
        // {
        //     scaleFactor = imageBounds.width / capturedImage.width;
        // }
        int width = Mathf.RoundToInt(capturedImage.width * scaleFactor);
        int height = Mathf.RoundToInt(capturedImage.height * scaleFactor);
            
        // Set the size:
        rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rawImageDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        const float borderThickness = 10f; // Set your border's thickness.

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
        EventManager.AssignmentCompleted?.Invoke();
        ResizeAndDisplayImage(capturedImage);

           
    }

    private Texture2D FixOrientation(Texture2D originalTexture, int orientation)
    {
        Texture2D fixedTexture = null;

        switch (orientation)
        {
            case 0:
            case 1: // Normal
                fixedTexture = originalTexture;
                break;

            case 2: // Horizontal Flip
                fixedTexture = FlipTexture(originalTexture, true, false);
                break;

            case 3: // 180 Rotate
                fixedTexture = RotateTexture(originalTexture, 180);
                break;

            case 4: // Vertical Flip
                fixedTexture = FlipTexture(originalTexture, false, true);
                break;

            case 5: // 90 Rotate + Horizontal Flip
                fixedTexture = FlipTexture(RotateTexture(originalTexture, 90), true, false);
                break;

            case 6: // 90 Rotate
                fixedTexture = RotateTexture(originalTexture, 90);
                break;

            case 7: // 90 Rotate + Vertical Flip
                fixedTexture = FlipTexture(RotateTexture(originalTexture, 90), false, true);
                break;

            case 8: // -90 Rotate
                fixedTexture = RotateTexture(originalTexture, -90);
                break;

            default:
                Debug.LogError("Invalid orientation value");
                break;
        }

        return fixedTexture;
    }
    private Texture2D RotateTexture(Texture2D originalTexture, float angle)
    {
        int x;
        int width = originalTexture.width;
        int height = originalTexture.height;
        var result = new Texture2D(height, width, originalTexture.format, false);
        for (x = 0; x < width; x++)
        {
            int y;
            for (y = 0; y < height; y++)
            {
                switch (angle)
                {
                    case 90:
                        result.SetPixel(y, width - x - 1, originalTexture.GetPixel(x, y));
                        break;
                    case 180:
                        result.SetPixel(width - x - 1, height - y - 1, originalTexture.GetPixel(x, y));
                        break;
                    case -90:
                        result.SetPixel(height - y - 1, x, originalTexture.GetPixel(x, y));
                        break;
                }
            }
        }
        result.Apply();
        return result;
    }

    private Texture2D FlipTexture(Texture2D originalTexture, bool flipHorizontally, bool flipVertically)
    {
        Texture2D flipped = new Texture2D(originalTexture.width, originalTexture.height);

        int xN = flipHorizontally ? (originalTexture.width - 1) : 0;
        int yN = flipVertically ? (originalTexture.height - 1) : 0;

        for (int i = 0; i < originalTexture.width; i++)
        {
            for (int j = 0; j < originalTexture.height; j++)
            {
                flipped.SetPixel(i, j, originalTexture.GetPixel(flipHorizontally ? xN - i : i, flipVertically ? yN - j : j));
            }
        }
        flipped.Apply();

        return flipped;
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
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(imgData);

            DisplayImage(texture2D, 0);
               
        }
    }
}