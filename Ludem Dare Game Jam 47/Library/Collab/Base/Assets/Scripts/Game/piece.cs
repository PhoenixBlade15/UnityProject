using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class piece : MonoBehaviour
{

    // Variable Initialization
    public Material defaultColor;
    public Material selectingColor;
    public bool isDirectionalSelector = false;
    public bool isWeaponSelector = false;
    public bool isRedTeam = true;
    public GameObject weaponOne;
    public GameObject weaponTwo;
    public GameObject weaponThree;
    public GameObject weaponFour;

    // Private variables
    private bool playerIsRed;
    private Renderer objectRenderer;
    public bool isSelected = false;
    private GameObject child;
    public Transform cubeParent;
    private GameObject detector;
    private int currentSpotInLoop = 0;
    private bool offline = false;

    // Public object refrence
    public int start_pos_X;
    public int start_pos_Z;
    public int previousAction = 5;
    public bool actionThisTurn = false;
    public bool isDead = false;
    public bool turnDone = false;
    public bool rpcCallDone = false;

    // Public loop actions
    public List<string[]> loop = new List<string[]>();
    public bool inLoop = false;
    public List<string[]> loopCopy;

    // Game Object Refrences
    public gameController gc;

    // Grab components
    void Start()
    {
        loopCopy = new List<string[]>(loop);
        if(PhotonNetwork.PlayerList.Length <= 1){
                PhotonNetwork.OfflineMode = true;
                offline = true;
        }
        //TODO
        playerIsRed = isRedTeam;
        //WILL UPDATE
        objectRenderer = GetComponent<Renderer>();
        if (!isDirectionalSelector)
        {
            child = transform.GetChild(0).gameObject;
			if (!isWeaponSelector)
			{
                detector = transform.GetChild(1).gameObject;
            }
        }
        if (isWeaponSelector || isDirectionalSelector)
        {
            cubeParent = transform.parent.parent;
        }
        else{
            cubeParent = this.transform;
        }
    }


    void Update()
    {

        if (!cubeParent.GetComponent<piece>().turnDone && cubeParent.GetComponent<piece>().inLoop)
        {
            if (!playLoop()){
                cubeParent.GetComponent<piece>().inLoop = false;
            }
        }
        if ((turnDone || isDead) && rpcCallDone == false && isRedTeam != PhotonNetwork.IsMasterClient)
        {
            cubeParent.GetComponent<piece>().rpcCallDone = true;
            rpcCallDone = true;
            isSelected = false;
            objectRenderer.material = defaultColor;
            // If it is not one of the direction selector deactivate the weapons from vision
            if (!isDirectionalSelector)
            {
                weaponOne.SetActive(false);
                weaponTwo.SetActive(false);
                weaponThree.SetActive(false);
                weaponFour.SetActive(false);
                child.SetActive(false);
            }
            //weaponOne.SetActive(false);
            //weaponTwo.SetActive(false);
            //weaponThree.SetActive(false);
            //weaponFour.SetActive(false);
            //gameController.isAttackHappening = false;
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("done", RpcTarget.All);
            cubeParent.GetComponent<piece>().rpcCallDone = true;
            cubeParent.GetComponent<piece>().turnDone = true;
        }

        // Makes sure no attack is happening at the moment
        if (!gameController.isAttackHappening)
		{
            // Checks if deselect button is pressed and if selected deselect or if final 3 turns passed
            if ((Input.GetKeyDown(KeyCode.Mouse1) && isSelected) || !turnDone == false)
            {

                // Unselect the cube and change the cube material to normal, decrease the amount of selected objects in game controller
                isSelected = false;
                objectRenderer.material = defaultColor;
                gameController.amountSelected--;

                // If it is not one of the direction selector deactivate the weapons from vision
                if (!isDirectionalSelector)
                {
                    child.SetActive(false);
                }
            }
        }

        // Allows the cube to detect collision with another cube
        if (!isWeaponSelector && !isDirectionalSelector)
        {

            // Make sure the dectector isn't dectecting nothing
            if (detector.GetComponent<detector>().insideOfTag != "Nothing")
			{

                // If the dectector detects a cube of another color, "kill" both and send them far away
                // Also detects if the weapon is inside of it, and if so "kill" and send itself far away
                if (detector.GetComponent<detector>().insideOfTag != detector.GetComponent<detector>().tag)
                {
                    StartCoroutine(Die());
                }

                // If the dectector detects an ally, move both allys back one action, check UndoPreviousAction for BUG
                if (detector.GetComponent<detector>().insideOfTag == detector.GetComponent<detector>().tag)
                {
                    //UndoPreviousAction();

                }
            }
        }

    }


    // Highlight the piece when hovering over
    void OnMouseOver()
    {
        if (!gameController.isAttackHappening && playerIsRed == isRedTeam && !turnDone)
        {
            // Sets the material to selectingColor and if they click selects object until deselected via right click
            objectRenderer.material = selectingColor;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                // If this is the cube and there is only one object selected 
                if (!isDirectionalSelector && gameController.amountSelected < 1 && !isWeaponSelector)
                {
                    //if multiplaye and for the player team
                    if(offline == false)
                    {
                        if(isRedTeam != PhotonNetwork.IsMasterClient)
                        {
                            isSelected = true;
                            child.SetActive(true);
                            gameController.amountSelected++;
                        }
                    }
                    else if(offline)
                    {
                        isSelected = true;
                        child.SetActive(true);
                        gameController.amountSelected++;
                    }
                }
                else       // Else assume it is the directional selector and take an action
                {
                    TakeAction();
                }
            }
        }
    }


    // Makes the color of the cube normal when player doesn't hover over it and didn't select it
    void OnMouseExit()
    {
        if ( !isSelected)
        {
            objectRenderer.material = defaultColor;
        }
    }


	// Either moves or attacks based on the game turn
	private void TakeAction()
	{
        //if (inLoop)
        //{
        //    return;
        //}
        PhotonView photonView = PhotonView.Get(this);
        // Gets the name of the directional selector or weapon clicked and the top parent of this cube
        string objectName = gameObject.name;
        Transform parentObject = transform.root;
        bool turnTaken = false;
     
        // Makes sure the cube isn't dead and turn isn't done
        if (!cubeParent.GetComponent<piece>().isDead)
		{
            if (!cubeParent.GetComponent<piece>().turnDone)
            {
                int direction = -1;

                // Checks if the weapon or directional selector is trying to take an action
                // Do an action corresponding to the correct one
                if (objectName.Contains("Weapon"))
                {
                    direction = GetDirection(objectName);
                    //photonView.RPC("LogAction", RpcTarget.All, "attack", direction);
                    cubeParent.GetComponent<piece>().LogAction("attack", direction);
                    StartCoroutine(Attack(direction));
                    turnTaken = true;
                }

                // If not attack check if can move
                else if (objectName.Contains("DirectionalSelector"))
                {
                    direction = GetDirection(objectName);
                    turnTaken = Move(direction, parentObject);

                    if (turnTaken)
					{
                        if (direction != -1)
                        {
                            cubeParent.GetComponent<piece>().previousAction = direction;
                            //photonView.RPC("LogAction", RpcTarget.All, "move", direction);
                            cubeParent.GetComponent<piece>().LogAction("move", direction);
                        }
                    }
                }


                // If a turn is successfully taken, the actionThisTurn will go to true, and after a second successful action, the turn will be done
                if (turnTaken)
                {
                    
                    if (cubeParent.GetComponent<piece>().actionThisTurn)
                    {
                        cubeParent.GetComponent<piece>().turnDone = true;
                    }

                    if (cubeParent.GetComponent<piece>().actionThisTurn == false)
                    {
                        cubeParent.GetComponent<piece>().actionThisTurn = true;
                    }
                }
            }
        }

	}

    // Gets the direction once so doesn't have to call multiple times, cleans up code
    private int GetDirection( string objectName)
	{
        int direction = -1;
        if (objectName.Contains("One"))
        {
            direction = 0;
        }
        else if (objectName.Contains("Two"))
        {
            direction = 1;
        }
        else if (objectName.Contains("Three"))
        {
            direction = 2;
        }
        else if (objectName.Contains("Four"))
        {
            direction = 3;
        }
        return direction;
	}


    [PunRPC]
    void rpcLogAction(string action, int direction)
	{
        cubeParent.GetComponent<piece>().loop.Add(new string[] { action, direction.ToString() });
        cubeParent.GetComponent<piece>().loopCopy.Add(new string[] { action, direction.ToString() });
    }

    // Logs the action neatly, cleans up code
    [PunRPC]
    private void LogAction(string action, int direction)
	{
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("rpcLogAction", RpcTarget.All, action, direction);
    }

    // Attack plays and waits for "animation" to finish before continuing
    IEnumerator Attack(int direction)
    {
        // Makes sure the attack is happening in the right direction
        switch (direction)
		{
            case 0:
                weaponOne.SetActive(true);
                break;
            case 1:
                weaponTwo.SetActive(true);
                break;
            case 2:
                weaponThree.SetActive(true);
                break;
            case 3:
                weaponFour.SetActive(true);
                break;
            default:
                Debug.LogError("Direction not associated in IEnumerator Attack.");
                break;
		}

        // Attack waiting logic
        gameController.isAttackHappening = true;
        yield return new WaitForSeconds(.1f);
        weaponOne.SetActive(false);
        weaponTwo.SetActive(false);
        weaponThree.SetActive(false);
        weaponFour.SetActive(false);
        gameController.isAttackHappening = false;
    }

    // Dies
    IEnumerator Die()
    {
        isDead = true;
        //turnDone = true;
        yield return new WaitForSeconds(.15f);
        // deaded
        transform.root.position += new Vector3(100f, 100f, 100f);
    }

    // we make a list of actions
    // copy the list and play the actions here
    [PunRPC]
    public void reset()
    {
        weaponOne.SetActive(false);
        weaponTwo.SetActive(false);
        weaponThree.SetActive(false);
        weaponFour.SetActive(false);
        cubeParent.GetComponent<piece>().inLoop = false;
        cubeParent.GetComponent<piece>().rpcCallDone = false;
        //turnDone = false;
        cubeParent.GetComponent<piece>().isDead = false;
        cubeParent.GetComponent<piece>().loopCopy = new List<string[]>(loop);
        Transform root = transform.root;
        //move back to start
        root.position = new Vector3(start_pos_X, 0, start_pos_Z);
        if(loopCopy.Count != 0)
        {
            cubeParent.GetComponent<piece>().inLoop = true;
            //turnDone = true;
        }
        //make visble again
    }


    [PunRPC]
    public bool playLoop(){
        //if the action is the same as the action passed to play loop
        //do the next action in the stack
        //else wait
        //false if couldnt do it
        //Debug.Log(loopCopy.Count);

        if (loopCopy.Count == 0)
        {
            //turnDone = false;
            return false;
        }
        if(actionThisTurn)
        {
            turnDone = true;
            if(PhotonNetwork.IsMasterClient == isRedTeam) //is on a diffrent team
            {
                inLoop = false;
                turnDone = false;
                actionThisTurn = false;
            }
            else
            {
                inLoop = true;
            }
        }
        actionThisTurn = true;

        //PhotonView photonView = PhotonView.Get(this);
        if ("move" == loopCopy[0][0])
        {
            //photonView.RPC("move_action", RpcTarget.All, loopCopy[0][1]);
            //if(PhotonNetwork.PlayerList.Length <= 1){
            move_action(loopCopy[0][1]);
            //}
            Debug.Log("move at " + (string)loopCopy[0][1]);
            loopCopy.RemoveAt(0);
            return true;
        }
        else if ("attack" == loopCopy[0][0])
        {
            //photonView.RPC("attack_action", RpcTarget.All, loopCopy[0][1]);
            //if(PhotonNetwork.PlayerList.Length <= 1){
            attack_action(loopCopy[0][1]);
            //}
            Debug.Log("attack at " + (string)loopCopy[0][1]);
            loopCopy.RemoveAt(0);
            return true;
        }
        else
        {
            return true;
        }
    }


    // Assumes we are using the gameobject holding all the parts in
    [PunRPC]
    public void attack_action(string ds)
    {
        int direction = int.Parse(ds);
        StartCoroutine(Attack(direction));
    }

    // Pass in the root of the prefab to piece
    [PunRPC]
    public void move_action(string direction)
    {
        Transform piece = transform.root;
        // Gets the constraints of the board size so players can't move out of it
        int minX = 0;
        int maxX = gameController.gameBoard.GetLength(0)-1;
        int minZ = 0;
        int maxZ = gameController.gameBoard.GetLength(1)-1;
        Vector3 currentVector3 = piece.transform.position;

        if (direction == "0")
        {
            // Move Z + 1
            if (piece.transform.position.z + 1 <= maxZ)
            {
                piece.transform.position += Vector3.forward;
                //cubeParent.GetComponent<piece>().previousAction = 0;
            }
        }
        else if (direction == "1")
        {
            // Move X + 1
            if (piece.transform.position.x + 1 <= maxX)
            {
                piece.transform.position += Vector3.right;
                //cubeParent.GetComponent<piece>().previousAction = 1;
            }
        }
        else if (direction == "2")
        {
            // Move Z - 1
            if (piece.transform.position.z - 1 >= minZ)
            {
                piece.transform.position += Vector3.back;
                //cubeParent.GetComponent<piece>().previousAction = 2;
            }

        }
        else if (direction == "3")
        {
            // Move X - 1
            if (piece.transform.position.x - 1 >= minX)
            {
                piece.transform.position += Vector3.left;
                //cubeParent.GetComponent<piece>().previousAction = 3;
            }

        }
    }

    [PunRPC]
    public void done()
    {
        cubeParent.GetComponent<piece>().turnDone = true;
        weaponOne.SetActive(false);
        weaponTwo.SetActive(false);
        weaponThree.SetActive(false);
        weaponFour.SetActive(false);
    }

    // Moves the cube exactly 1 unit in the direction corresponding to the players requested movement
    private bool Move(int direction, Transform parentObject)
	{
        // Gets the constraints of the board size so players can't move out of it
        int minX = 0;
        int maxX = gameController.gameBoard.GetLength(0)-1;
        int minZ = 0;
        int maxZ = gameController.gameBoard.GetLength(1)-1;

        // Makes sure the player gets to move without wasting a turn trying go out of bounds
        bool turnTaken = false;

        if (direction == 0)
        {
            // Move Z + 1
            if (parentObject.transform.position.z + 1 <= maxZ)
            {
                parentObject.transform.position += Vector3.forward;
                turnTaken = true;
            }
        }
        else if (direction == 1)
        {
            // Move X + 1
            if (parentObject.transform.position.x + 1 <= maxX)
            {
                parentObject.transform.position += Vector3.right;
                turnTaken = true;
            }
        }
        else if (direction == 2)
        {
            // Move Z - 1
            if (parentObject.transform.position.z - 1 >= minZ)
            {
                parentObject.transform.position += Vector3.back;
                turnTaken = true;
            }

        }
        else if (direction == 3)
        {
            // Move X - 1
            if (parentObject.transform.position.x - 1 >= minX)
            {
                parentObject.transform.position += Vector3.left;
                turnTaken = true;
            }

        }

        if (!turnTaken)
		{
            Debug.Log("Move is out of bounds");
        }

        return turnTaken;
	}


    /*
    // This function is called when two ally cubes are on the same tile
    // BUG: if the two ally cubes took the same move they get stuck on each other.
    // EX: Two ally cubes next to each other left and right, one moves once right, other moves twice
    //              They both get sent to their previous move, causing both cubes to be stacked
    //              This shouldn't matter since only one cube can be selected at a time, so when one moves the other will stay.
    private void UndoPreviousAction()
	{
		switch (previousAction)
		{
            case 0:
                transform.root.position -= Vector3.forward;
                break;
            case 1:
                transform.root.position -= Vector3.right;
                break;
            case 2:
                transform.root.position -= Vector3.back;
                break;
            case 3:
                transform.root.position -= Vector3.left;
                break;
            default:
                print("UndoPreviousAction Error.");
                break;
        }
        previousAction = 5;
    }
    */
}