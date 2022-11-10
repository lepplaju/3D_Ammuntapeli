using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showRatingScript : MonoBehaviour
{
    public int rating;
    [SerializeField] GameObject brightParent;
    [SerializeField] GameObject grayParent;
    [SerializeField] GameObject starObj;
    [SerializeField] GameObject grayStarObj;
    [SerializeField] TimerScript _timerScript;
    private bool starsOnScreen = false;
    private RectTransform starOffset;

    private void Awake()
    {
        gameObject.SetActive(false);
        starOffset = starObj.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (_timerScript.gameIsFinished)
        {
            //parent.SetActive(true);
            getRightAmountOfStars();
        }
    }

    private void getRightAmountOfStars()
    {
        if (!starsOnScreen)
        {
            rating = _timerScript.ratingBasedOnTime;
            Vector3 offset = _timerScript.timerTransform.localPosition;
            offset.y += 60f;
            offset.x -= 60f;
            //Vector3 offset = new Vector3(20, 0, 0);
            for (int i = 0; i < rating; i++)
            {
                GameObject StarCopy = Instantiate(starObj, brightParent.transform);
                StarCopy.transform.localPosition = offset;
                offset.x += 32f;
            }
            if (rating < 5)
            {
                offset.z += -10;
                for (int j = 0; j < 5-rating; j++)
                {
                    GameObject StarCopy = Instantiate(grayStarObj, grayParent.transform);
                    StarCopy.transform.localPosition = offset;
                    offset.x += 32f;
                }
            }
            starsOnScreen = true;
        }
    }
}
