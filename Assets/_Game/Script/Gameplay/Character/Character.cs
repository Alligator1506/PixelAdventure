using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : AbCharacter
{
    [SerializeField] private Animator anim;
    private string currentAnimName;

    public void Start()
    {
        OnInit();

    }

    public override void OnDespawn()
    {
    }

    public override void OnInit()
    {

    }

    public override void OnDeath()
    {

    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }
}
