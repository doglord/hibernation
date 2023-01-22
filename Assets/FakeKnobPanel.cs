using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FakeKnobPanel : MonoBehaviour
{
    public UnityEvent LeftButtonPressed;
    public UnityEvent RightButtonPressed;
    public void OnPressLeft(){LeftButtonPressed?.Invoke();}
    public void OnPressRight(){RightButtonPressed?.Invoke();}
}
