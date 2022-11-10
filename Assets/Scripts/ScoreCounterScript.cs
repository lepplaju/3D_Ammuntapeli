using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounterScript : MonoBehaviour
{
    [SerializeField] Level_01_Objective levelScript;
    [SerializeField] AudioSource bassAudioSource;
    [SerializeField] AudioClip Kick01;
    public int activeMultiKillCounter;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] public int killCount;
    [SerializeField] private TimerScript timerScript;
    // Start is called before the first frame update
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (killCount >= 1)
        {
            textMesh.text = "Kills: " + killCount;
        }
        */
    }

    private void LateUpdate()
    {
        if (killCount >= levelScript.numberOfEnemies)
        {
            levelScript.allEnemiesKilled = true;
            handleTimerStop();
        }
    }
    public void handleTimerStop()
    {
        timerScript.timerIsActive = false;
        timerScript.getEndScreen();
    }
    
    public void addANewKill()
    {
        playTheAudio();
        killCount += 1;
        bassAudioSource.pitch += .1f;
    }

    public void playTheAudio()
    {
        bassAudioSource.PlayOneShot(Kick01, 0.1f);
    }
}
