using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardScript : MonoBehaviour
{
    public bool faceUp = false;
    public Sprite face;
    public Sprite back;
    public string suit;
    public int value;
    public List<GameObject> location;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (faceUp == true) 
        {
            GetComponent<SpriteRenderer>().sprite = face;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = back;
        }
    }
}
