using UnityEngine;

public interface ICharacterAnimator
{
    public void SetTrigger(string triggerName);
    public void SetBool(string boolName, bool value);
    public void SetInteger(string intName, int value);
    public void Play(string stateName);

}
