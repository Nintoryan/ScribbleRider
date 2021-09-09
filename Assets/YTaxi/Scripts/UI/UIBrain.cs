using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YTaxi;
using YTaxi.Data;
using YTaxi.Drawing;

public class UIBrain : MonoBehaviour
{
    [SerializeField] private ProgressBar _progress;
    [SerializeField] private Car _car;
    [SerializeField] private Finish _finish;
    [SerializeField] private GameObject _endGameCanvas;
    [SerializeField] private GameObject _drawingCanvas;
    [SerializeField] private GameObject _gamePlayCanvas;
    [SerializeField] private GameObject _menuCanvas;
    

    private void Start()
    {
        _progress.Initialize(_car.Model.transform.position.x,_finish.transform.position.x);
        _car.OnFirstWheelSet += () =>
        {
            _menuCanvas.SetActive(false);
        };
        _finish.OnFinished += OpenEndGameCanvas;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_car == null)
        {
            _car = FindObjectOfType<Car>();
        }

        if (_finish == null)
        {
            _finish = FindObjectOfType<Finish>();
        }
        
    }
#endif

    private void OpenEndGameCanvas()
    {
        _gamePlayCanvas.SetActive(false);
        _drawingCanvas.SetActive(false);
        _endGameCanvas.SetActive(true);
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("YTaxi/Scenes/Shop");
    }
    
    private void Update()
    {
        _progress.PassCurrent(_car.Model.transform.position.x);
    }

    public void NextLevel()
    {
        PlayerData.LevelNumber++;
        SceneManager.LoadScene($"YTaxi/Scenes/Level{PlayerData.LevelNumber%7 + 1}");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        Time.timeScale = 1.4f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
