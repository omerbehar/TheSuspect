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
        public List<char> missingChars;

        [FormerlySerializedAs("inputFieldParent")]
        public Transform lineTransform;
    }

    public class Screen8StringCheck : ScreenBase
    {

         private static readonly int KeyboardIn = Animator.StringToHash("keyboardIn");
        // [SerializeField] private GameObject inputFieldPrefab;
        [SerializeField] private GameObject tmproInputFieldPrefab;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private GameObject wordParentPrefab;
        [SerializeField] private List<WordWithMissingChars> wordsWithMissingChars;
        [SerializeField] private GameObject failedGO;
        [SerializeField] private GameObject failedAgainGO;
        [SerializeField] private float elementSpacing = 2f;
        [SerializeField] private float charOffset = 10f;
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private Button fakeNextButton;
        // [SerializeField] private ScrollRect scrollView;

        [SerializeField] private Animator keyboardAnimator;
        [SerializeField] private Animator keyboardAnimator2;


        private List<TMP_InputField> inputFields = new();
        private List<string> correctChars = new();
        private List<GameObject> instantiatedObjects = new();
        private int incorrectTries;

          private bool keyboardActive;

        protected override void Start()
        {
            base.Start();
            
            if (initializeOnStart)
            {
                Init();
#if !UNITY_EDITOR && UNITY_WEBGL
                WebGLInput.mobileKeyboardSupport = true;
#endif
            }

            NextButton.onClick.AddListener(OnNextButtonClicked);
            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
            fakeNextButton.gameObject.SetActive(true);
            NextButton.gameObject.SetActive(false);
        }

        public void Init()
        {
            NextButton.interactable = false;
            if ( !tmproInputFieldPrefab || !textPrefab || !wordParentPrefab) return;

            if (wordsWithMissingChars == null || wordsWithMissingChars.Count == 0) return;

            keyboardAnimator.SetBool(KeyboardIn, false);
            keyboardAnimator2.SetBool(KeyboardIn, false);
            
            foreach (WordWithMissingChars wordWithMissingChars in wordsWithMissingChars)
            {
                string word = wordWithMissingChars.word;
                List<char> missingChars = wordWithMissingChars.missingChars;
                Transform inputFieldParent = wordWithMissingChars.lineTransform;

                if (string.IsNullOrEmpty(word) || missingChars == null || missingChars.Count == 0 ||
                    !inputFieldParent) continue;

                GameObject wordParentGO = Instantiate(wordParentPrefab, inputFieldParent);
                instantiatedObjects.Add(wordParentGO);

                HorizontalLayoutGroup layoutGroup = wordParentGO.GetComponent<HorizontalLayoutGroup>() ??
                                                    wordParentGO.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.reverseArrangement = true;
                float inputFieldWidth = tmproInputFieldPrefab.GetComponent<RectTransform>().rect.width;
                
                float totalWidth =
                    word.Length * (inputFieldWidth + charOffset);
                
                layoutGroup.spacing = elementSpacing + charOffset;
                layoutGroup.padding =
                    new RectOffset((int)(totalWidth / 2), (int)(totalWidth / 2), 0, 0);

                foreach (char c in word)
                {
                    if (missingChars.Contains(c))
                    {
                        GameObject inputFieldGO = Instantiate(tmproInputFieldPrefab , wordParentGO.transform);
                        instantiatedObjects.Add(inputFieldGO);
                        TMP_InputField inputField = inputFieldGO.GetComponent<TMP_InputField>();
                        inputField.characterLimit = 1;
                        inputField.text = " ";
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

             StartCoroutine(AddListenersAfterPrefill());
        }

        private IEnumerator AddListenersAfterPrefill()
        {
            yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds after prefill has finished
            AddListeners();
        }

        private void AddListeners()
        {
            foreach (TMP_InputField inputField in inputFields)
            {
               inputField.onSelect.AddListener((string arg) =>
            {
                
                OnInputFieldSelect(arg, inputField);
            });
                inputField.onDeselect.AddListener(OnInputFieldDeSelect);
                
            }
        }

       private int currentlySelectedInputFieldIndex = -1;

private TMP_InputField currentlySelectedInputField;


private void OnInputFieldSelect(string arg0, TMP_InputField inputField)
{
    currentlySelectedInputField = inputField;
    currentlySelectedInputFieldIndex = inputFields.IndexOf(inputField);
GameManagerKB.Instance.textBox = inputField;
    // Keep the keyboard open
    keyboardAnimator.SetBool(KeyboardIn, true);
    keyboardAnimator2.SetBool(KeyboardIn, true);
}

private void OnInputFieldDeSelect(string arg0)
{
   
}

         public void OnKeyboardClick()
        {
            // Function to be called by buttons on the keyboard to keep it active
            keyboardActive = true;
        }

private void OnFieldValueChanged(TMP_InputField inputField)
{
    int currentFieldIndex = inputFields.IndexOf(inputField);
    Debug.Log("Current field index: " + currentFieldIndex);

    // Limit input to only 1 character
    if (inputField.text.Length > 1)
    {
        inputField.text = inputField.text.Substring(0, 1);
    }

    // Handle deletion logic
    if (string.IsNullOrEmpty(inputField.text.Trim()) || inputField.text == " ")
    {
        inputField.text = " "; // Ensure the field shows " " when empty

        // Move to the previous field only if this is not the first field
        if (currentFieldIndex > 0)
        {
            inputFields[currentFieldIndex - 1].ActivateInputField();
            inputFields[currentFieldIndex - 1].caretPosition = inputFields[currentFieldIndex - 1].text.Length; // Set caret to the end
        }
    }
    else
    {
        // Do not move forward if on the last field
        if (currentFieldIndex < inputFields.Count - 1)
        {
            // Move focus to the next field if valid input exists
            inputFields[currentFieldIndex + 1].ActivateInputField();
            inputFields[currentFieldIndex + 1].caretPosition = 0;
        }
        else
        {
            // Stay in the last field
            inputField.ActivateInputField();
            inputField.caretPosition = inputField.text.Length;
        }
    }

    // Keep the keyboard animators open
    keyboardAnimator.SetBool(KeyboardIn, true);
    keyboardAnimator2.SetBool(KeyboardIn, true);

    // Check if all fields are filled
    if (inputFields.All(field => field.text.Trim() != "" && field.text.Trim() != " "))
    {
        NextButton.interactable = true;
        fakeNextButton.interactable = true;
    }
    else
    {
        NextButton.interactable = false;
        fakeNextButton.interactable = false;
    }
}








        private bool isKeyboardClosed = true;

    public void OpenKeyboard()
{
    isKeyboardClosed = false;
#if UNITY_WEBGL && !UNITY_EDITOR
    Application.ExternalCall("openKeyboard");
#elif UNITY_ANDROID
    AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
    AndroidJavaObject InputMethodManager = new AndroidJavaObject("android.view.inputmethod.InputMethodManager");
    InputMethodManager.Call("showSoftInput", View, 0);
#endif

    // Get the keyboard height
    float keyboardHeight = GetKeyboardHeight();

    // Find the main camera
    Camera mainCamera = Camera.main;

    // Adjust the position of the main camera based on the keyboard height
    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - keyboardHeight, mainCamera.transform.position.z);
}

public void CloseKeyboard()
{
    if (!isKeyboardClosed)
    {
        isKeyboardClosed = true;
        Debug.Log("closing keyboard");
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("closeKeyboard");
#endif

        // Get the keyboard height
        float keyboardHeight = GetKeyboardHeight();

        // Find the main camera
        Camera mainCamera = Camera.main;

        // Reset the position of the main camera based on the keyboard height
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + keyboardHeight, mainCamera.transform.position.z);
    }
}
private float GetKeyboardHeight()
{
    if (Application.platform == RuntimePlatform.Android)
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
            AndroidJavaObject InputMethodManager = new AndroidJavaObject("android.view.inputmethod.InputMethodManager");
            return InputMethodManager.Call<float>("getInputMethodWindowVisibleHeight", View);
        }
    }
    else
    {
        return Screen.height * 0.2f; // Use a default value for other platforms
    }
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
                //CloseKeyboard();
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
                    NextButton.gameObject.SetActive(true);
                    //CloseKeyboard(); // Show the real next button
                    NextButton.interactable = true; // Make the real next button non-interactable
                }
                else
                {
                    fakeNextButton.gameObject.SetActive(true); // Show the fake next button
                    fakeNextButton.interactable = false;
                    //CloseKeyboard(); // Make the fake next button non-interactable
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
            CloseKeyboard();
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

            foreach (TMP_InputField inputField in inputFields)
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