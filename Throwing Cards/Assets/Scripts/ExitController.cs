using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    public float playerExitTime; 
    public float exitSpeed;
    public float jumpForce; 
    public bool exitFacingRight;

    public Animator doorAnimator; 

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.tag == "Player")
        {
            // -- EXIT ANIMATION -- 
            Rigidbody2D rb = hitInfo.GetComponent<Rigidbody2D>();

            // -- don't let them move 
            Controller2D controller = hitInfo.GetComponent<Controller2D>();
            controller.enabled = false; 

            // -- flip them right direction depending on next level
            if(controller.isFacingRight() != exitFacingRight)
            {
                controller.Flip(); 
            }

            // -- make them jump in direction they should go 
            rb.AddForce(new Vector2(0f, jumpForce));
            hitInfo.GetComponent<Animator>().SetBool("isJumping", true);
            rb.velocity = new Vector2(exitSpeed, rb.velocity.y);


            // -- after time delay hide the player and load the next scene 
            StartCoroutine(LoadNextScene(hitInfo.gameObject));
        }
    }

    IEnumerator LoadNextScene(GameObject player)
    {
        yield return new WaitForSeconds(playerExitTime);

        player.SetActive(false);

        doorAnimator.SetBool("isSliding", true); 
        

    }

    // -- CALLED AT END OF DOOR ANIMATION DO NOT CALL MANUALLY 
    public void GoNextLevel()
    {
        GameManager.instance.LoadNextScene();
    }

}
