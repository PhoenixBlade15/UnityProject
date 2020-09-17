using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditSigns : MonoBehaviour
{
    public GameObject chatBox;
    public Text conversationText;
    public string toShow = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            chatBox.SetActive(true);
            conversationText.text = toShow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            conversationText.text = "";
            chatBox.SetActive(false);
        }
    }
}
