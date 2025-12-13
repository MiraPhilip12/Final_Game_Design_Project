using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class CharacterAnimations : MonoBehaviour
{
    public Animator characterAnimator;
    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (characterAnimator == null) Debug.LogError("CharacterAnimations: characterAnimator reference is null.");
    }
    void OnJump(InputValue value)
    {
        Debug.Log("OnJump");
        if (value.isPressed)
        {
            characterAnimator.SetTrigger("Jump");
        }
    }
}