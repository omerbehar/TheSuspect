using System;
using System.Threading.Tasks;
using DataLayer;
using Screens.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Screens.Bases
{
    public class ScreenBase : MonoBehaviour, IBackButton, INextButton
    {        
        [field: SerializeField] public Button BackButton { get; set; }
        [field: SerializeField] public Button NextButton { get; set; }
        [field: SerializeField] public Button HintButton { get; set; }
        [field: SerializeField] public Button CloseButton { get; set; }
        [field: SerializeField] public GameObject TargetObject { get; set; }
        protected int answersCount;

        private void Awake()
        { 
            if (!Data.SelectedAnswersData.ContainsKey(SceneManager.GetActiveScene().name))
                Data.SelectedAnswersData.Add(SceneManager.GetActiveScene().name, new bool[answersCount]);
        }

        protected virtual void Start()
        {
            InitListeners();
            NextButton.interactable = true;
            if (TargetObject)
                TargetObject.SetActive(false);
        }

        private void InitListeners()
        {
            EventManager.AssignmentCompleted.AddListener(OnAssigmentCompleted);
            EventManager.AssignmentNotCompleted.AddListener(OnAssignmentNotCompleted);
            if (BackButton == null)
            {
                //Debug.LogWarning("BackButton is null");
            }            
            else
                BackButton.onClick.AddListener(OnBackButtonClicked);
            if (NextButton == null)
            {
                //Debug.LogWarning("NextButton is null");
            }   
            else
            {
                NextButton.onClick.AddListener(OnNextButtonClicked);
                NextButton.interactable = false;
            }
            if (HintButton == null)
            {
                //Debug.LogWarning("HintButton is null");
            }     
            else
                HintButton.onClick.AddListener(OnHintButtonClicked);
            if (CloseButton == null)
            {
                //Debug.LogWarning("CloseButton is null");
            }
            else
                CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnAssigmentCompleted()
        {
            NextButton.interactable = true;
        }
        private void OnAssignmentNotCompleted()
        {
            NextButton.interactable = false;
        }

        public void OnBackButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        public virtual void OnNextButtonClicked()
        {
            //Debug.Log("Next button clicked");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnHintButtonClicked()
        {
            if (TargetObject != null)
            {
                TargetObject.SetActive(!TargetObject.activeSelf);
            }
            else
            {
                //Debug.LogWarning("TargetObject is null");
            }
        }

        public void OnCloseButtonClicked()
        {
            if (TargetObject != null)
            {
                TargetObject.SetActive(false);
            }
            else
            {
                //Debug.LogWarning("TargetObject is null");
            }
        }
    }
}