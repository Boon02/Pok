using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection{Up, Down, Left, Right}

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    [SerializeField] private List<Sprite> walkRightSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;
    
    // parameters
    public float MoveX { get; set; } = 0;
    public float MoveY { get; set; } = 0;
    public bool IsMoving { get; set; } = false;
    public FacingDirection DefaultDirection { get => defaultDirection; }
    bool WasPreviouslyMoving;
    
    // States
    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkLeftAnim;
    private SpriteAnimator walkRightAnim;

    private SpriteAnimator currentAnim;
    
    // Refrence
    private SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        SetFacingDirection(defaultDirection);
        
        currentAnim = walkDownAnim;
    }
    
    private void Update()
    {
        var prevAnim = currentAnim;
        
        if (MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if(MoveX == -1)
        {
            currentAnim = walkLeftAnim;
        }
        else if(MoveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if(MoveY == -1)
        {
            currentAnim = walkDownAnim;
        }

        if(prevAnim != currentAnim && WasPreviouslyMoving != IsMoving)
            currentAnim.Start();
        
        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        WasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Down)
        {
            MoveY = -1f;
        }else if (dir == FacingDirection.Up)
        {
            MoveY = 1f;
        }else if (dir == FacingDirection.Left)
        {
            MoveX = -1f;
        }else if (dir == FacingDirection.Right)
        {
            MoveX = 1f;
        }
    }
}
