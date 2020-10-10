﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class gameController : MonoBehaviour
{

    // UI vars
    public Text gameInfoText;

    // Variable Initialization
    public int gameBoardX = 11;
    public int gameBoardY = 11;
    public int rounds = 5;
    public Transform boardBase;
    public Transform boardBorder;

    // Variables the game pieces need to access
    public static bool isMoveHappening = false;
    public static int[,] gameBoard;
    public static int gameTurn = 1;
    public static int gameRound = 1;
    public static int amountSelected = 0;
    public static bool isAttackHappening = false;
    public static List<int[]> attack1 = new List<int[]>();
    public static List<int[]> attack2 = new List<int[]>();
    public static List<int[]> moves1 = new List<int[]>();
    public static List<int[]> moves2 = new List<int[]>();

    //private ref vars
    public List<piece> redP = new List<piece>();
    public List<piece> blueP = new List<piece>();

    //private update vars
    private bool endRoundLogicRan = false;
    private bool gameOver = false;


    IEnumerator showGameInfo(string info, float time)
    {
        gameInfoText.text = info;
        yield return new WaitForSeconds(time);
        gameInfoText.text = "";
    }

    //helper function to find objectst that are disabled
    GameObject FindInActiveObjectByName(string name)
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>() as GameObject[];
        foreach (GameObject obj in objs)
        {
            Debug.Log(obj.name == name);
            if (obj.name == name) 
            {
                return obj;
            }
        }
        return null;
    }


    void Awake()
    {
        gameBoard = new int[gameBoardX, gameBoardY];

		// Creates a gameboard based on input size
		for (int i = -1; i <= gameBoardX; i++)
		{
			for (int j = -1; j <= gameBoardY; j++)
			{
                if (i == -1 || j == -1 || i == gameBoardX || j == gameBoardY)
				{
                    Instantiate(boardBorder, new Vector3(i, -1.5f, j), Quaternion.identity);
                }
				else
                {
                    Instantiate(boardBase, new Vector3(i, -1.5f, j), Quaternion.identity);
                }

            }
		}
    }

    void Start()
    {
        redP = new List<piece>();
        blueP = new List<piece>();
        GameObject[] allRed = GameObject.FindGameObjectsWithTag("RedCube");
        foreach (var item in allRed)
        {
            if (item.GetComponent<piece>() && item.active) 
            {
                if (item.GetComponent<piece>().isDirectionalSelector == false)
                {
                    redP.Add(item.GetComponent<piece>());
                }
            }
        }
        GameObject[] allBlue = GameObject.FindGameObjectsWithTag("BlueCube");
        foreach (var item in allBlue)
        {
            if (item.GetComponent<piece>() && item.active) 
            {
                if (item.GetComponent<piece>().isDirectionalSelector == false)
                {
                    blueP.Add(item.GetComponent<piece>());
                }
            }
        }
        StartCoroutine(showGameInfo("ROUND " + gameRound.ToString() + " OF " + rounds.ToString(), 3.0f));
    }

    bool isRoundDone()
    {
        return false;
    }
    bool isGameDone()
    {
        return false;
    }

    void getNewActions()
    {
        bool turnsDone = true;
        while (turnsDone != true){
            Debug.Log("loop broken");
            //check for if there turn is done
            turnsDone = true;
            foreach (var item in redP)
            {
                if (item.turnDone == false)
                {
                    turnsDone = false;
                }
            }
            foreach (var item in blueP)
            {
                if (item.turnDone == false)
                {
                    turnsDone = false;
                }
            }
        }
        Debug.Log("returning to loop");
    }

    IEnumerator runEndTurnLogic()
    {
        Debug.Log("END TURN LOGIC");
        yield return new WaitForSeconds(.5f);
        //reset all postions
        /*foreach (var piece in redP)
        {
            piece.reset();
        }
        foreach (var piece in blueP)
        {
            piece.reset();
        }
        /*
        //then run all the actions that they did
        
        string action = "attack";
        bool did_action = true;
        bool new_actions = false;
        while (did_action != false)
        {
            //Wait for 1 seconds
            yield return new WaitForSeconds(.2f);
            did_action = false;
            new_actions = false;
            /*foreach (var piece in redP)
            {
                if (piece.playLoop(action) == true)
                {
                    did_action = true;
                }
                else{
                    new_actions = true;
                }
            }
            foreach (var piece in blueP)
            {
                if (piece.playLoop(action) == true)
                {
                    did_action = true;
                }
                else{
                    new_actions = true;
                }
            }
            if (action == "move")
            {
                action = "attack";
            }
            else
            {
                action = "move";
            }
            //if (new_actions && did_action == true)
            //{
            //    getNewActions();
            //}
        }
        */
       // if(PhotonNetwork.IsMasterClient)
        //{
            //move oposing pieces
        /*foreach (var piece in redP)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if (piece.playLoop())
                {
                    if(piece.playLoop())
                    {
                        //piece.inLoop = false;
                    }
                    else{
                        //piece.turnDone = true;
                    }
                }
                else
                {
                    //piece.inLoop = false;
                }
            }
        }
        //}
        //else
        //{
        foreach (var piece in blueP)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                if (piece.playLoop())
                {
                    if(piece.playLoop())
                    {
                        //piece.inLoop = false;
                    }
                    else{
                        //piece.turnDone = true;
                    }
                }
                else
                {
                    //piece.inLoop = false;
                }
            }
        }*/
        //}
        //then set all objects to be able to do turns again
        foreach (var piece in redP)
        {
            if(piece.isDead){
                piece.turnDone = true;
                piece.actionThisTurn = true;
                piece.rpcCallDone = false;
                piece.isSelected = false;
            }
            else
            {
                piece.turnDone = false;
                piece.actionThisTurn = false;
                piece.rpcCallDone = false;
                piece.isSelected = false;
                if(PhotonNetwork.IsMasterClient)
                {
                    piece.inLoop = true;
                }

            }
        }
        foreach (var piece in blueP)
        {
            if(piece.isDead){
                piece.turnDone = true;
                piece.actionThisTurn = true;
                piece.rpcCallDone = false;
                piece.isSelected = false;
            }
            else
            {
                piece.turnDone = false;
                piece.actionThisTurn = false;
                piece.rpcCallDone = false;
                piece.isSelected = false;
                if(!PhotonNetwork.IsMasterClient)
                {
                    piece.inLoop = true;
                }
            }
        }
        gameTurn += 2;
        yield return new WaitForSeconds(.1f);
        //check if end of round
        if (roundDone())
        {
            //play full loop
            //resset
            foreach (var piece in redP)
            {
                piece.turnDone = true;
                piece.actionThisTurn = true;
                piece.rpcCallDone = true;
                piece.isSelected = false;
                piece.inLoop = false;
                piece.reset();
            }
            foreach (var piece in blueP)
            {
                piece.turnDone = true;
                piece.actionThisTurn = true;
                piece.rpcCallDone = true;
                piece.isSelected = false;
                piece.inLoop = false;
                piece.reset();
            }
            //play loop
            bool did_action = true;
            while (did_action != false)
            {
                //Wait for 1 seconds
                yield return new WaitForSeconds(1f);
                did_action = false;
                foreach (var piece in redP)
                {
                    //piece.turnDone = true;
                    //piece.actionThisTurn = true;
                    //piece.rpcCallDone = true;
                    //piece.isSelected = false;
                    if (piece.playLoop())
                    {
                        did_action = true;
                    }
                }
                foreach (var piece in blueP)
                {
                    //piece.turnDone = true;
                    //piece.actionThisTurn = true;
                    //piece.rpcCallDone = true;
                    //piece.isSelected = false;
                    if (piece.playLoop())
                    {
                        did_action = true;
                    }
                }
            }
            //GameObject[] gameObjects;
            /*gameObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject item in gameObjects)
            {
                item.transform.position = new Vector3(item.transform.position.x - item.transform.position.y, 0 ,item.transform.position.z - item.transform.position.y);
            }*/
            //new round stuff
            gameRound++;
            Debug.Log("ROUND OVER");
            /*//get new piece
            //FindInActiveObjectByName("RedCube (" + gameRound.ToString() +")").SetActive(true);
            //FindInActiveObjectByName("BlueCube (" + gameRound.ToString() +")").SetActive(true);
            //get new list of objects
            redP = new List<piece>();
            blueP = new List<piece>();
            GameObject[] allRed = GameObject.FindGameObjectsWithTag("RedCube");
            foreach (var item in allRed)
            {
                if (item.GetComponent<piece>() && item.active) 
                {
                    if (item.GetComponent<piece>().isDirectionalSelector == false)
                    {
                        redP.Add(item.GetComponent<piece>());
                    }
                }
            }
            GameObject[] allBlue = GameObject.FindGameObjectsWithTag("BlueCube");
            foreach (var item in allBlue)
            {
                if (item.GetComponent<piece>() && item.active) 
                {
                    if (item.GetComponent<piece>().isDirectionalSelector == false)
                    {
                        blueP.Add(item.GetComponent<piece>());
                    }
                }
            }
            //reset again
            GameObject[] allBlued = GameObject.FindGameObjectsWithTag("BlueDetector");
            foreach (var item in allBlued)
            {
                item.GetComponent<detector>().insideOfTag = "Nothing";
            }
            //reset again
            GameObject[] allRedd = GameObject.FindGameObjectsWithTag("RedDetector");
            foreach (var item in allRedd)
            {
                item.GetComponent<detector>().insideOfTag = "Nothing";
            }
            foreach (var piece in redP)
            {
                piece.reset();
                piece.turnDone = false;
                piece.actionThisTurn = false;
                piece.rpcCallDone = false;
                piece.isSelected = false;
                piece.isDead = false;
            }
            foreach (var piece in blueP)
            {
                piece.reset();
                piece.turnDone = false;
                piece.actionThisTurn = false;
                piece.rpcCallDone = false;
                piece.isSelected = false;
                piece.isDead = false;
            }
            gameObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject item in gameObjects)
            {
                item.transform.position = new Vector3(item.transform.position.x - item.transform.position.y, 0 ,item.transform.position.z - item.transform.position.y);
            }
            StartCoroutine(showGameInfo("ROUND " + gameRound.ToString() + " OF " + rounds.ToString(), 3.0f));*/
        }
        endRoundLogicRan = false;
        yield return new WaitForSeconds(.1f);
    }


    string whoWon()
    {
        int blue = 0;
        int red = 0;
        foreach (var item in redP)
        {
            if (item.isDead == false)
            {
                red++;
            }
        }
        foreach (var item in blueP)
        {
            if (item.isDead == false)
            {
                blue++;
            }
        }
        if (blue > red)
        {
            return "BLUE WON!!";
        }
        if (blue < red)
        {
            return "RED WON!!";
        }
        return "TIE!!";
    }

    bool roundDone()
    {
        bool alldead = true;
        foreach (var item in redP)
        {
            if (item.isDead == false)
            {
                alldead = false;
            }
        }
        if(alldead)
        {
            return true;
        }
        alldead = true;
        foreach (var item in blueP)
        {
            if (item.isDead == false)
            {
                alldead = false;
            }
        }
        if(alldead)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if(!gameOver)
        {
            if (gameRound >= rounds+1)
            {
                //if game round was 5 then finished
                //check who won
                string winner = whoWon();
                Debug.Log(winner);
                showGameInfo(winner, 10.0f);
                Debug.Log("GAME OVER");
                gameOver = true;
            }
            //check for if there turn is done
            bool turnsDone = true;
            foreach (var item in redP)
            {
                if (item.turnDone == false)
                {
                    if(!PhotonNetwork.IsMasterClient)
                    {
                        if (!item.rpcCallDone)
                        {
                            turnsDone = false;
                        } 
                    }
                    else{
                        turnsDone = false;
                    }
                }
            }
            foreach (var item in blueP)
            {
                if (item.turnDone == false)
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        if (!item.rpcCallDone)
                        {
                            turnsDone = false;
                        } 
                    }
                    else{
                        turnsDone = false;
                    }
                }
            }
            if (turnsDone != false && endRoundLogicRan == false) 
            {
                Debug.Log("turns done");
                endRoundLogicRan = true;
                StartCoroutine(runEndTurnLogic());
            }  
        }
            
    }
}