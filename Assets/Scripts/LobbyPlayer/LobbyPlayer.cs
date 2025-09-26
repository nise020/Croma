using UnityEngine;
using static Enums;

public class LobbyPlayer : Player
{
    [SerializeField] private Transform weaponPoint;
    
    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.LobbyPlayer;


    protected override void Awake()
    {
        CharacterAnimator = GetComponent<Animator>();
        if (playerInPut) 
            playerInPut.enabled = false;
        //buffSystem = null;
    }

    private void Update() { }

    protected override void Start() { /* no base.Start() */ }
   
}
