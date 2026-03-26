using UnityEngine;

public class DetectorPlayer : MonoBehaviour
{
    [SerializeField] 
    private EnemyStateSystem chaseSystem;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            var player = collider.GetComponent<PlayerController>();
            chaseSystem.DetectPlayer(player, true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            var player = collider.GetComponent<PlayerController>();
            chaseSystem.DetectPlayer(player, false);
        }
    }

    /* Old code
    
    /// <summary>
    /// IsPlayerInSights checks if the player is within the aura's collider.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsPlayerInSights(PlayerController player)
    {
        var sphere = Physics.OverlapSphere(
            transform.position, GetComponent<SphereCollider>().radius);

        if (sphere == null)
            return false;

        if (sphere.Length == 0)
            return false;

        foreach (var collider in sphere)
        {
            PlayerController checkedPlayer = collider.GetComponent<PlayerController>();

            if (checkedPlayer == player)
                return true;
        }

        return false;
    }
    */
}
