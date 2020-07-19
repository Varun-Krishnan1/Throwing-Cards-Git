using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplierPopupController : MonoBehaviour
{
    public GameObject multiplerPopup;
    public float moveYSpeed;
    public float moveXSpeed;
    public float popupLengthTime;
    public float disappearSpeed;
    public float scaleAmount;

    private Vector3 moveVector;
    private float popupLength;
    private static int sortingOrder;    // ensure later ones stack above earlier ones 


    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        popupLength = popupLengthTime;
        moveVector = new Vector3(moveXSpeed, moveYSpeed); // horizontal movement 
    }
    public void Setup(string multiplier)
    {
        textMesh.SetText(multiplier.ToString());

        sortingOrder += 1;
        textMesh.sortingOrder = sortingOrder;
    }

    public GameObject Create(Vector3 position, string multiplier)
    {
        GameObject popup = Instantiate(multiplerPopup, position, Quaternion.identity);
        // -- have to get the specific component of that popup 
        popup.GetComponent<MultiplierPopupController>().Setup(multiplier);
        return popup;
    }

    // Update is called once per frame
    void Update()
    {
        popupLength -= Time.deltaTime;

        // -- move popup up and to right 
        transform.position += moveVector * Time.deltaTime;
        // -- decellerate 
        moveVector -= moveVector * 8f * Time.deltaTime;

        // -- scale up and down 
        if (popupLength > .5 * popupLengthTime)
        {
            // First half of popup 
            transform.localScale += Vector3.one * scaleAmount * Time.deltaTime;
        }
        // else if(popupLength >= 0) <- for it to not go into bg 
        else
        {
            transform.localScale -= Vector3.one * scaleAmount * Time.deltaTime;
        }

        // -- when length ends start disappearing 
        if (popupLength <= 0)
        {
            // -- Start diappearing 
            textColor.a -= disappearSpeed * Time.deltaTime; // a = alpha 
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
