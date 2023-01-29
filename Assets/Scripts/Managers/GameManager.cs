using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public TextMeshProUGUI m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;           


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    private void Start()
    {
        // set the timer for the coroutines
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();


        // the game loop untill the some player win 5/numberOfRounds rounds
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        // create array of transforms for the camera the same size as number of tanks
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            // get the transform of each indivual tank
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // feed the tanls transform to the array in the camera script
        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        // start the round starting instruction and go to the next sequence untill finished
        yield return StartCoroutine(RoundStarting());

        // wait for the round to end
        yield return StartCoroutine(RoundPlaying());

        // starts once the round finished and wait untill and round instruction runs
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            // start from the begining
            SceneManager.LoadScene(1);
        }
        else
        {
            // start another round
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // rest all tanks and disable their contorol
        ResetAllTanks();
        DisableTankControl();

        // snap the camera zoom and position
        m_CameraControl.SetStartPositionAndSize();

        // increment round number and display it
        m_RoundNumber++;
        m_MessageText.text = $"Round: {m_RoundNumber}";

        // wait for some time for returning to game loop
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // enable controls when the round starts
        EnableTankControl();

        // clear the message from the screen
        m_MessageText.text = string.Empty;

        // continue the game untill only one tanks lefts
        while (!OneTankLeft())
        {
            // return to the next frame
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // disable all controks
        DisableTankControl();

        // set the last round winner to null
        m_RoundWinner = null;

        // check for a round winner
        m_RoundWinner = GetRoundWinner();

        // increment the score of the winner
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        // check if the round winner wins the match
        m_GameWinner = GetGameWinner();

        // display the message based on the score or winning the match condition
        m_MessageText.text = EndMessage();

        // display the message for some time
        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        // set the coun to zero
        int numTanksLeft = 0;

        // add to the count according to number of tanks in game manager array
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        // check for the active tanks that's left standing
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        // check for the tanks in the array of number of win equal to the total numbe to win the match
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        // draw as default value
        string message = "DRAW!";

        // write the message the player win
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        // display number of wins of both players
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // display that message if the game is won
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}