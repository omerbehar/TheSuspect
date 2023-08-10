using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UploadImage : MonoBehaviour
    {
        public RawImage rawImageDisplay;
        

        void OnAssinmentComleted()
        {
            // Do something
        }
        private void Start()
        {
            rawImageDisplay.color = new Color(1, 1, 1, 0); // Start with a transparent image.

            LoadImage();
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

                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }

                    // Save image to PlayerPrefs
                    byte[] imgData = texture.EncodeToPNG();
                    string imgDataBase64 = System.Convert.ToBase64String(imgData);
                    PlayerPrefs.SetString("capturedImage", imgDataBase64);

                    // Display the image
                    DisplayImage(texture);
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
            rawImageDisplay.texture = capturedImage;
            rawImageDisplay.rectTransform.sizeDelta = new Vector2(capturedImage.width, capturedImage.height);
            rawImageDisplay.color = new Color(1, 1, 1, 1);
        }

        private void LoadImage()
        {
            if (PlayerPrefs.HasKey("capturedImage"))
            {
                byte[] imgData = System.Convert.FromBase64String(PlayerPrefs.GetString("capturedImage"));
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imgData);

                DisplayImage(tex);
                EventManager.OnAssignmentCompleted?.Invoke();
            }
        }
    }
}