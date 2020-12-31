using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectTest : MonoBehaviour
{
    public Image image;

    private void Start()
    {
        Rect rect = new Rect(10,10,10,10);
        image.rectTransform.rect.Set(10, 10, 10, 10);
        image.rectTransform.offsetMax = new Vector2(20, -200);
        image.rectTransform.offsetMin = new Vector2(10, -300);
        Debug.Log(image.rectTransform.anchorMax);
        Canvas.ForceUpdateCanvases();
    }
}
