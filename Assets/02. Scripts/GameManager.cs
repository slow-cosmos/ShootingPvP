using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform cam; // 카메라

    [SerializeField] private Transform[] positions; // 플레이어 생성 위치
    [SerializeField] private Quaternion[] rotations; // 플레이어 생성 회전
    
    private bool isPlay;
    public bool IsPlay => isPlay;
    public bool isStart;

    private void Awake()
    {
        Screen.SetResolution(540, 960, false); // 해상도 설정

        isPlay = false;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // 포톤 서버 접속
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom() // 방 입장 완료
    {
        CreatePlayer();
        UIManager.Instance.WaitGameUI();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            UIManager.Instance.CanStartGameUI();
        }
    }

    private void CreatePlayer()
    {
        Debug.Log("플레이어 생성");

        int playerIdx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Transform playerPos = positions[playerIdx % 2];
        Quaternion playerRot = rotations[playerIdx % 2];

        PhotonNetwork.Instantiate("Player", playerPos.position, playerRot);

        if (playerIdx % 2 == 1) // 두번째 플레이어라면 카메라 회전
        {
            cam.Rotate(0, 0, 180);
        }
    }

    [PunRPC]
    private void StartGame(int restart)
    {
        Debug.Log("게임 시작");
        isPlay = true;
        isStart = true;
        if (restart == 0) UIManager.Instance.StartGameUI();
        else UIManager.Instance.RestartGameUI();
    }

    [PunRPC]
    private void EndGame(int losePlayer)
    {
        Debug.Log("게임 끝");
        isPlay = false;
        UIManager.Instance.GameResultUI(losePlayer);
    }
}
