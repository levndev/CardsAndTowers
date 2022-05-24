using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScreen : MenuScreen
{
    public Button TestButton;
    // Start is called before the first frame update
    void Start()
    {
        TestButton.onClick.AddListener(onTestButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onTestButtonClick()
    {
        Debug.Log("Nice Test!");
    }
}
