using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.ComponentModel;
using Unity.VisualScripting;

public class Player : MonoBehaviourPunCallbacks
{
    public MeshRenderer meshRenderer;
    private GameManager gameManager;

    [SerializeField] private int hp = 3;
    private float speed = 5.0f;
    private Vector3 leftRange;
    private Vector3 rightRange;

    private bool canShoot;
    private float shootCool;
    private WaitForSeconds cool;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        leftRange = new Vector3(-2.6f, transform.position.y, transform.position.z);
        rightRange = new Vector3(2.6f, transform.position.y, transform.position.z);

        ChangeColor();

        canShoot = true;
        shootCool = 1;
        cool = new WaitForSeconds(shootCool);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if(gameManager.isStart) // 시작했다면 체력 초기화
        {
            photonView.RPC("InitHp", RpcTarget.All);
            gameManager.isStart = false;
        }

        if(gameManager.IsPlay) // 게임 중이라면
        {
            if (Input.GetKey("left"))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                if (transform.position.x < -2.6f)
                {
                    transform.position = leftRange;
                }
                else if (transform.position.x > 2.6f)
                {
                    transform.position = rightRange;
                }

            }
            if (Input.GetKey("right"))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                if (transform.position.x < -2.6f)
                {
                    transform.position = leftRange;
                }
                else if (transform.position.x > 2.6f)
                {
                    transform.position = rightRange;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(canShoot)
                {
                    PhotonNetwork.Instantiate("bullet", transform.position, Quaternion.identity);
                    canShoot = false;
                    StartCoroutine(WaitShoot());
                }
                else
                {
                    UIManager.Instance.StartCoroutine(UIManager.Instance.CoolShoot());
                }
            }
        }
    }

    private IEnumerator WaitShoot() // 발사 쿨타임
    {
        yield return cool;
        canShoot = true;
    }

    private void ChangeColor() // 플레이어 색 변경
    {
        int ownerIdx = photonView.OwnerActorNr - 1;
        meshRenderer.material = meshRenderer.materials[ownerIdx % 2];
    }

    [PunRPC]
    private void InitHp()
    {
        hp = 3;
    }

    [PunRPC]
    public void OnDamaged()
    {
        Debug.Log($"OnDamaged : {photonView.OwnerActorNr}가 맞음");
        hp -= 1;
        if (hp <= 0)
        {
            hp = 0;
            // 방장이 게임 종료를 모든 클라이언트에게 알림
            if(PhotonNetwork.LocalPlayer.IsMasterClient && gameManager.IsPlay)
            {
                gameManager.photonView.RPC("EndGame", RpcTarget.All, photonView.OwnerActorNr);
            }
        }
        else
        {
            UIManager.Instance.UpdateHpUI(photonView.OwnerActorNr, hp);
        }
    }
}
