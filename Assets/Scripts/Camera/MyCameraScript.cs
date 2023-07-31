using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraScript : MonoBehaviour
{
    private FollowPlayer _followPlayer;

    private void Awake()
    {
        _followPlayer = GetComponent<FollowPlayer>();
    }

    public void SetPlayer(Transform player)
    {
        _followPlayer.player = player;
    }
}

