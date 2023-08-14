using System;
using DataLayer;
using DefaultNamespace;
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
        protected int answersCount;

        private void Awake()
        { 
            if (!Data.SelectedAnswersData.ContainsKey(SceneManager.GetActiveScene().name))
                Data.SelectedAnswersData.Add(SceneManager.GetActiveScene().name, new bool[answersCount]);
        }

        protected virtual void Start()
        {
            InitListeners();
        }

        private void InitListeners()
        {
            EventManager.AssignmentCompleted.AddListener(OnAssigmentCompleted);
            if (BackButton == null)
                Debug.LogWarning("BackButton is null");
            else
                BackButton.onClick.AddListener(OnBackButtonClicked);
            if (NextButton == null)
                Debug.LogWarning("NextButton is null");
            else
            {
                NextButton.onClick.AddListener(OnNextButtonClicked);
                NextButton.interactable = false;
            }
        }


        private void OnAssigmentCompleted()
        {
            NextButton.interactable = true;
        }

        public void OnBackButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        public virtual void OnNextButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}