using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RTLText : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    private bool shouldReverse = true;
    private string originalString = "";

    void Start()
    {
        inputField.onValueChanged.AddListener(ReverseInputText);
    }
    private void ReverseInputText(string value)
    {
        if (value.Length > originalString.Length)
        {
            // Add the new character to the original string
            originalString += value[^1];
        }
        else if (value.Length < originalString.Length)
        {
            // Remove the last character from the original string
            originalString = originalString[..^1];
        }

        if (IsRightToLeft(originalString))
        {
            // Reverse the original string
            char[] reversedChars = originalString.ToCharArray();
            System.Array.Reverse(reversedChars);
            inputField.onValueChanged.RemoveListener(ReverseInputText);
            inputField.text = new string(reversedChars);
            inputField.onValueChanged.AddListener(ReverseInputText);
        }
    }
    
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
