using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform bow;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip shotClip;

    [SerializeField] private PlayerDetails player;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Animator bowAnimator;

    private float lastFireTime = 0f;
    private float fireRate = 0.5f;
    private float fireCharge = 5f;

    private void Start()
    {
        bowAnimator = bow.GetComponent<Animator>();
    }

    private void Update()
    {
        //if (Input.GetButton("Fire1") && !playerController.isDesactivated)
        //{
        //    bowAnimator.SetBool("isBending", true);
        //    if (fireCharge < 55f)
        //        fireCharge += Time.deltaTime * 33.3f;
        //}

        //if (Input.GetButtonUp("Fire1") && Time.time - lastFireTime > fireRate && !playerController.isDesactivated)
        //{
        //    Shoot();
        //    lastFireTime = Time.time;
        //}

        //if (Input.GetButtonUp("Fire1"))
        //{
        //    fireCharge = 5f;
        //    bowAnimator.SetBool("isBending", false);
        //}
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && !playerController.isDesactivated)
            Shoot();
    }

    void Shoot()
    {
        source.PlayOneShot(shotClip, 0.25f);

        GameObject arrow = Instantiate(arrowPrefab, bow.position, bow.rotation);
        ArrowDetails arrowComponent = arrow.GetComponent<ArrowDetails>();
        arrowComponent.SetShootingPlayer(player);
        arrowComponent.SetShootingForce(fireCharge);
    }
}