using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RTLText : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    private string originalString = "";

    void Start()
    {
        inputField.onValueChanged.AddListener(ReverseInputText);
        
    }
    // void FixedUpdate()
    // {
    //     if (inputField.isFocused)
    //     {
    //         HandleBackspace();
    //     }
    //    
    // }

    // private void HandleBackspace()
    // {
    //     if (BackspacePressed() && originalString.Length > 0 && IsRightToLeft(originalString))
    //     {
    //         originalString = originalString[..^1];
    //         // Now, update the text of the input field to reflect the deletion
    //         inputField.text = originalString;
    //         //ReverseInputText(originalString); // we'll manually call this with the updated originalString
    //     }
    // }

    private void ReverseInputText(string value)
    {
        if (value.Length > originalString.Length)
        {
            // Add the new character to the original string
            originalString += value[^1];
            // originalString = value[0] + originalString;
            
        }
        else if (value.Length < originalString.Length)
        {
            // Remove the last character from the original string
            originalString = originalString[..^1];
        }

        if (IsRightToLeft(originalString))
        {
            //AdjustCaretPosition();
            // Reverse the original string
            char[] reversedChars = originalString.ToCharArray();
            System.Array.Reverse(reversedChars);
            inputField.onValueChanged.RemoveListener(ReverseInputText);
            inputField.text = new string(reversedChars);
            inputField.onValueChanged.AddListener(ReverseInputText);
            // // Adjust caret position for RTL text
        }
        
    }
    // private bool BackspacePressed()
    // {
    //     return Input.GetKey(KeyCode.Backspace);
    // }
    // private void AdjustCaretPosition()
    // {
    //     if (inputField != null)
    //     {
    //         inputField.caretPosition = 0;
    //     }
    // }

    private bool IsRightToLeft(string text)
    {
        foreach (char c in text)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category == UnicodeCategory.OtherLetter)
            {
                if (c >= 0x0590 && c <= 0x08FF)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
