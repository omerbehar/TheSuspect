using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Screens.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private GameObject failedAgainGO;
        [SerializeField] private float elementSpacing = 2f; // Adjustable spacing value
        [SerializeField] private float charOffset = 10f; // Adjustable offset between characters
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private Button fakeNextButton; // Fake button for the initial state

        private List<InputField> inputFields = new List<InputField>();
        private List<string> correctChars = new List<string>();
        private List<GameObject> instantiatedObjects = new List<GameObject>(); // List to track instantiated GameObjects
        private int incorrectTries = 0; // Counter for incorrect tries

        protected override void Start()
        {
            base.Start();
            if (initializeOnStart)
            {
                Init();
            }
            NextButton.onClick.AddListener(OnNextButtonClicked);
            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
            fakeNextButton.gameObject.SetActive(true); // Show the fake button initially
            NextButton.gameObject.SetActive(false); // Hide the real next button initially
        }

        public void Init()
        {
            NextButton.interactable = false;
            if (inputFieldPrefab == null || textPrefab == null || wordParentPrefab == null)
            {
                Debug.LogError("One or more prefabs are not assigned.");
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

                if (string.IsNullOrEmpty(word) || missingChars == null || missingChars.Count == 0 || inputFieldParent == null)
                {
                    Debug.LogError("Missing or incorrect parameters for word setup.");
                    continue;
                }

                GameObject wordParentGO = Instantiate(wordParentPrefab, inputFieldParent);
                instantiatedObjects.Add(wordParentGO); // Track the instantiated GameObject

                HorizontalLayoutGroup layoutGroup = wordParentGO.GetComponent<HorizontalLayoutGroup>() ?? wordParentGO.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.reverseArrangement = true; // Enable RTL support

                float totalWidth = word.Length * (inputFieldPrefab.GetComponent<RectTransform>().rect.width + charOffset);
                layoutGroup.spacing = elementSpacing + charOffset;
                layoutGroup.padding = new RectOffset((int)(totalWidth / 2), (int)(totalWidth / 2), 0, 0);

                for (int i = 0; i < word.Length; i++)
                {
                    char c = word[i];
                    if (missingChars.Contains(c))
                    {
                        GameObject inputFieldGO = Instantiate(inputFieldPrefab, wordParentGO.transform);
                        instantiatedObjects.Add(inputFieldGO);
                        InputField inputField = inputFieldGO.GetComponent<InputField>();
                        inputField.characterLimit = 1;
                        inputField.onValueChanged.AddListener(delegate { OnFieldValueChanged(inputField); });
                        inputFields.Add(inputField);
                        correctChars.Add(c.ToString());
                    }
                    else
                    {
                        GameObject textGO = Instantiate(textPrefab, wordParentGO.transform);
                        instantiatedObjects.Add(textGO);
                        TextMeshProUGUI textComponent = textGO.GetComponent<TextMeshProUGUI>();
                        textComponent.text = c.ToString();
                    }
                }
            }
        }

private void OnFieldValueChanged(InputField inputField)
{
    int currentFieldIndex = inputFields.IndexOf(inputField);

    if (string.IsNullOrEmpty(inputField.text) || inputField.text == " ")
    {
        // Move to the previous input field when backspacing in an empty field
        if (currentFieldIndex > 0)
        {
            inputFields[currentFieldIndex - 1].Select();
            CloseKeyboard(); // Optional: close the keyboard here if needed
        }
    }
    else if (!string.IsNullOrEmpty(inputField.text) && currentFieldIndex < inputFields.Count - 1)
    {
        // Move to the next input field when a character is entered
        inputFields[currentFieldIndex + 1].text = " "; // Pre-fill with a space
        inputFields[currentFieldIndex + 1].Select();
        inputFields[currentFieldIndex + 1].caretPosition = 0; // Move the caret to the beginning
        if (currentFieldIndex + 1 != inputFields.Count) OpenKeyboard();
    }

    // Enable the next button only when all input fields are filled
    if (inputFields.All(field => field.text.Trim() != ""))
    {
        NextButton.interactable = true; // Enable the next button when all fields are filled
        fakeNextButton.interactable = true; // Enable the fake next button when all fields are filled
        CloseKeyboard();
    }
    else
    {
        NextButton.interactable = false; // Disable the next button when not all fields are filled
        fakeNextButton.interactable = false; // Disable the fake next button when not all fields are filled
    }
}



        public void OpenKeyboard()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("openKeyboard");
#endif
        }
        public void CloseKeyboard()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("closeKeyboard");
#endif
        }
        // private IEnumerator ActivateNextInputField(int nextFieldIndex)
        // {
        //     // yield return new WaitForSeconds(0.1f); // Small delay to ensure the focus transition is smooth
        //     // // inputFields[nextFieldIndex].ActivateInputField();
        //     // inputFields[nextFieldIndex].Select();
        //     // yield return new WaitForSeconds(0.1f);
        //     inputFields[nextFieldIndex].Select();
        //     if (nextFieldIndex != inputFields.Count) OpenKeyboard();
        //
        //     // inputFields[nextFieldIndex].caretPosition = 0;
        // }

        private void OnNextButtonClicked()
        {
            IsSentenceCorrect();
        }

        private void OnFakeNextButtonClicked()
        {
            IsSentenceCorrect();
        }

        private void IsSentenceCorrect()
        {
            bool isSentenceCorrect = true;

            for (int i = 0; i < inputFields.Count; i++)
            {
                if (inputFields[i].text != correctChars[i])
                {
                    Debug.Log($"Character '{inputFields[i].text}' is incorrect. Expected '{correctChars[i]}'.");
                    ActivateFailedMessage();
                    isSentenceCorrect = false;
                    break;
                }
            }

            if (isSentenceCorrect)
            {
                Debug.Log("Sentence is correct!");
                EventManager.AssignmentCompleted.Invoke();
                NextButton.interactable = true; // Make the next button interactable
                NextButton.gameObject.SetActive(true); // Show the real next button
                fakeNextButton.gameObject.SetActive(false); // Hide the fake next button
                LoadNextScene(); // Move to the next scene
            }
            else
            {
                incorrectTries++;
                ClearFields();
                Init();
                if (incorrectTries > 1)
                {
                    fakeNextButton.gameObject.SetActive(false); // Hide the fake next button
                    NextButton.gameObject.SetActive(true); // Show the real next button
                    NextButton.interactable = true; // Make the real next button non-interactable
                }
                else
                {
                    fakeNextButton.gameObject.SetActive(true); // Show the fake next button
                    fakeNextButton.interactable = false; // Make the fake next button non-interactable
                    NextButton.gameObject.SetActive(false); // Hide the real next button
                    //StartCoroutine(EnableNextButtonAfterDelay(1f)); // Make the real next button interactable after 1 second
                }
            }
        }

        private IEnumerator EnableNextButtonAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            NextButton.interactable = true;
        }

        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }

        private void ActivateFailedMessage()
        {
            if (incorrectTries == 0) failedGO.SetActive(true);
            else
            {
                //failedGO.SetActive(false);
                failedAgainGO.SetActive(true);
            }
            foreach (InputField inputField in inputFields)
            {
                bool parseSuccess = ColorUtility.TryParseHtmlString("#FF4050", out Color newCol);
                inputField.image.color = newCol;
            }
        }

        public void ClearFields()
        {
            foreach (var obj in instantiatedObjects)
            {
                Destroy(obj);
            }
            instantiatedObjects.Clear();
            inputFields.Clear();
            correctChars.Clear();
        }

        [ContextMenu("Initialize")]
        public void InitializeFromInspector()
        {
            Init();
        }
    }
}
