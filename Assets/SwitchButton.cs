using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SwitchButton : MonoBehaviour
{
    public Sprite SwitchOnSprite;
    public Sprite SwitchOffSprite;
    public AudioSource capn;
    public List<AudioClip> capnclips;

    bool toggledOn = true;
    public void Toggle()
    {
        if(toggledOn)
            SwitchOff();
        else
            SwitchOn();
    }
    void SwitchOn()
    {
        toggledOn = true;
        GetComponent<Image>().sprite = SwitchOnSprite;
        capn.PlayOneShot(capnclips[0]);
    }
    void SwitchOff()
    {
        toggledOn = false;
        GetComponent<Image>().sprite = SwitchOffSprite;
        capn.PlayOneShot(capnclips[1]);
    }
}
