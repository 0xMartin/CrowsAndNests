using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameGlobal;

/// <summary>
/// Manager pro hlavni menu hry
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    public Button buttonSingle, buttonMulti, buttonSettings, buttonExit;

    void Start()
    { 
        if(buttonSingle != null)
            buttonSingle.onClick.AddListener(onClick_ButtonSingle); 
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Faild to init listener for Button Singleplayer"));      

        if(buttonMulti != null)    
            buttonMulti.onClick.AddListener(onClick_ButtonMulti);  
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Faild to init listener for Button Multiplayer"));          

        if(buttonSettings != null)    
            buttonSettings.onClick.AddListener(onClick_ButtonSettings); 
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Faild to init listener for Button Settings"));       

        if(buttonExit != null)    
            buttonExit.onClick.AddListener(onClick_ButtonExit);   
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Faild to init listener for Button Exit"));   

        Debug.Log(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Setup Done"));       
    }

    void onClick_ButtonSingle() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Button Singleplayer clicked"));   
        SceneManager.LoadScene(GameGlobal.Scene.ARENA_MENU);
    }

    void onClick_ButtonMulti() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Button Multiplayer clicked"));  
        SceneManager.LoadScene(GameGlobal.Scene.MULTIPLAYER_MENU);
    }

    void onClick_ButtonSettings() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Button Settings clicked"));  
        SceneManager.LoadScene(GameGlobal.Scene.SETTINGS_MENU);
    }

    void onClick_ButtonExit() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(MainMenuManager), "Button Exit clicked"));  
        Application.Quit();
    }

}
