using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_01_Objective : MonoBehaviour
{
    public float normalTimeScale = 1.0f;
    public float slowMotionTimeScale = 0.7f;
    public float startFixedFeltaTime;

    public bool allEnemiesKilled = false;
    public int numberOfEnemies;
    [SerializeField] GameObject enemiesParent;
    [SerializeField] SimpleMovementScript characterScript;

    private void Awake()
    {
        numberOfEnemies = enemiesParent.transform.childCount;
        startFixedFeltaTime = Time.fixedDeltaTime;
    }
    private void Start()
    {
        startNormalTime();
    }

    private void Update()
    {
        Debug.Log("Level 1 script active");
        if (allEnemiesKilled)
        {
            activateLevelEndingButton();
        }
            
    }
    public void activateLevelEndingButton()
    {
        startSlowMotion();
        Debug.Log("Activate next level available");
        characterScript.handleLevelChange();   
    }

    private void startSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedFeltaTime * slowMotionTimeScale;
    }
    private void startNormalTime()
    {
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = startFixedFeltaTime;
    }
}
