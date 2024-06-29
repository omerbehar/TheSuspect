using System;
using System.Collections.Generic;
using System.Linq;
using Screens.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Screens
{
    [Serializable]
    public class WordWithMissingChars
    {
        public string word;
        public List<char> missingChars; // List of characters to be replaced with input fields
        [FormerlySerializedAs("inputFieldParent")] public Transform lineTransform; // Parent transform for the input fields of this word
    }

    public class Screen8StringCheck : ScreenBase
    {
        [SerializeField] private GameObject inputFieldPrefab;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private GameObject wordParentPrefab; // Prefab for the parent of each word
        [SerializeField] private List<WordWithMissingChars> wordsWithMissingChars;
        [SerializeField] private GameObject failedGO;
        [SerializeField] private float elementSpacing = 2f; // Adjustable spacing value

        private List<InputField> inputFields = new List<InputField>();
        private List<string> correctChars = new List<string>();

        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            NextButton.interactable = false;
            if (inputFieldPrefab == null)
            {
                Debug.LogError("inputFieldPrefab is not assigned.");
                return;
            }

            if (textPrefab == null)
            {
                Debug.LogError("textPrefab is not assigned.");
                return;
            }

            if (wordParentPrefab == null)
            {
                Debug.LogError("wordParentPrefab is not assigned.");
                return;
            }

            if (wordsWithMissingChars == null || wordsWithMissingChars.Count == 0)
            {
                Debug.LogError("wordsWithMissingChars is not assigned or empty.");
                return;
            }

            foreach (var wordWithMissingChars in wordsWithMissingChars)
            {
                string word = wordWithMissingChars.word;
                List<char> missingChars = wordWithMissingChars.missingChars;
                Transform inputFieldParent = wordWithMissingChars.lineTransform;

                if (string.IsNullOrEmpty(word))
                {
                    Debug.LogError("Word is null or empty.");
                    continue;
                }

                if (missingChars == null || missingChars.Count == 0)
                {
                    Debug.LogError("missingChars is null or empty.");
                    continue;
                }

                if (inputFieldParent == null)
                {
                    Debug.LogError("inputFieldParent is not assigned for word: " + word);
                    continue;
                }

                // Instantiate the parent for this word
                GameObject wordParentGO = Instantiate(wordParentPrefab, inputFieldParent);

                // Ensure the parent GameObject has a HorizontalLayoutGroup to arrange the characters properly
                HorizontalLayoutGroup layoutGroup = wordParentGO.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup == null)
                {
                    layoutGroup = wordParentGO.AddComponent<HorizontalLayoutGroup>();
                }

                // Set the alignment to middle center and adjust spacing
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.spacing = elementSpacing; // Use the adjustable spacing value
                layoutGroup.padding = new RectOffset(0, 0, 0, 0); // Minimal padding

                // Reverse the word for RTL display
                char[] reversedWordArray = word.ToCharArray();
                Array.Reverse(reversedWordArray);
                string reversedWord = new string(reversedWordArray);

                for (int i = 0; i < reversedWord.Length; i++)
                {
                    char c = reversedWord[i];
                    if (missingChars.Contains(c))
                    {
                        // Instantiate input field prefab for missing character
                        GameObject inputFieldGO = Instantiate(inputFieldPrefab, wordParentGO.transform);
                        InputField inputField = inputFieldGO.GetComponent<InputField>();
                        if (inputField == null)
                        {
                            Debug.LogError("TMP_InputField component is missing on inputFieldPrefab.");
                            continue;
                        }
                        inputField.characterLimit = 1;
                        inputField.onValueChanged.AddListener(delegate { OnFieldValueChanged(); });
                        inputField.onEndEdit.AddListener(delegate { OnFieldValueChanged(); });
                        inputFields.Add(inputField);
                        correctChars.Add(c.ToString());
                    }
                    else
                    {
                        // Instantiate text prefab for existing character
                        GameObject textGO = Instantiate(textPrefab, wordParentGO.transform);
                        TextMeshProUGUI textComponent = textGO.GetComponent<TextMeshProUGUI>();
                        if (textComponent == null)
                        {
                            Debug.LogError("TextMeshProUGUI component is missing on textPrefab.");
                            continue;
                        }
                        textComponent.text = c.ToString();
                    }
                }
            }
        }

        private void OnFieldValueChanged()
        {
            IsSentenceCorrect();
        }

        private void IsSentenceCorrect()
        {
            bool isSentenceCorrect = true;

            foreach (var wordWithMissingChars in wordsWithMissingChars)
            {
                List<char> missingChars = wordWithMissingChars.missingChars;

                foreach (char missingChar in missingChars)
                {
                    bool charFound = false;
                    foreach (InputField inputField in inputFields)
                    {
                        if (inputField.text == missingChar.ToString())
                        {
                            charFound = true;
                            break;
                        }
                    }
                    if (!charFound)
                    {
                        isSentenceCorrect = false;
                        break;
                    }
                }

                if (!isSentenceCorrect)
                {
                    break;
                }
            }

            if (isSentenceCorrect && inputFields.All(field => !string.IsNullOrEmpty(field.text)))
            {
                Debug.Log("Sentence is correct!");
                DeactivateFailedMessage();
                EventManager.AssignmentCompleted.Invoke();
            }
            else
            {
                Debug.Log("Sentence is incorrect or not all fields are filled.");
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
