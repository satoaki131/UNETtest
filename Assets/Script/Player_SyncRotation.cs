using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Player_SyncRotation : NetworkBehaviour {

    [SyncVar] //SyncVar:ホストからクライアントに送られる
    //プレイヤーの角度
    private Quaternion syncPlayerRotation;

    //FirstPersonCharacterのカメラの角度
    [SyncVar]
    private Quaternion syncCamRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform camTransform;
    [SerializeField]
    private float leapRate = 15;

	void FixedUpdate ()
    {
        //クライアントと側のPlayerの角度を所得
        TransmitRotations();
        //現在角度と所得した角度を補完する
        LeapRotations();
	}
    
    //角度を補完するメソッド
    private void LeapRotations()
    {
        //自プレイヤー以外のPlayer時
        if(!isLocalPlayer)
        {
            //プレイヤーの角度とカメラの角度を補完
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation,
                syncPlayerRotation, Time.deltaTime * leapRate);
            camTransform.rotation = Quaternion.Lerp(camTransform.rotation,
                syncCamRotation, Time.deltaTime * leapRate);
        }
    }

    //クライアントからホストに送られる
    [Command]
    void CmdProvideRotationsToServer(Quaternion playerRot, Quaternion camRot)
    {
        syncPlayerRotation = playerRot;
        syncCamRotation = camRot;
    }

    //クライアント側だけが実行できるメソッド
    [Client]
    private void TransmitRotations()
    {
        if(isLocalPlayer)
        {
            CmdProvideRotationsToServer(playerTransform.rotation, camTransform.rotation);
        }
    }
}
