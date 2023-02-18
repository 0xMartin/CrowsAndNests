using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameGlobal;

/// <summary>
/// Manager pro menu areny
/// </summary>
public class ArenaMenuManager : MonoBehaviour
{
    public Button buttonBack, buttonCreate;

    void Start()
    {
        if(buttonBack != null)
            buttonBack.onClick.AddListener(onClick_ButtonBack); 
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Back"));  

        if(buttonCreate != null)
            buttonCreate.onClick.AddListener(onClick_ButtonCreate); 
        else
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Create"));
            
        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaMenuManager), "Setup Done"));            
    }

    void onClick_ButtonBack() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaMenuManager), "Button Back clicked"));   
        SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
    }

    void onClick_ButtonCreate() {
        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaMenuManager), "Button Create clicked"));   
        SceneManager.LoadScene(GameGlobal.Scene.ARENA);
    }

}
