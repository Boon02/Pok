using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneDetails : MonoBehaviour
{
    public List<SceneDetails> connectedScenes;

    public bool Isloaded { get; private set; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            
            foreach (var item in connectedScenes)
            {
                item.LoadScene();
            }

            if (GameController.Instance.PrevScene != null)
            {
                var prevConnectdScenes = GameController.Instance.PrevScene.connectedScenes;

                foreach (var scene in prevConnectdScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnLoadScene();
                    }
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!Isloaded)
        {
            Isloaded = true;
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        }
    }
    
    public void UnLoadScene()
    {
        if (Isloaded)
        {
            Isloaded = false;
            SceneManager.UnloadSceneAsync(gameObject.name);
        }
    }
}
