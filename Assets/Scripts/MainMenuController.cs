using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    public GameObject suggestionArrow;
    public GameObject tutorialPrompt;

    void Start()
    {
        suggestionArrow.SetActive(false);
        tutorialPrompt.SetActive(false);

        if (!GameInformation.firstPlayComplete)
        {
            suggestionArrow.SetActive(true);
            tutorialPrompt.SetActive(true);
        }

    }
}
