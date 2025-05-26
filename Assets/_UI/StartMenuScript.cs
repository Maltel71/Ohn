using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    private UIDocument document;
    private VisualElement MainMenu;
    private VisualElement OptionsMenu;


    void Awake()
    {
        document = GetComponent<UIDocument>();

        //MainMenu
        MainMenu = document.rootVisualElement.Q("MainMenu");

        Button playButton = MainMenu.Q("Play") as Button;
        playButton.RegisterCallback<ClickEvent>(OnPlayClicked);

        Button optionsButton = MainMenu.Q("Options") as Button;
        optionsButton.RegisterCallback<ClickEvent>(OnOptionsClicked);

        Button quitButton = MainMenu.Q("Quit") as Button;
        quitButton.RegisterCallback<ClickEvent>((evt) => Application.Quit()); // Closes the application;


        //OptionsMenu

        OptionsMenu = document.rootVisualElement.Q("OptionsMenu");

    }

    void Update()
    {
        
    }



    private void OnPlayClicked(ClickEvent evt)
    {
        //SceneManager.LoadScene("Game_Scene");
        SceneManager.LoadScene(1);
        Debug.Log("You clicked the play button");
    }

    private void OnOptionsClicked(ClickEvent evt)
    {
        MainMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.Flex;

    }






}
