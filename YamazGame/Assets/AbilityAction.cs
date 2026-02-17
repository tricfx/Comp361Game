using UnityEngine;

public class AbilityAction : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] private MonoBehaviour abilityQ; // drag a component like Heal here
    [SerializeField] private MonoBehaviour abilityE;
    

    private IAbility Q => abilityQ as IAbility;
    private IAbility E => abilityE as IAbility;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Q?.Do();

        if (Input.GetKeyDown(KeyCode.E))
            E?.Do();
    }
}
