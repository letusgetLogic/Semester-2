using UnityEngine;

public class DetectorAttackRange : MonoBehaviour
{
    [SerializeField]
    private EnemyStateSystem chaseSystem;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            var player = collider.GetComponent<PlayerController>();
            chaseSystem.AttackPlayer(player, true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            var player = collider.GetComponent<PlayerController>();
            chaseSystem.AttackPlayer(player, false);
        }
    }

    /* Old code
     
    [SerializeField]
    private EnemyController enemyController;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            enemyController.IsPlayerInAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            enemyController.IsPlayerInAttackRange = false;
        }
    }

    /// <summary>
    /// IsPlayerInRange checks if the player is within the collider.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsPlayerInRange(PlayerController player)
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
