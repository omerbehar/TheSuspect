using TMPro;
using UnityEngine;
public class InputHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    public void ShowInputDialog()
    {
#if UNITY_WEBGL
        Application.ExternalCall("focusHandleAction", gameObject.name, "Default text");
#endif
    }

    public void ReceiveInputData(string inputData)
    {
        Debug.Log("Received input data: " + inputData);
        inputField.text = inputData;

        // Handle the input data as needed
    }
    
    private void OnEnable()
    {
        //Debug.Log("OnEnable called");
        // Add a listener to the input field's onEndEdit event
        inputField.onEndEdit.AddListener(delegate { ShowInputDialog(); });
    }

    private void OnDisable()
    {
        // Remove the listener
        inputField.onEndEdit.RemoveListener(delegate { ShowInputDialog(); });
    }
}