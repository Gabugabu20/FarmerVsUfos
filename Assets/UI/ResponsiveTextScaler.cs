using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResponsiveTextScaler : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label descriptionText;
    private Label titleText;
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
        
    void Start()
    {
        var root = uiDocument.rootVisualElement;

        descriptionText = root.Q<Label>("description-text");
        titleText = root.Q<Label>("title-text");
        startButton = root.Q<Button>("start-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");

        AdjustFontSize();
    }

    void AdjustFontSize()
    {
        float screenWidth = Screen.width;

        float scaleFactor = screenWidth / 1920f;
        int textSize = Mathf.RoundToInt(30 * scaleFactor);
        int titleSize = Mathf.RoundToInt(40 * scaleFactor);

        if (titleText != null)
        {
            titleText.style.fontSize = titleSize;
        }

        if (descriptionText != null)
        {
            descriptionText.style.fontSize = textSize;
        }

        if (startButton != null)
        {
            startButton.style.fontSize = textSize;
        }

        if (settingsButton != null)
        {
            settingsButton.style.fontSize = textSize;
        }

        if (quitButton != null)
        {
            quitButton.style.fontSize = textSize;
        }
    }
}
