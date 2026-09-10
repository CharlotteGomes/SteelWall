using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour, IInteractable
{
    //iniciador do dialogo
    public Dialogue dialogue;
    public bool playerNearby = false;

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            playerNearby = false;
        }
    }

    public void Interact()
    {
        GM.instance.previousScene = SceneManager.GetActiveScene().name; //guarda a cena onde o jogador tava

        GameObject player = GameObject.FindGameObjectWithTag("Player"); //guarda o dialogo do npc

        if (player != null)
        {
            GM.instance.playerPosition = player.transform.position;
        }

        GM.instance.returningFromDialogue = true;

        SceneManager.LoadScene("Teste");
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
