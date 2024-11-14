using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image circleImage;
    public float totalTime = 5f;
    [SerializeField] private float timeRemaining;

    void Start()
    {
        timeRemaining = totalTime;
        circleImage.fillAmount = 1f;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            circleImage.fillAmount = timeRemaining / totalTime;
        }
        else
        {
            circleImage.fillAmount = 0f;
        }
    }
}
