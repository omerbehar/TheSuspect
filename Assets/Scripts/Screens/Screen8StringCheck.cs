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
    public class Screen8StringCheck : ScreenBase
    {
        private InputField[] inputFields = new InputField[3];
        [SerializeField] private string[] correctStrings = new string[3];
        [SerializeField] private GameObject failedGO;

        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            inputFields = GetComponentsInChildren<InputField>();

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
            IsCodeCorrect();
        }

        private void IsCodeCorrect()
        {
            // Check if all fields are filled
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
                if (inputFields[i].text != correctStrings[i])
                {
                    isCodeCorrect = false;
                }
            }

            if (isCodeCorrect)
            {
                DeactivateFailedMessage();
                EventManager.AssignmentCompleted.Invoke();
            }
            else
            {
                ActivateFailedMessage();
            }
        }

        private void DeactivateFailedMessage()
        {
            failedGO.SetActive(false);
            // Change color of input fields
            foreach (InputField inputField in inputFields)
            {
                inputField.image.color = Color.white;
            }
        }

        private void ActivateFailedMessage()
        {
            failedGO.SetActive(true);
            foreach (InputField inputField in inputFields)
            {
                bool parseSuccess = ColorUtility.TryParseHtmlString("#FF4050", out Color newCol);
                inputField.image.color = newCol;
            }
        }
    }
}
