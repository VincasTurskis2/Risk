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

    // This is necessary in order to check whether the player has clicked on something without making the class inherit from monobehaviour
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
