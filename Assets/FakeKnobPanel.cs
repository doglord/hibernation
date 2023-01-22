using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FakeKnobPanel : MonoBehaviour
{
    public UnityEvent LeftButtonPressed;
    public UnityEvent RightButtonPressed;
    public void OnPressLeft(){LeftButtonPressed?.Invoke();}
    public void OnPressRight(){RightButtonPressed?.Invoke();}

    public Button leftButton;
    public Button rightButton;
    public void DisableInteract()
    {
        leftButton.interactable = false;
        rightButton.interactable = false;
    }
    public void EnableInteract()
    {
        leftButton.interactable = true;
        rightButton.interactable = true;
    }
}
