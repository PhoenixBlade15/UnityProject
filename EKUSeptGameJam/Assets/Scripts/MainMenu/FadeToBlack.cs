using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public GameObject fadeSquare;

    public int buildIndex = 0;
    public int fadeTime = 5;
    public int levelLoadDelay = 2;

    private void Start()
    {
        Color fadeColor = fadeSquare.GetComponent<Image>().color;
        fadeSquare.GetComponent<Image>().color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
    }

    public IEnumerator FadeBlackOut(bool fade = true, int fadeSpeed = 5)
    {
        Debug.Log("Called");
        Color objectColor = fadeSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fade)
        {
            while (fadeSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                fadeSquare.GetComponent<Image>().color = objectColor;
                Debug.Log(fadeSquare.GetComponent<Image>().color.a);
                yield return null;
            }
        } else
        {
            while (fadeSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                fadeSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FadeAndLoad();
    }

    private void FadeAndLoad()
    {
        StartCoroutine(FadeBlackOut(true, fadeTime));
        StartCoroutine(LoadLevelAfterDelay(levelLoadDelay));
    }

    IEnumerator LoadLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(buildIndex);
    }
}
