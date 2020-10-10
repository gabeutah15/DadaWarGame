using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseButtonManager : MonoBehaviour
{
    private bool _PauseFlag = false;
    public Button _PauseButton;
    public Button _ContinueButton;
    public void PauseButton()
    {
       
        Time.timeScale = 0;
        _PauseFlag = true;
        _PauseButton.gameObject.SetActive(false);
        _ContinueButton.gameObject.SetActive(true);

    }
    public void ContinueButton()
    {
        Time.timeScale = 1;
        _PauseFlag = false;
        _PauseButton.gameObject.SetActive(true);
        _ContinueButton.gameObject.SetActive(false);
    }
}
