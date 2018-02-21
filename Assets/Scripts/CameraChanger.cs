using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraChanger : MonoBehaviour {

    //public KeyValuePair<Camera, RenderTexture> textures;
    public RenderTexture screen_texture;
    public List<Camera> cams;
    private Camera cur_cam;
    GameObject game_manager;

    private void Start()
    {
        cur_cam = cams[0];
        cur_cam.targetTexture = screen_texture;
        cur_cam.enabled = true;
        game_manager = GameObject.FindGameObjectWithTag("GameManager");
        game_manager.GetComponent<GameControl>().cam_control = this;
    }

	public void turn_cam(int k)
    {
        cur_cam.enabled = false;
        cur_cam = cams[k];
        cur_cam.targetTexture = screen_texture;
        cur_cam.enabled = true;
    }
}
