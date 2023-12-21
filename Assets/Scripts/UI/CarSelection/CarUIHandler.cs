using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour
{
    [Header("Car Details")]
    [SerializeField] private Image carImage;
    private Animator animator = null;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void SetupCar(CarData carData)
    {
        carImage.sprite = carData.CarUISprite;
    }
    public void StartCarEntranceAnimation(bool isAppearingOnRightSide)
    {
        if (isAppearingOnRightSide)
        {
            animator.Play("Car UI Appear From Right");
        }
        else
        {
            animator.Play("Car UI Appear From Left");
        }
    }
    public void StartCarExitAnimation(bool isExitingOnRightSide)
    {
        if (isExitingOnRightSide)
        {
            animator.Play("Car UI Disappear To Right");
        }
        else
        {
            animator.Play("Car UI Disappear To Left");
        }
    }
    public void OnExitAnimationCompleted()
    {
        Destroy(gameObject);
    }
}
