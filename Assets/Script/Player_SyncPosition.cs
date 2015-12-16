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

    //Leap:２ベクトルの間を補完する
    [SerializeField]
    float leapRate = 15;

    //前フレームの最終位置
    private Vector3 lastPos;

    //threshold:しきい値、境目となる値のこと
    //0.5unitを越えなければ移動していないこととする
    private float threshold = 0.5f;

    void Update()
    {
        LeapPosition();
    }

    void FixedUpdate()
    {
        TransmitPosition();
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
        if(isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }
}
