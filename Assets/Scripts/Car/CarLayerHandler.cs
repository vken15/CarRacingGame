using System.Collections.Generic;
using UnityEngine;

public class CarLayerHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer carOutlineSpriteRenderer;
    private readonly List<SpriteRenderer> defaultLayerSpriteRenderers = new();
    private readonly List<Collider2D> overpassColliderList = new();
    private readonly List<Collider2D> underpassColliderList = new();
    private Collider2D carCollider;

    //State
    private bool isDrivingOnOverpass = false;
    private bool isDrivingOnRoad = true;

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
        carCollider = GetComponentInChildren<Collider2D>();
        //Default drive on underpass
        carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnUnderpass");
    }

    // Start is called before the first frame update
    private void Start()
    {
        UpdateSortingAndCollisionLayers();
    }

    private void UpdateSortingAndCollisionLayers()
    {
        if (isDrivingOnOverpass)
        {
            SetSortingLayer("RaceTrackOverpass");
            carOutlineSpriteRenderer.enabled = false;
        }
        else
        {
            SetSortingLayer("Default");
            carOutlineSpriteRenderer.enabled = true;
        }
        SetCollisionWithOverPass();
    }

    private void SetCollisionWithOverPass()
    {
        foreach (Collider2D collider2D in overpassColliderList)
        {
            Physics2D.IgnoreCollision(carCollider, collider2D, !isDrivingOnOverpass);
        }
        foreach (Collider2D collider2D in underpassColliderList)
        {
            Physics2D.IgnoreCollision(carCollider, collider2D, isDrivingOnOverpass);
        }
    }

    private void SetSortingLayer(string layerName)
    {
        foreach (SpriteRenderer renderer in defaultLayerSpriteRenderers)
        {
            renderer.sortingLayerName = layerName;
        }
    }
    public bool IsDrivingOnOverpass()
    {
        return isDrivingOnOverpass;
    }
    public bool IsDrivingOnRoad()
    {
        return isDrivingOnRoad;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UnderpassTrigger"))
        {
            isDrivingOnOverpass = false;
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnUnderpass");
            UpdateSortingAndCollisionLayers();
        }
        else if (collision.CompareTag("OverpassTrigger"))
        {
            isDrivingOnOverpass = true;
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnOverpass");
            UpdateSortingAndCollisionLayers();
        }
        if (collision.CompareTag("Road")) {
            isDrivingOnRoad = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Road"))
        {
            isDrivingOnRoad = false;
        }
    }
}
