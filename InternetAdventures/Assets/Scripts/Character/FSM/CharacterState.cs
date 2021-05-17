using UnityEngine;

public abstract class CharacterState : MonoBehaviour
{
    //This class provides the basic structure for a state that the character can use.
    //The subclasses can override all methods to extend and adjust their functionality based on their use case.
    
    protected CharacterFSM _characterFSM;
    
    public virtual void Initialize(CharacterFSM pCharacterFSM)
    {
        _characterFSM = pCharacterFSM;
    }
    
    public virtual void EnterState()
    {
        //Enters state
    }

    public virtual void Update()
    {
        //Updates state
    }

    public virtual void ExitState()
    {
        //Exits state
    }
}
