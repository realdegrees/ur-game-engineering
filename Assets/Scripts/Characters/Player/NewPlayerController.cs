using UnityEngine;

[RequireComponent(typeof(CharacterStateMachine))]
public class NewPlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementConfig config;
    private CharacterStateMachine stateMachine;
    private void Awake()
    {
        TryGetComponent(out stateMachine);
        InputManager.Instance.onMovementStart.AddListener((_) => stateMachine.ChangeState(ECharacterState.Moving));
    }
    private void Update()
    {

    }
}