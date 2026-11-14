using UnityEngine;

public class CarSelect : MonoBehaviour
{
    [SerializeField] public GameObject[] gameUI; 
    [SerializeField] public GameObject[] cars;

    public void Car1()
    {
        gameUI[0].SetActive(false);
        gameUI[1].SetActive(true);
        cars[0].SetActive(true);
    }
    
    public void Car2()
    {
        gameUI[0].SetActive(false);
        gameUI[1].SetActive(true);
        cars[1].SetActive(true);
    }
    
    public void Car3()
    {
        gameUI[0].SetActive(false);
        gameUI[1].SetActive(true);
        cars[2].SetActive(true);
    }
}
