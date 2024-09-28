using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement root;
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        startButton = root.Q<Button>("start-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");

        if (startButton != null) startButton.clicked += OnStartButtonClick;
        if (settingsButton != null) settingsButton.clicked += OnSettingsButtonClick;
        if (quitButton != null) quitButton.clicked += OnQuitButtonClick;
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void OnSettingsButtonClick()
    {
        Debug.Log("Settings");
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
