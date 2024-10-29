using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleHyperlink : MonoBehaviour
{
    public Button button;
    public string url;

    private void Start()
    {
        // Add a listener to the button's onClick event
        button.onClick.AddListener(OpenURL);
    }

    private void OpenURL()
    {
        // Open the URL when the button is clicked
        Application.OpenURL(url);
    }

    private void OnDestroy()
    {
        // Remove the listener when the script is destroyed
        button.onClick.RemoveListener(OpenURL);
    }
}