using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneNum = -1;
    [SerializeField] private Transform spawnPoint;
    
    private PlayerController player;
    
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(sceneNum);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
    
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}