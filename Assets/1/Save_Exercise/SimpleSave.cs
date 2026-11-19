using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleSave : MonoBehaviour
{
    //Usually use it for player preferences not for save data (E.g. resolution)
    //For something small and simple, like what level the player is currently at

    void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetFloat("Whatever", Time.time);
        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Debug.Log(PlayerPrefs.GetFloat("Whatever"));
        }
    }
}
