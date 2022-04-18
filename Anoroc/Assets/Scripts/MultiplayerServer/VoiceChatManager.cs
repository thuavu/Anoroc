using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using Photon.Pun;

public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    string appID = "da1b21c6f6824d1b9c079a6f2045987a";
    public static VoiceChatManager Instance;
    IRtcEngine rtcEngine;

    void Awake(){
        if(Instance){
            Destroy(gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start(){
        rtcEngine = IRtcEngine.GetEngine(appID);

        rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        rtcEngine.OnLeaveChannel += OnLeaveChannel;
        rtcEngine.OnError += OnError;
    }

    void OnError(int error, string msg){
        Debug.Log("Error with Agora: " + msg);
    }

    void OnLeaveChannel(RtcStats stats){
        Debug.Log("Left channel with duration " + stats.duration);
    }

    void OnJoinChannelSuccess(string channelName, uint uid, int elapsed){
        Debug.Log("Joined channel " + channelName);
    }

    public override void OnJoinedRoom(){
        rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom(){
        rtcEngine.LeaveChannel();
    }

    void OnDestroy(){
        IRtcEngine.Destroy();
    }
}
