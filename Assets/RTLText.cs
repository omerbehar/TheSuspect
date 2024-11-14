using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using DA_Assets.Shared.CodeHelpers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RTLText : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private TMP_InputField tmproInputField;
    //[SerializeField] private RectTransform mainScreen;
    private string originalString = "";
    //public RectTransform canvasRectTransform; // Assign the Canvas' RectTransform
    public float desiredYOffsetFromTop = 100f; // How many pixels you want from the top
    private Vector2 originalPosition;
    private RectTransform inputFieldRectTransform;
    int keyboardHeight = 250;
    bool wasKeyboardOpen;
    private bool gotFocus;
    private static int _screenWidth;
    private static int _newScreenHeight;
    private int originalScreenHeight;
    

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendScreenWidth(GetScreenWidth callback);
    [DllImport("__Internal")]
    private static extern void SendScreenHeight(GetScreenHeight callback);

    delegate void GetScreenWidth(string screenWidth);
    [AOT.MonoPInvokeCallback(typeof(GetScreenWidth))]

    private static void OnWidthRecieved(string receivedScreenWidth)
    {
        _screenWidth = int.Parse(receivedScreenWidth);
    }
    delegate void GetScreenHeight(string screenHeight);
    [AOT.MonoPInvokeCallback(typeof(GetScreenHeight))]

    private static void OnHeightRecieved(string receivedScreenHeight)
    {
        _newScreenHeight = int.Parse(receivedScreenHeight);
    }
#endif

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SendScreenHeight(OnHeightRecieved);
        originalScreenHeight = _newScreenHeight;
#endif
        inputFieldRectTransform = GetComponent<RectTransform>();
        //originalPosition = canvasRectTransform.anchoredPosition;
        inputField?.onValueChanged.AddListener(ReverseInputText);
        tmproInputField?.onValueChanged.AddListener(ReverseInputText);
    }

    private void Update()
    {
        if (inputField && inputField.isFocused && !gotFocus)
        {
            gotFocus = true;
            OnInputSelected();
        }

        if (tmproInputField && tmproInputField.isFocused && !gotFocus)
        {
            gotFocus = true;
            OnInputSelected();
        }
        if (inputField && !inputField.isFocused && gotFocus)
        {
            gotFocus = false;
            OnInputDeselected();
        }

        if (tmproInputField && !tmproInputField.isFocused && gotFocus)
        {
            gotFocus = false;
            OnInputDeselected();
        }
    }
    
    private void ReverseInputText(string value)
    {
        //Debug.Log("reverseMain");
        if (value.Length > originalString.Length)
        {
            // Add the new character to the original string
            originalString += value[value.Length - 1];
        }
        else if (value.Length < originalString.Length)
        {
            // Remove the last character from the original string
            originalString = originalString.Substring(0, originalString.Length - 1);
        }

        if (IsRightToLeft(originalString))
        {
            char[] reversedChars = originalString.ToCharArray();
            Array.Reverse(reversedChars);
            inputField?.onValueChanged.RemoveListener(ReverseInputText);
            tmproInputField.onValueChanged.RemoveListener(ReverseInputText);
            if (inputField) inputField.text = new string(reversedChars);
            tmproInputField.text = new string(reversedChars);
            inputField?.onValueChanged.AddListener(ReverseInputText);
            tmproInputField.onValueChanged.AddListener(ReverseInputText);
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

    private void OnInputSelected()
    {
        if (!wasKeyboardOpen)
        {
            StartCoroutine(GetKeyboardHeight());
            wasKeyboardOpen = true;
        }
#if UNITY_EDITOR
        MockKeyboardOpening();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
        SendScreenWidth(OnWidthRecieved);
#endif
        if (_screenWidth < 800)
        {
            //move canvas down by keyboard height
            // canvasRectTransform.position = new Vector2(canvasRectTransform.position.x,
            //      canvasRectTransform.position.y - (keyboardHeight - 100));

        }
    }

    private void MockKeyboardOpening()
    {
        //move main screen up by keyboard height
        //mainScreen.anchoredPosition = new Vector2(mainScreen.anchoredPosition.x, mainScreen.anchoredPosition.y + keyboardHeight);
    }
    private void MockKeyboardClosing()
    {
        //move main screen down by keyboard height
        //mainScreen.anchoredPosition = new Vector2(mainScreen.anchoredPosition.x, mainScreen.anchoredPosition.y - keyboardHeight);
    }
    private void OnInputDeselected()
    {
        #if UNITY_EDITOR
        MockKeyboardClosing();
        #endif
        //canvasRectTransform.anchoredPosition = Vector2.zero;
    }
    private IEnumerator GetKeyboardHeight()
    {
        yield return new WaitForSeconds(0.1f);
#if UNITY_EDITOR
        keyboardHeight = 250;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        SendScreenHeight(OnHeightRecieved);
        keyboardHeight = originalScreenHeight - _newScreenHeight;
#endif
    }
}

    
