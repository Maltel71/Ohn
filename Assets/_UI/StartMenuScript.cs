using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Cursor = UnityEngine.Cursor;

public class StartMenuScript : MonoBehaviour
{

    private UIDocument document;
    private VisualElement MainMenu;
    private VisualElement OptionsMenu;
    private VisualElement GameEndMenu;
    private VisualElement CredMenu;

    public AudioMixer mixer;

    private static bool isCreated = false;

    private Stack<VisualElement> menuStack = new Stack<VisualElement>();
    private VisualElement currentMenu;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        if (!isCreated)
        {
            DontDestroyOnLoad(this.gameObject);
            isCreated = true;
            Debug.LogError(this.gameObject + " is undestroyable");
        }
    }

    private void OnEnable()
    {
       
        //MainMenu
        MainMenu = document.rootVisualElement.Q("MainMenu");
        Button playButton = MainMenu.Q("Play") as Button;
        playButton.RegisterCallback<ClickEvent>(OnPlayClicked);

        Button optionsButton = MainMenu.Q("Options") as Button;
        optionsButton.RegisterCallback<ClickEvent>(OnOptionsClicked);

        Button credButton = MainMenu.Q("Credits") as Button;
        credButton.RegisterCallback<ClickEvent>(OnCredClicked);

        Button quitButton = MainMenu.Q("Quit") as Button;
        quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);

        //OptionsMenu
        OptionsMenu = document.rootVisualElement.Q("OptionsMenu");
        Slider masterSlider = OptionsMenu.Q("MasterSlider") as Slider;
        masterSlider.RegisterValueChangedCallback(OnMasterSliderChanged);

        Button backToMainButton = OptionsMenu.Q("Back") as Button;
        backToMainButton.RegisterCallback<ClickEvent>(OnBackClicked);

        //GameEndMenu
        GameEndMenu = document.rootVisualElement.Q("GameEndMenu");

        Button continueButton = GameEndMenu.Q("Continue") as Button;
        continueButton.RegisterCallback<ClickEvent>(OnContinueClicked);

        Button endCredButton = GameEndMenu.Q("Credits") as Button;
        endCredButton.RegisterCallback<ClickEvent>(OnCredClicked);

        Button endQuitButton = GameEndMenu.Q("Quit") as Button;
        endQuitButton.RegisterCallback<ClickEvent>(OnQuitClicked);

        //CredMenu
        CredMenu = document.rootVisualElement.Q("CreditsMenu");
        Button backToEndButton = CredMenu.Q("Back") as Button;
        backToEndButton.RegisterCallback<ClickEvent>(OnBackClicked);

        ShowMenu(MainMenu);
    }

    private void ShowMenu(VisualElement menuToShow)
    {
        MainMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.None;
        GameEndMenu.style.display = DisplayStyle.None;
        CredMenu.style.display = DisplayStyle.None;

        menuToShow.style.display = DisplayStyle.Flex;
        currentMenu = menuToShow;
    }

    public void OpenMenu(VisualElement newMenu)
    {
        if (currentMenu != null)
        {
            menuStack.Push(currentMenu);
        }
        ShowMenu(newMenu);
    }

    public void OnBackClicked(ClickEvent evt)
    {
        if (menuStack.Count > 0)
        {
            VisualElement previousMenu = menuStack.Pop();
            ShowMenu(previousMenu);
        }
        else
        {
            Debug.Log("No previous menu in stack!");
        }
    }

    private void OnPlayClicked(ClickEvent evt)
    {
        SceneManager.LoadScene(1);
        MainMenu.style.display = DisplayStyle.None;
        Debug.Log("You clicked the play button");
    }

    private void OnOptionsClicked(ClickEvent evt)
    {
        OpenMenu(OptionsMenu);
    }

    private void OnCredClicked(ClickEvent evt)
    {
        if (MainMenu.style.display == DisplayStyle.Flex)
        {
            OpenMenu(CredMenu);
        }
        else if (GameEndMenu.style.display == DisplayStyle.Flex)
        {
            OpenMenu(CredMenu);
        }
    }

    private void OnContinueClicked(ClickEvent evt)
    {
        GameEndMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        Application.Quit();
        Debug.LogError("Quitting game");
    }

    private void OnMasterSliderChanged(ChangeEvent<float> value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log(value.newValue) * 20);
        Debug.Log("MasterSlider Value Changed: " + value.newValue);
    }

    public void OnGameEnd()
    {
        StartCoroutine(GameEnd());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(2f);
        ShowMenu(GameEndMenu);
        Time.timeScale = 0f;
    }

    /*public void ClearHistory()
    {
        menuStack.Clear();
    }*/
}
