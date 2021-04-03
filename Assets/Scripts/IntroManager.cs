using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Image teamLogo;
    public Image gameLogo;
    public SpriteRenderer introImage;
    public SceneLoader sl;

    private void Awake()
    {
        introImage.enabled = false;
        gameLogo.gameObject.SetActive(false);
        gameLogo.color = new Color(1, 1, 1, 1);
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

        teamLogo.gameObject.SetActive(false);
        introImage.enabled = true;
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        for (float i = -4; i <= 4; i += .04f)
        {
            gameObject.transform.position = new Vector3(i, transform.position.y, -5.0f);
            yield return null;
        }

        StartCoroutine(MoveCamera1());
    }

    IEnumerator MoveCamera1()
    {
        for (float i = 2; i >= -2; i -= .03f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, i, -5.0f);
            yield return null;
        }

        StartCoroutine(MoveCamera2());
    }

    IEnumerator MoveCamera2()
    {
        for (float i = 4; i >= -4; i -= .04f)
        {
            gameObject.transform.position = new Vector3(i, transform.position.y, -5.0f);
            yield return null;
        }
        StartCoroutine(MoveCamera3());
    }

    IEnumerator MoveCamera3()
    {
        for (float i = 0; i <= 200; i++)
        {
            gameObject.transform.position = new Vector3(transform.position.x + .02f, transform.position.y + .01f, transform.position.z - .025f);
            yield return null;
        }

        gameLogo.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            gameLogo.color = new Color(1, 1, 1, i);
            yield return null;
        }
        Invoke("LoadMainMenu", 1.0f);
    }

    public void LoadMainMenu()
    {
        sl.LoadMenuScene();
    }
}
