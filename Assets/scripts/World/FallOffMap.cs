using UnityEngine;
using UnityEngine.SceneManagement;


public class FallOffMap : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player)
        {
            print(other.name);
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
        
        Destroy(other);
    }
}
