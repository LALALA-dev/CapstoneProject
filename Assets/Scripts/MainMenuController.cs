using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject tutorialPrompt;
    public ArrowController suggestionArrow;
    public AudioSource button;
    public bool forward = true; 

    void Start()
    {
        suggestionArrow.DisableArrow();
        tutorialPrompt.SetActive(false);

        if (!GameInformation.tutorialNeeded)
        {
            suggestionArrow.gameObject.SetActive(true);
            tutorialPrompt.SetActive(true);
            StartCoroutine(MoveForward());
        }

    }

    IEnumerator MoveForward()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            suggestionArrow.gameObject.transform.position = new Vector3(suggestionArrow.gameObject.transform.position.x + 10f, suggestionArrow.gameObject.transform.position.y);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveBackward());
    }

    IEnumerator MoveBackward()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            suggestionArrow.gameObject.transform.position = new Vector3(suggestionArrow.gameObject.transform.position.x - 10f, suggestionArrow.gameObject.transform.position.y);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveForward());
    }

    public void OnButtonClick()
    {
        button.Play();
    }
}
