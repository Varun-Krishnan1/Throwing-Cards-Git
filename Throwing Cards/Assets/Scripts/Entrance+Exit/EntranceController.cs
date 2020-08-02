using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceController : MonoBehaviour
{
    public GameObject player;
    public float enterSpeed;
    public float jumpForce; 
    public float totalEnterTime;        // time delay before control returned to player 

    // Start is called before the first frame update
    void Awake()
    {
        // -- Entrance ANIMATION -- 

        // -- start player at correct location 
        player.transform.position = this.transform.position; 
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        // -- don't let them move 
        Controller2D controller = player.GetComponent<Controller2D>();
        controller.enabled = false;

        // -- make them jump in direction they should go 
        rb.AddForce(new Vector2(0f, jumpForce));
        player.GetComponent<Animator>().SetBool("isJumping", true);

        // -- make them move while jumping in the right direction 
        rb.velocity = new Vector2(enterSpeed, rb.velocity.y);


        // -- after time delay allow them to move and start scene 
        StartCoroutine(StartScene(totalEnterTime));
    }

    // -- after time delay allow them to control character 
    IEnumerator StartScene(float time)
    {
        yield return new WaitForSeconds(time);

        GameManager.instance.LoadPlayer(player);

        player.GetComponent<Controller2D>().enabled = true; 


    }

}
