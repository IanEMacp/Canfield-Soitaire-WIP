using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public Sprite[] cardSprites;
    public GameObject cardPrefab;
    public string[] suits = new string[] {"Spades", "Clubs", "Hearts", "Diamonds"};
    public List<GameObject> stock = new List<GameObject>();
    public List<GameObject> reserve = new List<GameObject>();
    public List<GameObject> wastepile = new List<GameObject>();
    public List<GameObject> foundation1 = new List<GameObject>();
    public List<GameObject> foundation2 = new List<GameObject>();
    public List<GameObject> foundation3 = new List<GameObject>();
    public List<GameObject> foundation4 = new List<GameObject>();
    public int foundationBaseVal;
    public List<GameObject> unit1 = new List<GameObject>();
    public List<GameObject> unit2 = new List<GameObject>();
    public List<GameObject> unit3 = new List<GameObject>();
    public List<GameObject> unit4 = new List<GameObject>();
    public List<GameObject>[] tableau;
    public GameObject selectedCard = null;

    // Start is called before the first frame update
    void Start()
    {
        //add unit lists to tableau list
        tableau = new List<GameObject>[] {unit1, unit2, unit3, unit4};

        //add foundation and unit lists to their object
        GameObject.Find("unit1").GetComponent<cardBaseScript>().location = unit1;
        GameObject.Find("unit2").GetComponent<cardBaseScript>().location = unit2;
        GameObject.Find("unit3").GetComponent<cardBaseScript>().location = unit3;
        GameObject.Find("unit4").GetComponent<cardBaseScript>().location = unit4;
        GameObject.Find("foundation1").GetComponent<cardBaseScript>().location = foundation1;
        GameObject.Find("foundation2").GetComponent<cardBaseScript>().location = foundation2;
        GameObject.Find("foundation3").GetComponent<cardBaseScript>().location = foundation3;
        GameObject.Find("foundation4").GetComponent<cardBaseScript>().location = foundation4;
        
        // First we spawn the deck and add it to the stock
        int i = 0;
        foreach (Sprite img in cardSprites)
        {
            if (img == cardSprites[52])
            {
                break;
            }
            GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
            card.GetComponent<cardScript>().suit = suits[i % 4];
            card.GetComponent<cardScript>().value = i/4 + 1;
            card.GetComponent<cardScript>().face = img;
            stock.Add(card);
            card.GetComponent<cardScript>().location = stock;
            i++;
        }
        
        //shuffle the cards
        Shuffle(stock);
        foreach (GameObject thing in stock)
        {
            Vector3 pos = GameObject.Find("Stock").transform.position;
            pos.z += -0.1f;
            thing.transform.position = pos;
        }
        
        //deal to reserve
        for (int j = 0; j < 13; j++)
        {
            Vector3 pos = GameObject.Find("Reserve").transform.position;
            reserve.Insert(0, stock[0]);
            stock.RemoveAt(0);
            if (reserve.Count > 1)
            {
                pos = reserve[1].transform.position;
            }
            pos.z += -0.1f;
            reserve[0].transform.position = pos;
            reserve[0].GetComponent<cardScript>().faceUp = true;
            reserve[0].GetComponent<cardScript>().location = reserve;
            
        }

        //deal to foundation1 and set foundations
        Vector3 pos2 = GameObject.Find("foundation1").transform.position;
        foundation1.Insert(0, stock[0]);
        stock.RemoveAt(0);
        if ( foundation1.Count != 1)
        {
            pos2 =  foundation1[1].transform.position;
        }
        pos2.z += -0.1f;
        foundation1[0].transform.position = pos2;
        foundation1[0].GetComponent<cardScript>().faceUp = true;
        foundation1[0].GetComponent<cardScript>().location = foundation1;
        foundationBaseVal = foundation1[0].GetComponent<cardScript>().value;
        
        //deal to tableau
        for (int j = 0; j < 4; j++)
        {
            Vector3 pos3 = GameObject.Find("unit" + (j + 1)).transform.position;
            tableau[j].Insert(0, stock[0]);
            stock.RemoveAt(0);
            List<GameObject> currentUnit = tableau[j];
            if (tableau[j].Count != 1)
            {
                pos3 =  currentUnit[1].transform.position;
            }
            pos3.z += -0.1f;
            currentUnit[0].transform.position = pos3;
            currentUnit[0].GetComponent<cardScript>().faceUp = true;
            currentUnit[0].GetComponent<cardScript>().location = currentUnit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (foundation1.Count == 13 && foundation2.Count == 13 && foundation3.Count == 13 && foundation4.Count == 13)
        {
            print("you win");
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                //clicking the top of the stock deals three to the wastepile
                if (hit.collider.gameObject.name == "Stock Button")
                {
                    Vector3 pos4;

                    //if stock is empty
                    if (stock.Count < 1)
                    {
                        foreach (GameObject input in wastepile)
                        {
                            pos4 = GameObject.Find("Stock").transform.position;
                            stock.Insert(0, input);
                            if (stock.Count > 1)
                            {
                                pos4 = stock[1].transform.position;
                            }
                            pos4.z += -0.1f;
                            input.transform.position = pos4;
                            input.GetComponent<cardScript>().faceUp = false;
                            input.GetComponent<cardScript>().location = stock;
                        }
                        wastepile.Clear();
                    }
                    
                    for (int j = 0; j < 3; j++)
                    {
                        pos4 = GameObject.Find("Wastepile").transform.position;
                        if (stock.Count < 1)
                        {
                            break;
                        }
                        wastepile.Insert(0, stock[0]);
                        stock.RemoveAt(0);
                        if (wastepile.Count > 1)
                        {
                            pos4 = wastepile[1].transform.position;
                        }
                        pos4.z += -0.1f;
                        pos4.y = j * -0.7f + 2.5f;
                        wastepile[0].transform.position = pos4;
                        wastepile[0].GetComponent<cardScript>().faceUp = true;
                        wastepile[0].GetComponent<cardScript>().location = wastepile;
                    }
                }
                else if (hit.collider.CompareTag("Unit"))
                {
                    if (selectedCard != null)
                    {
                        MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, hit.collider.gameObject.GetComponent<cardBaseScript>().location, hit.collider.gameObject.name);
                    }
                }
                else if (hit.collider.CompareTag("Foundation"))
                {
                    if (selectedCard != null)
                    {
                        if (selectedCard.GetComponent<cardScript>().value == foundationBaseVal)
                        {
                            MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, hit.collider.gameObject.GetComponent<cardBaseScript>().location, hit.collider.gameObject.name);
                        }
                    }
                }
                else if (hit.collider.gameObject == wastepile[0])
                {
                    if (selectedCard == null)
                    {
                        selectedCard = hit.collider.gameObject;
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else
                    {
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                    }
                }
                else if (hit.collider.gameObject == reserve[0])
                {
                    if (selectedCard == null)
                    {
                        selectedCard = hit.collider.gameObject;
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else
                    {
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == unit1)
                {
                    if (hit.collider.gameObject == unit1[0])
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else if (CanPlaceOnUnit(hit.collider.gameObject))
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            Vector3 pos5 = GameObject.Find("unit1").transform.position;
                            unit1.Insert(0, selectedCard);
                            selectedCard.GetComponent<cardScript>().location.Remove(selectedCard);
                            if ( unit1.Count > 1)
                            {
                                pos5 =  unit1[1].transform.position;
                            }
                            pos5.z += -0.1f;
                            pos5.y += -0.7f;
                            unit1[0].transform.position = pos5;
                            unit1[0].GetComponent<cardScript>().faceUp = true;
                            unit1[0].GetComponent<cardScript>().location = unit1;
                            selectedCard = null;
                        }
                    }
                    else if (unit1.Contains(hit.collider.gameObject))
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            selectedCard = null;
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == unit2)
                {
                    if (hit.collider.gameObject == unit2[0])
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else if (CanPlaceOnUnit(hit.collider.gameObject))
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            Vector3 pos5 = GameObject.Find("unit2").transform.position;
                            unit2.Insert(0, selectedCard);
                            selectedCard.GetComponent<cardScript>().location.Remove(selectedCard);
                            if ( unit2.Count > 1)
                            {
                                pos5 =  unit2[1].transform.position;
                            }
                            pos5.z += -0.1f;
                            pos5.y += -0.7f;
                            unit2[0].transform.position = pos5;
                            unit2[0].GetComponent<cardScript>().faceUp = true;
                            unit2[0].GetComponent<cardScript>().location = unit2;
                            selectedCard = null;
                        }
                    }
                    else if (unit2.Contains(hit.collider.gameObject))
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            selectedCard = null;
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == unit3)
                {
                    if (hit.collider.gameObject == unit3[0])
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else if (CanPlaceOnUnit(hit.collider.gameObject))
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            Vector3 pos5 = GameObject.Find("unit3").transform.position;
                            unit3.Insert(0, selectedCard);
                            selectedCard.GetComponent<cardScript>().location.Remove(selectedCard);
                            if ( unit3.Count > 1)
                            {
                                pos5 =  unit3[1].transform.position;
                            }
                            pos5.z += -0.1f;
                            pos5.y += -0.7f;
                            unit3[0].transform.position = pos5;
                            unit3[0].GetComponent<cardScript>().faceUp = true;
                            unit3[0].GetComponent<cardScript>().location = unit3;
                            selectedCard = null;
                        }
                    }
                    else if (unit3.Contains(hit.collider.gameObject))
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            selectedCard = null;
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == unit4)
                {
                    if (hit.collider.gameObject == unit4[0])
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else if (CanPlaceOnUnit(hit.collider.gameObject))
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            Vector3 pos5 = GameObject.Find("unit4").transform.position;
                            unit4.Insert(0, selectedCard);
                            selectedCard.GetComponent<cardScript>().location.Remove(selectedCard);
                            if ( unit4.Count > 1)
                            {
                                pos5 =  unit4[1].transform.position;
                            }
                            pos5.z += -0.1f;
                            pos5.y += -0.7f;
                            unit4[0].transform.position = pos5;
                            unit4[0].GetComponent<cardScript>().faceUp = true;
                            unit4[0].GetComponent<cardScript>().location = unit4;
                            selectedCard = null;
                        }
                    }
                    else if (unit4.Contains(hit.collider.gameObject))
                    {
                        if (selectedCard == null)
                        {
                            selectedCard = hit.collider.gameObject;
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        else
                        {
                            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                            selectedCard = null;
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == foundation1)
                {
                    if (selectedCard != null)
                    {
                        if (selectedCard.GetComponent<cardScript>().suit == hit.collider.gameObject.GetComponent<cardScript>().suit)
                        {
                            if (selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value + 1 || selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value - 12)
                            {
                                MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, foundation1, "foundation1");
                            }
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == foundation2)
                {
                    if (selectedCard != null)
                    {
                        if (selectedCard.GetComponent<cardScript>().suit == hit.collider.gameObject.GetComponent<cardScript>().suit)
                        {
                            if (selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value + 1 || selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value - 12)
                            {
                                MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, foundation2, "foundation2");
                            }
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == foundation3)
                {
                    if (selectedCard != null)
                    {
                        if (selectedCard.GetComponent<cardScript>().suit == hit.collider.gameObject.GetComponent<cardScript>().suit)
                        {
                            if (selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value + 1 || selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value - 12)
                            {
                                MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, foundation3, "foundation3");
                            }
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<cardScript>().location == foundation4)
                {
                    if (selectedCard != null)
                    {
                        if (selectedCard.GetComponent<cardScript>().suit == hit.collider.gameObject.GetComponent<cardScript>().suit)
                        {
                            if (selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value + 1 || selectedCard.GetComponent<cardScript>().value == hit.collider.gameObject.GetComponent<cardScript>().value - 12)
                            {
                                MoveCardTo(selectedCard, selectedCard.GetComponent<cardScript>().location, foundation4, "foundation4");
                            }
                        }
                    }
                }
            }
        }
    }

    //The following shuffle method is not mine. I pulled it from stackoverflow: https://stackoverflow.com/questions/273313/randomize-a-listt
    void Shuffle<T>(List<T> list)  
    {  
        System.Random rng = new System.Random();
        int n = list.Count;  
        while (n > 1)
        {
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
    }

    bool CanPlaceOnUnit(GameObject input)
    {
        if (input.GetComponent<cardScript>().value - 1 == selectedCard.GetComponent<cardScript>().value || input.GetComponent<cardScript>().value == selectedCard.GetComponent<cardScript>().value - 12)
        {
            if (input.GetComponent<cardScript>().suit == "Diamonds" || input.GetComponent<cardScript>().suit == "Hearts")
            {
                if (selectedCard.GetComponent<cardScript>().suit == "Clubs" || selectedCard.GetComponent<cardScript>().suit == "Spades")
                {
                    return true;
                }
            }
            else if (input.GetComponent<cardScript>().suit == "Clubs" || input.GetComponent<cardScript>().suit == "Spades")
            {
                if (selectedCard.GetComponent<cardScript>().suit == "Diamonds" || selectedCard.GetComponent<cardScript>().suit == "Hearts")
                {
                    return true;
                }
            }
        }
        return false;
    }

    void MoveCardTo(GameObject input, List<GameObject> destinationFrom, List<GameObject> destinationTo, string destinationName)
    {
        Vector3 posi;
        posi = GameObject.Find(destinationName).transform.position;
        destinationTo.Insert(0, destinationFrom[0]);
        destinationFrom.RemoveAt(0);
        if (destinationTo.Count > 1)
        {
            posi = destinationTo[1].transform.position;
        }
        posi.z += -0.1f;
        input.transform.position = posi;
        input.GetComponent<cardScript>().faceUp = true;
        input.GetComponent<cardScript>().location = destinationTo;
        if (selectedCard != null)
        {
            selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
            selectedCard = null;
        }
    }
}
