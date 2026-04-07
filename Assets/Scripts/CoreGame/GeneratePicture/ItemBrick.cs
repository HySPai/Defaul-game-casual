using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBrick : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private SpriteRenderer sprite;
    public void GetDataColor(Color color)
    {
        //mesh.material.color = color;
        sprite.color = color;
        sprite.DOFade(0, 0);
    }
    public void SetHint()
    {
        sprite.DOFade(0.3f, 0);
    }
}
