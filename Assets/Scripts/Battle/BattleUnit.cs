using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private BattleHud hud;

    public bool IsPlayer
    {
        get { return isPlayer; }
    }
    public BattleHud Hud
    {
        get { return hud; }
    }

    public Pokemon Pokemon { get; set; }

    private Image image;
    private Vector3 orginalPos;
    private Color orginalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.transform.localPosition;
        orginalColor = image.color;
    }

    public void SetUp(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayer)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FontSprite;
        }
        transform.localScale = new Vector3(1f, 1f, 1f);
        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);
        image.color = orginalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayer)
        {
            gameObject.transform.localPosition = new Vector3(-1163, orginalPos.y);
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(1163, orginalPos.y);
        }

        gameObject.transform.DOLocalMoveX(orginalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer)
        {
            sequence.Append(gameObject.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(gameObject.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));
        }
        
        sequence.Append(gameObject.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(orginalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(gameObject.transform.DOLocalMoveY(orginalPos.y - 80f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
    
    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.5f));
        sequence.Join(transform.DOLocalMoveY(orginalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));

        yield return sequence.WaitForCompletion();
    }
    
    public IEnumerator PlayBrokeOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, 0.5f));
        sequence.Join(transform.DOLocalMoveY(orginalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));

        yield return sequence.WaitForCompletion();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }
    
    public void UnClear()
    {
        hud.gameObject.SetActive(true);
    }
    
}
