using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Inst;
    
    public Scene mainGame;
    void Awake() 
    { 
        Inst = this; 

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
    }

    public void RestartScene()
    {
        SceneManager.UnloadScene("SampleScene");
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
    }

}
