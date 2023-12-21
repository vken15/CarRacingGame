using UnityEngine;
using UnityEngine.UI;

public class MapUIHandler : MonoBehaviour
{
    [Header("Map Details")]
    [SerializeField] private Image mapImage;
    private Animator animator = null;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void SetupMap(MapData mapData)
    {
        mapImage.sprite = mapData.MapUISprite;
    }
    public void StartMapEntranceAnimation(bool isAppearingOnRightSide)
    {
        if (isAppearingOnRightSide)
        {
            animator.Play("Map UI Appear From Right");
        }
        else
        {
            animator.Play("Map UI Appear From Left");
        }
    }
    public void StartMapExitAnimation(bool isExitingOnRightSide)
    {
        if (isExitingOnRightSide)
        {
            animator.Play("Map UI Disappear To Right");
        }
        else
        {
            animator.Play("Map UI Disappear To Left");
        }
    }
    public void OnExitAnimationCompleted()
    {
        Destroy(gameObject);
    }
}
