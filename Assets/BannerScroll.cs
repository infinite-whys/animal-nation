using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerScroll : MonoBehaviour
{
    public float scrollSpeed = 50f;  // Adjust speed as needed
    public RectTransform textRect;
    public float startPosition = 2190;
    public float endPosition = -2180;

    void Start()
    {
        //textRect = GetComponent<RectTransform>();
        //startPosition = textRect.anchoredPosition.x;
    }

    void Update()
    {
        // Move text leftward over time
        textRect.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

        // Check if text has scrolled past the panel and reset its position
        if (textRect.anchoredPosition.x <= endPosition)
        {
            textRect.anchoredPosition = new Vector2(startPosition, textRect.anchoredPosition.y);
        }
    }
}
