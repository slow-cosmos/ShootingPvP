using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    private GameManager gameManager;

    private float speed = 5.0f;
    private Vector3 networkPosition;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    private void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        
        int ownerIdx = photonView.OwnerActorNr - 1;
        if(ownerIdx % 2 == 0)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if(!gameManager.IsPlay) // 게임이 끝나면 총알 Destroy
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider col) // 자기 총알이면 처리하기
    {
        if(!photonView.IsMine)
        {
            return;
        }
        if(col.tag == "Wall")
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        else if(col.tag == "Player")
        {
            PhotonView player = col.GetComponent<Player>().photonView;
            if(player.OwnerActorNr != photonView.OwnerActorNr)
            {
                player.RPC("OnDamaged", RpcTarget.All);
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}