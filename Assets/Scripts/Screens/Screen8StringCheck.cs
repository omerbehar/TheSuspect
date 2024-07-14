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
        [SerializeField] private float charOffset = 10f; // Adjustable offset between characters
        [SerializeField] private Button fakeNextButton;
        [SerializeField] private bool initializeOnStart = true;

        private List<InputField> inputFields = new List<InputField>();
        private List<string> correctChars = new List<string>();
        private List<GameObject> instantiatedObjects = new List<GameObject>(); // List to track instantiated GameObjects

        protected override void Start()
        {
            if (initializeOnStart)
            {
                Init();
            }

            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
        }

        private void OnFakeNextButtonClicked()
        {
            ActivateFailedMessage();
        }

        public void Init()
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
                instantiatedObjects.Add(wordParentGO); // Track the instantiated GameObject

                // Ensure the parent GameObject has a HorizontalLayoutGroup to arrange the characters properly
                HorizontalLayoutGroup layoutGroup = wordParentGO.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup == null)
                {
                    layoutGroup = wordParentGO.AddComponent<HorizontalLayoutGroup>();
                }

                // Set the alignment to middle center and adjust spacing
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.reverseArrangement = true; // Enable RTL support

                // Calculate the total width of the word
                float totalWidth = word.Length * (inputFieldPrefab.GetComponent<RectTransform>().rect.width + charOffset);

                // Adjust spacing and padding based on the word length
                layoutGroup.spacing = elementSpacing + charOffset; // Use the adjustable spacing value and char offset
                layoutGroup.padding = new RectOffset((int)(totalWidth / 2), (int)(totalWidth / 2), 0, 0); // Adjust padding

                for (int i = 0; i < word.Length; i++)
                {
                    char c = word[i];
                    if (missingChars.Contains(c))
                    {
                        // Instantiate input field prefab for missing character
                        GameObject inputFieldGO = Instantiate(inputFieldPrefab, wordParentGO.transform);
                        instantiatedObjects.Add(inputFieldGO); // Track the instantiated GameObject
                        InputField inputField = inputFieldGO.GetComponent<InputField>();
                        if (inputField == null)
                        {
                            Debug.LogError("TMP_InputField component is missing on inputFieldPrefab.");
                            continue;
                        }
                        inputField.characterLimit = 1;
                        inputField.onValueChanged.AddListener(delegate { OnFieldValueChanged(inputField.text, inputFields.IndexOf(inputField)); });
                        inputFields.Add(inputField);
                        correctChars.Add(c.ToString());
                    }
                    else
                    {
                        // Instantiate text prefab for existing character
                        GameObject textGO = Instantiate(textPrefab, wordParentGO.transform);
                        instantiatedObjects.Add(textGO); // Track the instantiated GameObject
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

        public void ClearFields()
        {
            // Destroy all instantiated GameObjects
            foreach (var obj in instantiatedObjects)
            {
                DestroyImmediate(obj);
            }
            instantiatedObjects.Clear();

            // Clear the lists
            inputFields.Clear();
            correctChars.Clear();

            Debug.Log("All fields and lists have been cleared.");
        }

        private void OnFieldValueChanged(string input, int fieldIndex)
        {
            // Check if the input is correct
            if (input == correctChars[fieldIndex])
            {
                // Input is correct, do nothing
                DeactivateFailedMessage();
            }
            else if (string.IsNullOrEmpty(input))
            {
                // Input is empty, do nothing
                DeactivateFailedMessage();
            }
            else
            {
                // Input is incorrect
                Debug.Log($"Character '{input}' is incorrect. Expected '{correctChars[fieldIndex]}'.");
                ActivateFailedMessage();
            }

            // Move to the next input field
            if (fieldIndex < inputFields.Count - 1)
            {
                inputFields[fieldIndex + 1].ActivateInputField();
            }

            // Check if all input fields are filled
            if (inputFields.All(field => !string.IsNullOrEmpty(field.text)))
            {
                Debug.Log("All input fields are filled. Checking sentence correctness...");
                IsSentenceCorrect();
            }
        }

        private void IsSentenceCorrect()
        {
            bool isSentenceCorrect = true;

            for (int i = 0; i < inputFields.Count; i++)
            {
                if (inputFields[i].text != correctChars[i])
                {
                    Debug.Log($"Character '{inputFields[i].text}' is incorrect. Expected '{correctChars[i]}'.");
                    isSentenceCorrect = false;
                    break;
                }
            }

            if (isSentenceCorrect)
            {
                Debug.Log("Sentence is correct!");
                EventManager.AssignmentCompleted.Invoke();
                fakeNextButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Sentence is incorrect.");
                fakeNextButton.gameObject.SetActive(true);
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

        [ContextMenu("Initialize")]
        public void InitializeFromInspector()
        {
            Init();
        }
    }
}
