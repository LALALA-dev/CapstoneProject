using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Image teamLogo;
    public SceneLoader sl;

    private void Awake()
    {
        teamLogo.color = new Color(1, 1, 1, 1);
    }

    void Start()
    {
        StartCoroutine(FadeIn(teamLogo));
    }

    void Update()
    {
        if (Input.anyKey)
        {
            sl.LoadMenuScene();
        }
    }

    IEnumerator FadeIn(Image img)
    {
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }

        // wait a sec
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        StartCoroutine(FadeOut(img));
    }

    IEnumerator FadeOut(Image img)
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }

        sl.LoadMenuScene();
    }
}
