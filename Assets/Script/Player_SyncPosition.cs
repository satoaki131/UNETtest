using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Player_SyncPosition : NetworkBehaviour {

    [SyncVar] //ホストから全クライアントに送られる
    private Vector3 syncPos;

    //Playerの現在位置
    [SerializeField]
    Transform myTransform;

    //Leap:２ベクトル間を補完する
    [SerializeField]
    float leapRate = 15;

    void FixedUpdate()
    {
        TransmitPosition();
        LeapPosition();
    }

    //ポジション補完用メソッド
    private void LeapPosition()
    {
        //補完対象は相手プレイヤーのみ
        if(!isLocalPlayer)
        {
            //Leap(from, to, 割合)from～toのベクトル間を補完する
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * leapRate);
        }
    }

    [Command] //クライアントからホストへ、Position情報を送る
    void CmdProvidePositionToServer(Vector3 pos)
    {
        //サーバー側が受け取る値
        syncPos = pos;
    }

    [ClientCallback] //クライアントのみ実行される
    //位置情報を送るメソッド
    private void TransmitPosition()
    {
        if(isLocalPlayer)
        {
            CmdProvidePositionToServer(myTransform.position);
        }
    }
}
