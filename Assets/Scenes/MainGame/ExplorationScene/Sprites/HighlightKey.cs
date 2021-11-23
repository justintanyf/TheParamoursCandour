using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightKey : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;
    public string key;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null) spriteRenderer.sprite = sprite1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(key))
        {
            spriteRenderer.sprite = sprite2;
        }
        else
        {
            spriteRenderer.sprite = sprite1;
        }
    }
}
