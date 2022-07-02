using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL_Scenes : MonoBehaviour {

	public void FacebookPage()
    {
        Application.OpenURL("https://www.facebook.com/zoxtergames/");
    }
    public void SketchfabPage()
    {
        Application.OpenURL("https://sketchfab.com/ZoxterGames");
    }
    public void YouTubeChannel()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCKL8wh1TYmEPvzeNEG-NrGg");
    }
    public void GameJoltPage()
    {
        Application.OpenURL("http://gamejolt.com/games/plazmaburst/261475");
    }
    /*public void MainMenu()
    {
        
    }*/
    public void CloseGame()
    {
        Application.Quit();
    }
}
