using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screens.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AnswerCheck : ScreenBaseWithTimer
{
    private static readonly int KeyboardIn = Animator.StringToHash("keyboardIn");
    [SerializeField]
    private UnityEvent CorrectAnswerEvent;

    [SerializeField]
    private UnityEvent IncorrectAnswerEvent;

    [SerializeField] private TMP_InputField tmpInputField;

    [SerializeField]
    private string correctAnswer;

    [SerializeField] private GameObject failedGO;
    [SerializeField] private GameObject failedAgainGO;
    private int incorrectTries;
    [SerializeField] private Animator keyboardAnimator;
    [SerializeField] private Animator keyboardAnimator2;
    private TouchScreenKeyboard keyboard;
    private bool keyboardActive;
    private bool isKeyboardActive;

    protected override async void Start()
    {
        incorrectTries = 0;
        base.Start();
        await Initialize();

#if !UNITY_EDITOR && UNITY_WEBGL 
            keyboard.active = false;
            UnityEngine.WebGLInput.mobileKeyboardSupport = true;
#endif
        NextButton.interactable = false;
    }

    private void Update()
    {
        WasClickedToCloseKeyboard();
    }

    private void WasClickedToCloseKeyboard()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (IsPointerOverKeyboardRelatedUI(Input.GetTouch(0).position)) return;
            if (isKeyboardActive) ShowKeyboard(false);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverKeyboardRelatedUI(Input.mousePosition)) return;
            Debug.Log($"iskeyboardactive: {isKeyboardActive}");
            if (isKeyboardActive) ShowKeyboard(false);
        }
    }

    private static bool IsPointerOverKeyboardRelatedUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = position
        };
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("KeyboardRelatedUI"))
                return true;
        }
        return false;
    }

    private Task Initialize()
    {
        base.Start();
        ShowKeyboard(false);        
        AddListeners();
        return Task.CompletedTask;
    }
    private void ShowKeyboard(bool showKeyboard)
    {
        keyboardAnimator.SetBool(KeyboardIn, showKeyboard);
        keyboardAnimator2.SetBool(KeyboardIn, showKeyboard);
        isKeyboardActive = showKeyboard;
    }

    public void OnKeyboardClick()
    {
        // Function to be called by buttons on the keyboard to keep it active
        keyboardActive = true;
    }

    private void AddListeners()
    {
        tmpInputField.onValueChanged.AddListener(delegate { OnInputFieldValueChanged(); });
        tmpInputField.onSelect.AddListener(_ =>
        {
            OnInputFieldSelect();
        });
        tmpInputField.onDeselect.AddListener(OnInputFieldDeSelect);

    }
    
    private void OnInputFieldDeSelect(string arg0)
    {
        if (!keyboardActive)
        {
            ShowKeyboard(false);
        }
    }
    private void OnInputFieldSelect()
    {
        keyboardActive = true;
        ShowKeyboard(true);
    }

    private void OnInputFieldValueChanged()
    {
        // If any field input is changed, set the NextButton as interactable.
        NextButton.interactable = true;
    }

    public override void OnNextButtonClicked()
    {
        string reverseAnswer = Reverse(tmpInputField.text);
        Debug.Log($"answer: {reverseAnswer}, correctAnswer: {correctAnswer}");
        
        if (reverseAnswer.Equals(correctAnswer))
        {
            CorrectAnswerEvent?.Invoke();
            base.OnNextButtonClicked();
        }
        else
        {
            Debug.Log("test");
            ActivateFailedMessage();
            IncorrectAnswerEvent?.Invoke();
        }
    }

    public static string Reverse( string s )
    {
        if (string.IsNullOrEmpty(s)) return "";
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    public void TwoMistakesNextScene()
    {
        base.OnNextButtonClicked();
    }

    private void ActivateFailedMessage()
    {
        if (incorrectTries == 0)
        {
            failedGO.SetActive(true);
            incorrectTries++;
        }
        else
        {
            failedAgainGO.SetActive(true);
        }
    }

    public void LinkClicked()
    {
        Application.OpenURL("https://levana.org.il/%D7%A9%D7%A4%D7%AA-%D7%94%D7%A1%D7%99%D7%9E%D7%A0%D7%99%D7%9D/");
    }

   
}