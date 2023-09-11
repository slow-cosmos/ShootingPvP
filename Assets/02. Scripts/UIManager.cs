using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField] private GameManager gameManager;

    // 게임 시작
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject waitText;

    [SerializeField] private GameObject coolText;

    // 게임 결과
    [SerializeField] private TMP_Text gameResult;
    [SerializeField] private Button restartButton;

    // 체력 UI
    [SerializeField] private GameObject myHp; // 내 체력
    [SerializeField] private GameObject youHp; // 상대 체력

    public void WaitGameUI()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            waitText.SetActive(true);
        }
    }

    public void StartGameUI()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.gameObject.SetActive(false);
        }
        else
        {
            waitText.SetActive(false);
        }

        myHp.SetActive(true);
        youHp.SetActive(true);
        InitHpUI();
    }

    public void RestartGameUI()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            restartButton.gameObject.SetActive(false);
        }
        gameResult.gameObject.SetActive(false);

        myHp.SetActive(true);
        youHp.SetActive(true);
        InitHpUI();
    }

    public void CanStartGameUI()
    {
        startButton.interactable = true;
    }

    public void GameResultUI(int losePlayer)
    {
        myHp.SetActive(false);
        youHp.SetActive(false);

        gameResult.gameObject.SetActive(true);

        if (losePlayer == PhotonNetwork.LocalPlayer.ActorNumber) // 게임 결과
        {
            gameResult.text = "YOU LOSE";
        }
        else
        {
            gameResult.text = "YOU WIN";
        }

        if(PhotonNetwork.LocalPlayer.IsMasterClient) // Restart 버튼
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    public void InitHpUI() // 체력 UI 초기화
    {
        for (int i = 0; i < myHp.transform.childCount; i++)
        {
            myHp.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < youHp.transform.childCount; i++)
        {
            youHp.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UpdateHpUI(int attackedPlayer, int curHp) // 체력 UI 업데이트
    {
        if (attackedPlayer == PhotonNetwork.LocalPlayer.ActorNumber) // 내가 맞았다면
        {
            for (int i = curHp; i < myHp.transform.childCount; i++)
            {
                myHp.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else // 상대가 맞았다면
        {
            for (int i = curHp; i < youHp.transform.childCount; i++)
            {
                youHp.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void StartButton() // 시작 버튼 클릭
    {
        gameManager.photonView.RPC("StartGame", RpcTarget.All, 0);
    }

    public void RestartButton() // 재시작 버튼 클릭
    {
        gameManager.photonView.RPC("StartGame", RpcTarget.All, 1);
    }

    public IEnumerator CoolShoot() // 쿨타임 표시
    {
        coolText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        coolText.SetActive(false);
    }
}
