using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    public UnityEngine.UI.Button BackButton;
    [HideInInspector]
    public MenuManager menuManager;

    
    public void Enter()
    {
        if (BackButton != null)
        {
            BackButton.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
        onScreenEnter();
    }

    public void Leave()
    {
        if (BackButton != null)
        {
            BackButton.gameObject.SetActive(false);
        }
        onScreenLeave();
        gameObject.SetActive(false);
    }

    public virtual void onScreenEnter()
    {

    }

    public virtual void onScreenLeave()
    {

    }
}