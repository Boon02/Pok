using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneDetails : MonoBehaviour
{
    public List<SceneDetails> connectedScenes;

    public bool Isloaded { get; private set; }

    private List<SavableEntity> savableEntities;
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

            var prevScene = GameController.Instance.PrevScene;
            if (prevScene != null)
            {
                var prevConnectedScenes = prevScene.connectedScenes;

                foreach (var scene in prevConnectedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnLoadScene();
                    }
                }
                
                if (!connectedScenes.Contains(prevScene))
                {
                    prevScene.UnLoadScene();
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!Isloaded)
        {
            Isloaded = true;
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntities();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
            
        }
    }
    
    public void UnLoadScene()
    {
        if (Isloaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);
            
            Isloaded = false;
            SceneManager.UnloadSceneAsync(gameObject.name);
        }
    }

    public List<SavableEntity> GetSavableEntities()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();

        return savableEntities;
    }

}
