using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RTLText : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    private string originalString = "";
    public RectTransform canvasRectTransform; // Assign the Canvas' RectTransform
    public float desiredYOffsetFromTop = 100f; // How many pixels you want from the top
    private Vector2 originalPosition;
    private RectTransform inputFieldRectTransform;
    int keyboardHeight = 270;
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
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        canvasRectTransform = mainCanvas.GetComponent<RectTransform>();
        originalPosition = canvasRectTransform.anchoredPosition;
        inputField.onValueChanged.AddListener(ReverseInputText);
    }

    private void Update()
    {
        if (inputField.isFocused && !gotFocus)
        {
            gotFocus = true;
            OnInputSelected();
        }
        if (!inputField.isFocused && gotFocus)
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
            // Reverse the original string
            //Debug.Log("text reversed");
            char[] reversedChars = originalString.ToCharArray();
            Array.Reverse(reversedChars);
            inputField.onValueChanged.RemoveListener(ReverseInputText);
            inputField.text = new string(reversedChars);
            inputField.onValueChanged.AddListener(ReverseInputText);
        }
        
    }

    private bool IsRightToLeft(string text)
    {
        //Debug.Log("IsRightToLeft");
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
#if UNITY_WEBGL && !UNITY_EDITOR
        SendScreenWidth(OnWidthRecieved);
        if (Input.touchSupported && _screenWidth < 800)
        {
            float distanceFromTop = -inputFieldRectTransform.localPosition.y;
            canvasRectTransform.anchoredPosition = new Vector2(originalPosition.x,
                canvasRectTransform.anchoredPosition.y - keyboardHeight + distanceFromTop);
        }
#endif
    }

    private void MockKeyboard()
    {
        canvasRectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
    }

    private void OnInputDeselected()
    {
        canvasRectTransform.anchoredPosition = Vector2.zero;
    }
    private IEnumerator GetKeyboardHeight()
    {
        yield return new WaitForSeconds(0.1f);
#if UNITY_EDITOR
        //MockKeyboard();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        SendScreenHeight(OnHeightRecieved);
        keyboardHeight = originalScreenHeight - _newScreenHeight;
#endif
    }
}

    
