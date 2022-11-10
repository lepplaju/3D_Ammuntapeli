using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public int ratingBasedOnTime =5;
    public RectTransform timerTransform;
    [SerializeField] private GameObject starCanvas;
    [SerializeField] private GameObject timerTextMeshParent;
    [SerializeField] private TextMeshProUGUI timerTextMesh;
    [SerializeField] private Color textGlowColor;
    [SerializeField] private GameObject instructions;
    //[SerializeField] private timerShader;
    private float timeVal = 0;
    public bool timerIsActive = false;
    public bool gameIsFinished = false;
    private string[] ratingStrings = { "Excellent!", "Good", "Meh...", "You can do better" };
    [SerializeField] float timeThreshold;

    private void Awake()
    {
        textGlowColor = new Color(0, 150, 0, 128);
        timerTextMesh = GetComponent<TextMeshProUGUI>();
        timerTextMeshParent = timerTextMesh.gameObject;
    }
    public void myStart()
    {
        timerIsActive = true;
        //timerShader = timerTextMeshParent.GetComponent<Shader>();
    }
    private void Update()
    {
        if (timerIsActive)
        {
            timerTextMesh.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, textGlowColor);
            timeVal += Time.deltaTime;
            HandleTimer();
        }
    }

    private void HandleTimer()
    {
        timerTextMesh.text = timeVal.ToString("0.00");
        if (timeVal > timeThreshold)
        {
            textGlowColor.r = 50;
        }
        if (timeVal > timeThreshold*2f)
        {
            textGlowColor.r = 150;
            textGlowColor.g =0;
        }
    }

    public void getEndScreen()
    {
        starCanvas.SetActive(true);
        gameIsFinished = true;
        if (timeVal > timeThreshold)        
        {
            ratingBasedOnTime = 4;
        }
        if (timeVal > timeThreshold *2f)
        {
            ratingBasedOnTime = 3;
        }
        if (timeVal > timeThreshold * 2.5f)
        {
            ratingBasedOnTime = 2;
        }
        if (timeVal > timeThreshold * 3f)
        {
            ratingBasedOnTime = 1;
        }
        timerTextMesh.text = "<br>Level Cleared!<br>Time: " + timeVal.ToString("0.00");

        timerTextMesh.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.7f);
        //Shader timerShader = timerTextMeshParent.GetComponent<Shader>();
        timerTransform = timerTextMeshParent.GetComponent<RectTransform>();
        timerTransform.localPosition = new Vector3(10, 10, 0);
        //timerTextMesh.fontSize = 50;
        Invoke("getInstructions", 2f);
    }

    private void getInstructions()
    {
        RectTransform instructionsTransform = instructions.GetComponent<RectTransform>();
        instructionsTransform.localPosition = new Vector3(0, -100, 0);
        TextMeshProUGUI instructionsTextMesh = instructions.GetComponent<TextMeshProUGUI>();
        instructionsTextMesh.text = "Press R3 to enter next level";
    }
}
