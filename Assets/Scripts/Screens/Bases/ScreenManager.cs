using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Screens.Bases
{
    public class ScreenManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private List<Question> questions = new List<Question>();

        private int currentQuestionIndex = 0;
        private ListView answerListView;

        [SerializeField] private float itemHeight = 60f;
        [SerializeField] private float SPACING = 10f; 
        [SerializeField] private float itemSpacing = 10f;

        private int correctCounter;
        private List<int> selectedIndices = new List<int>();

        #endregion
    

        #region UI Setup
        // Retrieves the root visual element from the UI Document
        private VisualElement GetRootVisualElement()
        {
            return FindObjectOfType<UIDocument>().rootVisualElement;
        }

        // Setup the ListView for displaying answers
        private void SetUpListView()
        {
            answerListView = new ListView(questions, itemHeight + SPACING, MakeItem, BindItem);

            var scrollView = answerListView.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;

            answerListView.selectionType = SelectionType.None;
        }
        public void AdjustListViewHeight()
        {
            var totalItemHeight = itemHeight * questions[currentQuestionIndex].GetAnswerOptions().Count;
            var totalSpacing = SPACING * (questions[currentQuestionIndex].GetAnswerOptions().Count - 1);
            var totalHeight = totalItemHeight + totalSpacing;
    
            answerListView.style.height = totalHeight;
        }

        // Create a visual element with spacer for UI
        private VisualElement CreateContainerWithSpacer(VisualElement vo)
        {
            var container = new VisualElement();
            container.Add(vo);

            var spacer = new VisualElement
            {
                style = {height = itemSpacing, backgroundColor = Color.clear}
            };

            container.Add(spacer);
            return container;
        }

        #endregion
    
    
        #region game management 

        // Initialization of UI event handlers
        public void Start()
        {
            InitializeListView();
            DisplayCurrentQuestion();

            var root = GetRootVisualElement();
            var continueButton = root.Q<Button>("ContinueButton");
            if (continueButton != null)
            {
                continueButton.clicked += ContinueButtonClicked;
            }

            // Adding this for your BackButton
            var backButton = root.Q<Button>("BackButton");
            if (backButton != null)
            {
                backButton.clicked += BackButtonClicked;
            }
        }

        public void BackButtonClicked()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex > 0)
            {
                SceneManager.LoadScene(currentSceneIndex - 1);
            }
            else
            {
                //Debug.Log("Cannot go back further, already at first scene.");
            }
        }
        
        

        // Handle the event of Continue button being clicked
        public void ContinueButtonClicked()
        {
            if (currentQuestionIndex + 1 < questions.Count)
            {
                currentQuestionIndex++;
                DisplayCurrentQuestion();
            }
            else
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                if(currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                else
                {
                    //Debug.Log("No more scenes");
                }
            }
        }
        
        // Display the current question on the UI
        public void DisplayCurrentQuestion()
        {
         
            // No more questions to display
            if (currentQuestionIndex >= questions.Count)
            {
                //Debug.Log("No more questions");
                return;
            }

            // Get current question
            var currentQuestion = questions[currentQuestionIndex];

            // Fetch needed UI Elements
            var root = GetRootVisualElement();
            var questionLabel = root.Q<Label>("QuestionLabel");

            // Set question text
            string hebrewQuestionText = currentQuestion.QuestionText;
            questionLabel.text = ReverseHebrewText(hebrewQuestionText);
            questionLabel.style.unityTextAlign = UnityEngine.TextAnchor.MiddleRight;

            // Adjust questionLabel's style to fill its parent
            questionLabel.style.height = Length.Percent(31);
           
            questionLabel.style.whiteSpace = WhiteSpace.Normal;


            // Reset ListView
            ResetListView();

            // Supply ListView with new data source
            answerListView.itemsSource = currentQuestion.GetAnswerOptions();

            if (currentQuestion.Type == QuestionType.SingleChoice)
            {
                answerListView.makeItem = MakeSingleChoiceAnswerVO;
                answerListView.bindItem = BindItemToSingleChoiceAnswerVO;
            }
            else if (currentQuestion.Type == QuestionType.MultipleChoice)
            {
                answerListView.makeItem = MakeMultiChoiceAnswerVO;
                answerListView.bindItem = BindItemToMultiChoiceAnswerVO;
            }
            else if (currentQuestion.Type == QuestionType.OpenEnded)
            {
                answerListView.Clear(); // Clear the old elements
                answerListView.makeItem = MakeOpenEndedAnswerVO;
                answerListView.bindItem = BindItemToOpenEndedAnswerVO;
                answerListView.itemsSource = currentQuestion.GetAnswerOptions();
               
            }
            else
            {
                throw new NotImplementedException($"Display for question type {currentQuestion.Type} is not implemented");
            }
    
            // Refresh the ListView
            answerListView.Rebuild();
        }
        
        private string ReverseHebrewText(string text)
        {
            var reversedTextArray = text.ToCharArray();
            Array.Reverse(reversedTextArray);
            return new string(reversedTextArray);
        }

        // Initializes the ListView by locating it in the UI and setting it up
        private void InitializeListView()
        {
            var root = GetRootVisualElement();

            SetUpListView();

            var scrollView = answerListView.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;

            answerListView.selectionType = SelectionType.None;

            root.Q<VisualElement>("Main").Add(answerListView);
        }
    
// Reset the ListView by removing the old one and setting up a new one
        public void ResetListView()
        {
            var root = GetRootVisualElement();
            var mainVisualElement = root.Q<VisualElement>("Main");

            if (answerListView != null) mainVisualElement.Remove(answerListView);

            SetUpListView();

            mainVisualElement.Add(answerListView);
        }
        #endregion

    
        #region item management
    
        // Checking if the current question is answered
        private VisualElement MakeItem()
        {
            return Enum.IsDefined(typeof(QuestionType), questions[currentQuestionIndex].Type)
                ? MakeSingleChoiceAnswerVO()
                : throw new NotImplementedException($"List item creation not implemented for question type {questions[currentQuestionIndex].Type}");
        }
    
        // Bind a data item to the visual element for the ListView
        private void BindItem(VisualElement element, int index)
        {
            if (Enum.IsDefined(typeof(QuestionType), questions[currentQuestionIndex].Type))
            {
                BindItemToSingleChoiceAnswerVO(element, index);
            }
            else
            {
                throw new NotImplementedException($"List item binding not implemented for question type {questions[currentQuestionIndex].Type}");
            }
        }
        
        private VisualElement MakeOpenEndedAnswerVO()
        {
            var openEndedAnswerVO = new OpenEndedAnswerVO { onSubmit = HandleOpenEndedAnswerSubmit };
            return CreateContainerWithSpacer(openEndedAnswerVO);
        }

        private void HandleOpenEndedAnswerSubmit(string userAnswer)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            // Suppose the CheckAnswer method for open-ended question checks if userAnswer is equal to the correct answer
            bool isCorrectAnswer = currentQuestion.CheckAnswer(userAnswer);

            if (isCorrectAnswer)
            {
                //Debug.Log("Open-ended answer is correct!");
                // Perform actions related to correct answer...
            }
            else
            {
                //Debug.Log("Open-ended answer is incorrect.");
                // Perform actions related to incorrect answer...
            }
        }

        private void BindItemToOpenEndedAnswerVO(VisualElement element, int index)
        {
            var container = element as VisualElement;
            if (container[0] is OpenEndedAnswerVO openEndedAnswerVO)
            {
                openEndedAnswerVO.answerInputField.value = "";
            }
            else
            {
                throw new Exception("Element is not of type OpenEndedAnswerVO");
            }
        }

        public void HandleOpenEndedAnswerSubmit(int[] answerText)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            bool isCorrectAnswer = currentQuestion.CheckAnswer(answerText);
            Debug.Log(isCorrectAnswer ? "Correct Answer!" : "Incorrect Answer!");
        }
    
        // Method to create a single choice answer Visual Object for ListView item
        private VisualElement MakeSingleChoiceAnswerVO()
        {
            var singleChoiceAnswerVO = new SingleChoiceAnswerVO {onAnswerClicked = HandleAnswerClicked};
            return CreateContainerWithSpacer(singleChoiceAnswerVO);
        }

        // Method to bind a single choice answer to a Visual Object in ListView
        private void BindItemToSingleChoiceAnswerVO(VisualElement element, int index)
        {
            var container = (VisualElement)element;
            if (container[0] is SingleChoiceAnswerVO singleChoiceAnswerVO)
            {
                singleChoiceAnswerVO.answerLabel.text = questions[currentQuestionIndex].GetAnswerOptions()[index];
                singleChoiceAnswerVO.OptionIndex = index;
            }
            else
            {
                throw new Exception("Element is not of type SingleChoiceAnswerVO");
            }
        }

    
        // Method to create a multi choice answer Visual Object for ListView item
        private VisualElement MakeMultiChoiceAnswerVO()
        {
            var multiChoiceAnswerVO = new MultiChoiceAnswerVO {onAnswerMultipleChoiceSelected = HandleAnswerMultipleChoiceClicked};
            return CreateContainerWithSpacer(multiChoiceAnswerVO);
        }

    
        // Method to bind a multi choice answer to a Visual Object in ListView
        private void BindItemToMultiChoiceAnswerVO(VisualElement element, int index)
        {
            var container = (VisualElement)element;
            if (container[0] is MultiChoiceAnswerVO multiChoiceAnswerVO)
            {
                multiChoiceAnswerVO.answerLabel.text = questions[currentQuestionIndex].GetAnswerOptions()[index];
                multiChoiceAnswerVO.OptionIndex = index;
            }
            else
            {
                throw new Exception("Element is not of type MultiChoiceAnswerVO");
            }
        }
        #endregion
    
    
        #region answer management 
        // Handling single choice answers
        public void HandleAnswerClicked(int answerIndex)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            var isCorrectAnswer = currentQuestion.CheckAnswer(new[] { answerIndex });
            if (isCorrectAnswer)
            {
                Debug.Log("Correct Answer! The selected index is: " + answerIndex);
            }
            else
            {
                Debug.Log("Incorrect Answer! The selected index is: " + answerIndex);
            }
        }

        // Checking all correct answers selected or not for the given question
        private bool AllCorrectAnswersSelected(MultipleChoiceQuestion question)
        {
            foreach (var correctIndex in question.correctAnswerIndices)
            {
                if (!selectedIndices.Contains(correctIndex))
                {
                    return false;
                }
            }

            return true;
        }

        // Handling the multiple choice answers
        public void HandleAnswerMultipleChoiceClicked(int answerIndex)
        {
            var currentQuestion = questions[currentQuestionIndex] as MultipleChoiceQuestion;

            if (selectedIndices.Contains(answerIndex))
            {
                selectedIndices.Remove(answerIndex);
                Debug.Log($"Deselected multi-choice answer option, its index is: { answerIndex }");

                if (currentQuestion.correctAnswerIndices.Contains(answerIndex))
                {
                    correctCounter--;
                }
            }
            else
            {
                selectedIndices.Add(answerIndex);
                Debug.Log($"Selected multi-choice answer option, its index is: { answerIndex }");

                if (currentQuestion.correctAnswerIndices.Contains(answerIndex))
                {
                    correctCounter++;
                    Debug.Log("Correct Answer! Selected index is: " + answerIndex);
                }
                else
                {
                    Debug.Log("Incorrect Answer! Selected index is: " + answerIndex);
                }

                if (AllCorrectAnswersSelected(currentQuestion))
                {
                    Debug.Log("All correct answers have been selected");
                }
            }
        }
  

   

        private bool CurrentQuestionIsAnswered()
        {
            Question currentQuestion = questions[currentQuestionIndex];

            switch (currentQuestion.Type)
            {
                case QuestionType.SingleChoice:
                    RadioButton radioButton = answerListView.Q<RadioButton>();
                    // If RadioButton exists and is selected
                    if (radioButton != null && radioButton.value)
                    {
                        return true;
                    }
                    break;

                case QuestionType.MultipleChoice:
                    foreach (Toggle checkbox in answerListView.Children().OfType<Toggle>())
                    {
                        // If any Checkbox (Toggle) is checked
                        if (checkbox.value)
                        {
                            return true;
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException($"No answer check implemented for question type {currentQuestion.Type}");
            }

            return false;
        }
        #endregion
    }
}