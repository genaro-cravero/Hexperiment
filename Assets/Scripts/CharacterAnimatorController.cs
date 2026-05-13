using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour, ICharacterAnimator
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBool(string boolName, bool value)
    {
        _animator.SetBool(boolName, value);
    }

    public void SetInteger(string intName, int value)
    {
        _animator.SetInteger(intName, value);
    }

    public void SetTrigger(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }

    public void Play(string stateName)
    {
        _animator.Play(stateName, 0, 0f);
    }
}
