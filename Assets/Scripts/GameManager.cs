using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainUI;
    [SerializeField] private UIDocument endGameUI;
    [SerializeField] private UIDocument pauseUI;
    [SerializeField] private GameBGMusic backgroundMusic;

    [SerializeField] private float levelDuration = 300f;

    private FirstPersonController fpsController;
    private Weapon weapon;

    private Label cowTextLabel;
    private Label timerTextLabel;
    private Label resultTextLabel;
    private Button returnButton;
    private Button quitButton;
    private Button backButton;
    private Button pauseReturnButton;

    private float remainingTime;
    private bool isGameRunning = false;
    private bool isGamePaused = false;

    private List<AudioSource> pausedAudioSources = new List<AudioSource>();

    void Start()
    {
        fpsController = FindObjectOfType<FirstPersonController>();
        weapon = FindObjectOfType<Weapon>();

        var root = mainUI.rootVisualElement;

        cowTextLabel = root.Q<Label>("cow-text");
        timerTextLabel = root.Q<Label>("timer-text");

        remainingTime = levelDuration;
        isGameRunning = true;

        UpdateUI();

        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        endGameUI.rootVisualElement.style.display = DisplayStyle.None;

        SetupPauseUI();
        SetupEndGameUI();
    }

    private void SetupPauseUI()
    {
        var pauseRoot = pauseUI.rootVisualElement;

        backButton = pauseRoot.Q<Button>("back-button");
        pauseReturnButton = pauseRoot.Q<Button>("return-button");

        backButton.clicked += ResumeGame;
        pauseReturnButton.clicked += ReturnToTitle;
    }


    private void SetupEndGameUI()
    {
        var endUIRoot = endGameUI.rootVisualElement;

        resultTextLabel = endUIRoot.Q<Label>("result-text");
        returnButton = endUIRoot.Q<Button>("returntitle-button");
        quitButton = endUIRoot.Q<Button>("quit-button");

        returnButton.clicked += ReturnToTitle;
        quitButton.clicked += QuitGame;
    }

    void Update()
    {
        if (isGameRunning)
        {
            remainingTime -= Time.deltaTime;

            UpdateUI();

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isGameRunning = false;
                EndLevel();
            }

            if (CowManager.Instance.GetCows().Count == 0)
            {
                isGameRunning = false;
                EndLevel();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        isGamePaused = true;
        isGameRunning = false;

        Time.timeScale = 0f;

        if (fpsController != null)
        {
            fpsController.DisableInput();
        }

        if (weapon != null)
        {
            weapon.SetCanShoot(false);
        }

        backgroundMusic.PauseMusic();
        PauseAllAudio();

        mainUI.rootVisualElement.style.display = DisplayStyle.None;
        pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        isGameRunning = true;

        Time.timeScale = 1f;

        if (fpsController != null)
        {
            fpsController.EnableInput();
        }

        if (weapon != null)
        {
            weapon.SetCanShoot(true);
        }

        backgroundMusic.ResumeMusic();
        ResumeAllPausedAudio();

        mainUI.rootVisualElement.style.display = DisplayStyle.Flex;
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void PauseAllAudio()
    {
        pausedAudioSources.Clear();
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                pausedAudioSources.Add(audioSource);
            }
        }
    }

    public void ResumeAllPausedAudio()
    {
        foreach (var audioSource in pausedAudioSources)
        {
            audioSource.UnPause();
        }
    }

    private void UpdateUI()
    {
        int cowsRemaining = CowManager.Instance.GetCows().Count;
        cowTextLabel.text = $"{cowsRemaining}";

        int minutes = Mathf.FloorToInt(remainingTime / 60F);
        int seconds = Mathf.FloorToInt(remainingTime % 60F);
        int milliseconds = Mathf.FloorToInt((remainingTime * 100F) % 100F);
        timerTextLabel.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private void EndLevel()
    {
        isGameRunning = false;
        Time.timeScale = 0f;

        if (fpsController != null)
        {
            fpsController.DisableInput();
        }

        if (weapon != null)
        {
            weapon.SetCanShoot(false);
        }

        backgroundMusic.PauseMusic();
        PauseAllAudio();

        mainUI.rootVisualElement.style.display = DisplayStyle.None;
        endGameUI.rootVisualElement.style.display = DisplayStyle.Flex;

        int cowsRemaining = CowManager.Instance.GetCows().Count;
        string resultText = "";
        if (cowsRemaining == 0)
        {
            resultText = "GAME OVER!\n The UFOs were too fast for you... but don't give up and try to save your cows again!";
        }
        else
        {
            resultText = "Well Done partner!\n You were a real Cowboy and showed these stupid UFOs what you were made of!\n";
            if (cowsRemaining == 16)
            {
                resultText += "HOLY MOLY! You saved all 16 cows!\nNow this is what I call SKILL!";
            }
            else if (cowsRemaining > 10)
            {
                resultText += $"GOLLY! You saved {cowsRemaining} cows!\nCouldn’t save ‘em all, but hey, that's a whole herd!";
            }
            else if (cowsRemaining > 1)
            {
                resultText += $"You saved {cowsRemaining} cows!\nNot bad, but I bet you can milk that score even higher!";
            }
            else
            {
                resultText += "Well, you saved just 1 cow...\nGuess that’s one lonely moo for this Christmas...";
            }
        }

        resultTextLabel.text = resultText;

        if (fpsController != null)
        {
            fpsController.DisableInput();
        }
    }

    private void ReturnToTitle()
    {
        ResumeGame();
        SceneManager.LoadScene("HomeMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
    }

}
