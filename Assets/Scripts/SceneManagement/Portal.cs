using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int sceneNum = -1;
    [SerializeField] private DestinationIdentifider destinationPortal;
    [SerializeField] private Transform spawnPoint;
    
    private PlayerController player;

    private Fader fader;
    
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);
        yield return fader.FaderIn(0.5f);
        yield return SceneManager.LoadSceneAsync(sceneNum);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        
        yield return fader.FaderOut(0.5f);
        GameController.Instance.PauseGame(false);
    
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifider {A, B, C, D, E, F, J}