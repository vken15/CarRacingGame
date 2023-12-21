using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PhysicsObjectsLayerHandler : MonoBehaviour
{
    private Collider2D objectCollider;
    [SerializeField] private SpriteRenderer objectOutlineSpriteRenderer;
    private readonly List<SpriteRenderer> defaultLayerSpriteRenderers = new();
    private readonly List<Collider2D> overpassColliderList = new();
    private readonly List<Collider2D> underpassColliderList = new();
    private bool isOnOverpass = false;
    private void Awake()
    {
        foreach (SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer.sortingLayerName == "Default")
            {
                defaultLayerSpriteRenderers.Add(renderer);
            }
        }
        foreach (GameObject overpassColliderGameObject in GameObject.FindGameObjectsWithTag("OverpassCollider"))
        {
            overpassColliderList.Add(overpassColliderGameObject.GetComponent<Collider2D>());
        }
        foreach (GameObject underpassColliderGameObject in GameObject.FindGameObjectsWithTag("UnderpassCollider"))
        {
            underpassColliderList.Add(underpassColliderGameObject.GetComponent<Collider2D>());
        }
        objectCollider = GetComponentInChildren<Collider2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        UpdateSortingAndCollisionLayers();
    }

    private void UpdateSortingAndCollisionLayers()
    {
        if (isOnOverpass)
        {
            SetSortingLayer("RaceTrackOverpass");
            objectOutlineSpriteRenderer.enabled = false;
        }
        else
        {
            SetSortingLayer("Default");
            objectOutlineSpriteRenderer.enabled = true;
        }
        SetCollisionWithOverPass();
    }

    private void SetCollisionWithOverPass()
    {
        foreach (Collider2D collider2D in overpassColliderList)
        {
            Physics2D.IgnoreCollision(objectCollider, collider2D, !isOnOverpass);
        }
        foreach (Collider2D collider2D in underpassColliderList)
        {
            Physics2D.IgnoreCollision(objectCollider, collider2D, isOnOverpass);
        }
    }

    private void SetSortingLayer(string layerName)
    {
        foreach (SpriteRenderer renderer in defaultLayerSpriteRenderers)
        {
            renderer.sortingLayerName = layerName;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UnderpassTrigger"))
        {
            isOnOverpass = false;
            objectCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnUnderpass");
            UpdateSortingAndCollisionLayers();
        }
        else if (collision.CompareTag("OverpassTrigger"))
        {
            isOnOverpass = true;
            objectCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnOverpass");
            UpdateSortingAndCollisionLayers();
        }
    }
}
