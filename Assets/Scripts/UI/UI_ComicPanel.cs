using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    private bool comicOver;

    [SerializeField] private Image[] comicPanels;
    [SerializeField] private GameObject buttonToEnable;

    private Image currentImage;
    private int imageIndex;
    private CancellationTokenSource cts;

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
        ChangeImageAlpha(1, 1.5f, ShowNextImage).Forget();
    }
    private async UniTaskVoid ChangeImageAlpha(float targetAlpha, float duration, Action onComplete)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        var ct = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, this.GetCancellationTokenOnDestroy()).Token;

        int indexToChange = imageIndex;
        float time = 0;
        Color currentColor = comicPanels[indexToChange].color;
        float startAlpha = currentColor.a;

        while (time < duration && !comicOver)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            comicPanels[indexToChange].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        if (comicOver) return;

        comicPanels[indexToChange].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        imageIndex++;

        if (imageIndex >= comicPanels.Length)
        {
            FinishComicShow();
        }

        onComplete?.Invoke();
    }
    private void FinishComicShow()
    {
        cts?.Cancel();
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
