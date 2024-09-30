using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement root;
    private Button level1Button;
    private Button level2Button;
    private Button quitButton;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        level1Button = root.Q<Button>("level1-button");
        level2Button = root.Q<Button>("level2-button");
        quitButton = root.Q<Button>("quit-button");

        if (level1Button != null) level1Button.clicked += OnLevel1ButtonClick;
        if (level2Button != null) level2Button.clicked += OnLevel2ButtonClick;
        if (quitButton != null) quitButton.clicked += OnQuitButtonClick;
    }

    private void OnLevel1ButtonClick()
    {
        SceneManager.LoadScene("Level_1");
    }

    private void OnLevel2ButtonClick()
    {
        SceneManager.LoadScene("Level_2");
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
