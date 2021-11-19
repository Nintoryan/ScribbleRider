using System;
using UnityEngine;
using UnityEngine.UI;

namespace YTaxi.Scripts.UI
{
    public class Onboarding : MonoBehaviour
    {
        [SerializeField] private GameObject _ui;
        [SerializeField] private GameObject _drawingCanvas;

        [SerializeField] private GameObject _onboarding;
        [SerializeField] private Button _continue;

        private void OnEnable()
        {
            _continue.onClick.AddListener(OnContinueClicked);
        }

        private void Start()
        { 
            _ui.SetActive(false); 
            _drawingCanvas.SetActive(false);
        }

        private void OnDisable()
        {
            _continue.onClick.RemoveListener(OnContinueClicked);
        }

        private void OnContinueClicked()
        {
            _ui.SetActive(true);
            _drawingCanvas.SetActive(true);
            
            _onboarding.SetActive(false);
        }
    }
}