using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum Thought { Hungry, Sad }

public class CompanimonThoughtHandler : MonoBehaviour
{
    public SpriteRenderer ImageThought;

    public Sprite SpriteHungry;
    public Sprite SpriteSad;

    public GameObject Cover;

    // Start is called before the first frame update
    void Start()
    {
        Cover.transform.localPosition = new Vector3(Cover.transform.localPosition.x, -22, Cover.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void ShowThought(Thought thought)
    {
        switch (thought)
        {
            case Thought.Hungry:
                ImageThought.sprite = SpriteHungry;
                break;
            case Thought.Sad:
                ImageThought.sprite = SpriteSad;
                break;
        }

        Cover.transform.localPosition = new Vector3(Cover.transform.localPosition.x, Cover.transform.localPosition.y + 26, Cover.transform.localPosition.z);
        await Task.Delay(800);

        Cover.transform.localPosition = new Vector3(Cover.transform.localPosition.x, Cover.transform.localPosition.y + 22, Cover.transform.localPosition.z);
        await Task.Delay(800);

        Cover.gameObject.SetActive(false);
        await Task.Delay(3000);

        Cover.transform.localPosition = new Vector3(Cover.transform.localPosition.x, Cover.transform.localPosition.y - 48, Cover.transform.localPosition.z);
        Cover.gameObject.SetActive(true);
    }
}
