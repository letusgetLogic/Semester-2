using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    /// <summary>
    /// Destroys the object that enters the trigger collider.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}

