using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Screens.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        [SerializeField] private Animator keyboardAnimator;
        [SerializeField] private Animator keyboardAnimator2;


        private List<string> correctChars = new();
        private List<GameObject> instantiatedObjects = new();
        private int incorrectTries;

        private bool keyboardActive;
        
        private bool isKeyboardActive;
        
        private List<TMP_InputField> inputFields = new();
        private int currentlySelectedInputFieldIndex = -1;
        private TMP_InputField currentlySelectedInputField;

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
            SetupNextButtons();
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

        private void SetupNextButtons()
        {
            NextButton.interactable = false;
            NextButton.onClick.AddListener(OnNextButtonClicked);
            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
            fakeNextButton.gameObject.SetActive(true);
            NextButton.gameObject.SetActive(false);
        }

        public void Init()
        {
            if (!tmproInputFieldPrefab || !textPrefab || !wordParentPrefab) return;
            if (wordsWithMissingChars == null || wordsWithMissingChars.Count == 0) return;

            ShowKeyboard(false);

            CreateSentence();

            StartCoroutine(AddListenersAfterPrefill());
        }

        private void CreateSentence()
        {
            foreach (WordWithMissingChars wordWithMissingChars in wordsWithMissingChars)
            {
                string word = wordWithMissingChars.word;
                List<char> missingChars = wordWithMissingChars.missingChars;
                Transform inputFieldParent = wordWithMissingChars.lineTransform;

                if (string.IsNullOrEmpty(word) || missingChars == null || missingChars.Count == 0 ||
                    !inputFieldParent) continue;

                CreateWord(inputFieldParent, word, missingChars);
            }
        }

        private void CreateWord(Transform inputFieldParent, string word, List<char> missingChars)
        {
            GameObject wordParentGO = CreateWordParent(inputFieldParent, word);
            foreach (char c in word)
            {
                if (missingChars.Contains(c))
                    CreateMissingChar(wordParentGO, c);
                else
                    CreateExistingChar(wordParentGO, c);
            }
        }

        private void CreateExistingChar(GameObject wordParentGO, char c)
        {
            GameObject textGO = Instantiate(textPrefab, wordParentGO.transform);
            instantiatedObjects.Add(textGO);
            TextMeshProUGUI textComponent = textGO.GetComponent<TextMeshProUGUI>();
            textComponent.text = c.ToString();
        }

        private void CreateMissingChar(GameObject wordParentGO, char c)
        {
            GameObject inputFieldGO = Instantiate(tmproInputFieldPrefab, wordParentGO.transform);
            instantiatedObjects.Add(inputFieldGO);
            TMP_InputField inputField = inputFieldGO.GetComponent<TMP_InputField>();
            inputField.characterLimit = 1;
            inputFields.Add(inputField);
            correctChars.Add(c.ToString());
        }

        private GameObject CreateWordParent(Transform inputFieldParent, string word)
        {
            GameObject wordParentGO = Instantiate(wordParentPrefab, inputFieldParent);
            instantiatedObjects.Add(wordParentGO);
            HorizontalLayoutGroup layoutGroup = wordParentGO.GetComponent<HorizontalLayoutGroup>() ??
                                                wordParentGO.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.reverseArrangement = true;
            float inputFieldWidth = tmproInputFieldPrefab.GetComponent<RectTransform>().rect.width;
            float totalWidth = word.Length * (inputFieldWidth + charOffset);
            layoutGroup.spacing = elementSpacing + charOffset;
            layoutGroup.padding = new RectOffset((int)(totalWidth / 2), (int)(totalWidth / 2), 0, 0);
            return wordParentGO;
        }

        private void ShowKeyboard(bool showKeyboard)
        {
            keyboardAnimator.SetBool(KeyboardIn, showKeyboard);
            keyboardAnimator2.SetBool(KeyboardIn, showKeyboard);
            isKeyboardActive = showKeyboard;
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
                inputField.onSelect.AddListener(_ =>
                {
                    OnInputFieldSelect(inputField);
                    inputField.onDeselect.AddListener(OnInputFieldDeSelect);
                });
            }
        }
        
        private void OnInputFieldSelect(TMP_InputField inputField)
        {
            currentlySelectedInputField = inputField;
            currentlySelectedInputFieldIndex = inputFields.IndexOf(inputField);
            GameManagerKB.Instance.textBox = inputField;
            ShowKeyboard(true);
        }

        private void OnInputFieldDeSelect(string arg0)
        {
            foreach (TMP_InputField inputField in inputFields)
            {
                if (inputField.IsActive()) return;
            }
        }


        public void OnKeyboardClick()
        {
            //  called by buttons on the keyboard to keep it active
            keyboardActive = true;
        }
        
        private void CheckIfAllFieldsAreFilled()
        {
            if (inputFields.All(field => field.text.Trim() != "" && field.text.Trim() != " "))
            {
                NextButton.interactable = true;
                fakeNextButton.interactable = true;
                if (currentlySelectedInputFieldIndex == inputFields.Count - 1)
                    ShowKeyboard(false);
            }
            else
            {
                NextButton.interactable = false;
                fakeNextButton.interactable = false;
            }
        }
        
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
                if (inputFields[i].text == correctChars[i]) continue;
                ActivateFailedMessage();
                isSentenceCorrect = false;
                break;
            }

            if (isSentenceCorrect)
            {
                EventManager.AssignmentCompleted.Invoke();
                NextButton.interactable = true;
                NextButton.gameObject.SetActive(true);
                fakeNextButton.gameObject.SetActive(false);
                LoadNextScene();
            }
            else
            {
                incorrectTries++;
                ClearFields();
                Init();
                if (incorrectTries > 1)
                {
                    fakeNextButton.gameObject.SetActive(false);
                    NextButton.gameObject.SetActive(true);
                    NextButton.interactable = true;
                }
                else
                {
                    fakeNextButton.gameObject.SetActive(true); 
                    fakeNextButton.interactable = false;
                    NextButton.gameObject.SetActive(false);
                }
            }
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
                failedAgainGO.SetActive(true);
            }

            foreach (TMP_InputField inputField in inputFields)
            {
                ColorUtility.TryParseHtmlString("#FF4050", out Color newCol);
                inputField.image.color = newCol;
            }
        }

        public void ClearFields()
        {
            foreach (GameObject obj in instantiatedObjects)
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

        public void DeletePreviousLetter()
        {
            if (currentlySelectedInputFieldIndex == 0) return;
            currentlySelectedInputFieldIndex--;
            currentlySelectedInputField = inputFields[currentlySelectedInputFieldIndex];
            currentlySelectedInputField.text = "";
            currentlySelectedInputField.ActivateInputField();
        }

        public void OnAddedLetter()
        {
            if (currentlySelectedInputField.text.Length > 1)
            {
                string s = currentlySelectedInputField.text.
                    Substring(currentlySelectedInputField.text.Length - 1, 1);
                currentlySelectedInputField.text = s;
            }
            if (currentlySelectedInputFieldIndex < inputFields.Count - 1)
            {
                currentlySelectedInputFieldIndex++;
                currentlySelectedInputField = inputFields[currentlySelectedInputFieldIndex];
                currentlySelectedInputField.ActivateInputField();
            }
            else ShowKeyboard(false);
            CheckIfAllFieldsAreFilled();
        }
    }
}