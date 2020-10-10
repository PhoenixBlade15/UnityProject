using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGameMaster : MonoBehaviourPunCallbacks
{
    public Text blueTeam;
    public Text redTeam;
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.PlayerList.Length <= 1){
                PhotonNetwork.OfflineMode = true;
        }
        else{
            blueTeam.text = "BLUE TEAM: " + PhotonNetwork.PlayerList[0].NickName;
            redTeam.text = "RED TEAM: " + PhotonNetwork.PlayerList[1].NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.PlayerList.Length <= 1){
            PhotonNetwork.LeaveRoom();
            Debug.Log("Other player left");
            Application.LoadLevel("MainMenu");
        }
    }
}
