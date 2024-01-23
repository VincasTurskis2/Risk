using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayerWrapper : MonoBehaviour
{

    public HumanPlayer player {get; set;} = null;

    public void Setup(HumanPlayer Player)
    {
        player = Player;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            player.Update();
        }
        else
        {
            Debug.Log("Human player is null!");
        }
    }
}
