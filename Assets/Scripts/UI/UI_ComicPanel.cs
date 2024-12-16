using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    private bool comicOver;

    [SerializeField] private Image[] comicPanels;
    [SerializeField] private GameObject buttonToEnable;

    private Image currentImage;
    private int imageIndex;

    private void Start()
    {
        currentImage = GetComponent<Image>();
        ShowNextImage();
    }
    private void ShowNextImage()
    {
        if (comicOver)
        {
            return;
        }
        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }
    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, Action onComplete)
    {
        float time = 0;
        Color currentColor = comicPanels[imageIndex].color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            comicPanels[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
        comicPanels[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        imageIndex++;

        if (imageIndex >= comicPanels.Length)
        {
            FinishComicShow();
        }

        onComplete?.Invoke();
    }
    private void FinishComicShow()
    {
        StopAllCoroutines();
        comicOver = true;
        buttonToEnable.SetActive(true);
        currentImage.raycastTarget = false;
    }
    private void ShowNextImageOnClick()
    {
        comicPanels[imageIndex].color = Color.white;
        imageIndex++;

        if (imageIndex >= comicPanels.Length)
        {
            FinishComicShow();
        }

        if (comicOver)
        {
            return;
        }

        ShowNextImage();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowNextImageOnClick();
    }
}
