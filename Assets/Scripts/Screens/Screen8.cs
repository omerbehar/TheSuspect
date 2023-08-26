using System;
using Screens.Bases;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace Screens
{
    public class Screen8 : ScreenBase
    {
        private InputField[] inputFields = new InputField[3];
        [SerializeField] private int keycode;
        private int[] keycodes = new int[3];
        [SerializeField] private GameObject failedGO;

        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            inputFields = GetComponentsInChildren<InputField>();
            keycodes[0] = keycode / 100;
            keycodes[1] = keycode / 10 % 10;
            keycodes[2] = keycode % 10;
            
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].characterLimit = 1;
                int i1 = i;
                inputFields[i].onValueChanged.AddListener(delegate(string text) { OnFieldValueChanged(i1, text); });
                inputFields[i].onEndEdit.AddListener(delegate(string text) { OnFieldValueChanged(i1, text); });

            }
        }

        private void OnFieldValueChanged(int i, string text)
        {
            if(string.IsNullOrEmpty(text) && i > 0)
            {
                inputFields[i - 1].Select();
            }
            else if (i < inputFields.Length - 1 && text != "")
            {
                inputFields[i + 1].Select();
            }
        }
        

        private void Update()
        {
            IsCodeCorrect();
        }

        private void IsCodeCorrect()
        {
            //check if all fields are filled
            foreach (InputField inputField in inputFields)
            {
                if (inputField.text == "")
                {
                    DeactivateFailedMessage();
                    return;
                }
            }

            bool isCodeCorrect = true;
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (inputFields[i].text != keycodes[i].ToString())
                {
                    isCodeCorrect = false;
                }
            }
            if (isCodeCorrect)
                EventManager.AssignmentCompleted.Invoke();
            else
                ActivateFailedMessage();
        }

        private void DeactivateFailedMessage()
        {
            failedGO.SetActive(false);
            //change color of input fields
            foreach (InputField inputField in inputFields)
            {
                inputField.image.color = Color.white;
            }}

        private void ActivateFailedMessage()
        {
            failedGO.SetActive(true);
            //change color of input fields
            foreach (InputField inputField in inputFields)
            {
                bool parseSuccess = ColorUtility.TryParseHtmlString("#FF4050", out Color newCol);
                Debug.Log(parseSuccess + " " + newCol);
                inputField.image.color = newCol;
            }
        }
    }
}