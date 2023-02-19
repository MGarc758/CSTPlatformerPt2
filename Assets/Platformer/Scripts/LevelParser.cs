using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelParser : MonoBehaviour
{
    public string filename;
    public GameObject rockPrefab;
    public GameObject brickPrefab;
    public GameObject questionBoxPrefab;
    public GameObject stonePrefab;
    public Transform environmentRoot;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;

    private int timeLeft;
    private int timerSinceReset;
    private int coinsCollected;
    private int score;

    // --------------------------------------------------------------------------
    void Start()
    {
        LoadLevel();
        timeLeft = 400;
        timerSinceReset = 0;
        coinsCollected = 0;
        score = 0;
    }

    // --------------------------------------------------------------------------
    void Update()
    {
        scoreText.text = "Score \n" + score.ToString();
        coinsText.text = "Coins \n" + coinsCollected.ToString();
        timerText.text = "Time \n" + Math.Floor(timeLeft - (Time.realtimeSinceStartup - timerSinceReset)).ToString();

        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.name == "QuestionBlock(Clone)")
                {
                    Object.Destroy(raycastHit.collider.gameObject);
                    coinsCollected++;
                    score += 100;
                } else if (raycastHit.collider.name == "BrickBlock(Clone)")
                {
                    Object.Destroy(raycastHit.collider.gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
    }

    // --------------------------------------------------------------------------
    private void LoadLevel()
    {
        string fileToParse = $"{Application.dataPath}/Resources/{filename}.txt";
        Debug.Log($"Loading level file: {fileToParse}");

        Stack<string> levelRows = new Stack<string>();

        // Get each line of text representing blocks in our level
        using (StreamReader sr = new StreamReader(fileToParse))
        {
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                levelRows.Push(line);
            }

            sr.Close();
        }

        // Go through the rows from bottom to top
        int row = 0;
        while (levelRows.Count > 0)
        {
            string currentLine = levelRows.Pop();

            char[] letters = currentLine.ToCharArray();
            for (int column = 0; column < letters.Length; column++)
            { 
                var letter = letters[column];
                if (letter == 'x')
                {
                    var newDirt = Instantiate(rockPrefab, new Vector3(column, row, 0f), Quaternion.identity);
                    newDirt.transform.parent = gameObject.transform.GetChild(0).transform;
                } else if (letter == 'b')
                {   
                    var newBrick = Instantiate(brickPrefab, new Vector3(column, row, 0f), Quaternion.identity);
                    newBrick.transform.parent = gameObject.transform.GetChild(0).transform;
                } else if (letter == 's')
                {
                    var newStone = Instantiate(stonePrefab, new Vector3(column, row, 0f), Quaternion.identity);
                    newStone.transform.parent = gameObject.transform.GetChild(0).transform;
                } else if (letter == '?')
                {
                    var newQuestion = Instantiate(questionBoxPrefab, new Vector3(column, row, 0f), Quaternion.identity);
                    newQuestion.transform.parent = gameObject.transform.GetChild(0).transform;
                }
                // Todo - Instantiate a new GameObject that matches the type specified by letter
                // Todo - Position the new GameObject at the appropriate location by using row and column
                // Todo - Parent the new GameObject under levelRoot
            }
            row++;
        }
    }

    // --------------------------------------------------------------------------
    private void ReloadLevel()
    {
        timeLeft = 400;
        timerSinceReset = Convert.ToInt32(Math.Floor(Time.realtimeSinceStartup));
        
        score = 0;
        coinsCollected = 0;
        
        foreach (Transform child in environmentRoot)
        {
           Destroy(child.gameObject);
        }
        LoadLevel();
    }
}
