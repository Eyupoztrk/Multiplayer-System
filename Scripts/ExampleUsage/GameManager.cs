using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameManager : MultiplayerManager
{
    public GameObject EntryPanel;
    public GameObject GamePanel;
    public TextMeshProUGUI roomNameText;
    protected override void HandleOnJoinRoom()
    {
        base.HandleOnJoinRoom();
        SetUI();
        CreateObjectForEnemy();
        CreateObjectForPlayer();

    }
    protected override void HandleOnRoomCreated()
    {
        base.HandleOnRoomCreated();
        CreateObjectForPlayer();
        CreateObjectForEnemy();
        SetUI();
    }

    public void CreateObjectForPlayer()
    {
        foreach (var item in TransformedObjects)
        {
            if (item.GetComponent<ServerObject>().IsMine)
            {
                Instantiate(item);
            }
        }
    }

    public void CreateObjectForEnemy()
    {
        foreach (var item in TransformedObjects)
        {
            if (!item.GetComponent<ServerObject>().IsMine)
            {
                Instantiate(item);
            }
        }
    }
    private void SetUI()
    {
        EntryPanel.SetActive(false);
        GamePanel.SetActive(true);
        roomNameText.text = activeRoomName;
    }




}
