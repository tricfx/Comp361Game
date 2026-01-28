using UnityEngine;

[System.Serializable]
public class GameData
{
   public int sceneIndex;
   public int gemsCollected;
   public string[] abilities; 
   public bool left_during_combat;

    //these will be the default values upon a new game start, if the player presses continue, we will load saved data and if there is none, we will use these defaults
    public GameData()
    {
        //Index 0 means menu (an index of scene is where we should make the player start)
        sceneIndex = 0;
        gemsCollected = 0;
        abilities = new string[] { };
        left_during_combat = false;
    }
}
