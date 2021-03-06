using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDetails : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private BumpSystem bumpSystem;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] hitClips;

    [SerializeField] private float maxPercentage = 999.9f;


    public PlayerController playerController;

    public int playerID;
    public string playerName;
    public Vector2 playerSpawn;
    public int wins;
    public int maxHealth = 3;
    public int currentHealth;
    public float currentPercentage;

    public bool isInvicible;
    public bool isDead;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        transform.position = playerSpawn;
        currentHealth = maxHealth;
    }

    public void Initialize(int _playerID, Vector2 _playerSpawn)
    {
        playerID = _playerID;
        playerSpawn = _playerSpawn;
        playerName = "Joueur " + (_playerID + 1);
    }

    public void RemoveArrows()
    {
        // Suppression des flèches plantées dans le joueur
        for (int i = 1; i < transform.childCount; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
    }

    public void Death()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth -= 1;

        // Appel de la fonction GameOver lorsque le joueur n'a plus de vies
        if (currentHealth == 0)
        {
            GameManager.Instance.GameOver();
            RemoveArrows();
            return;
        }

        StopCoroutine("RespawnAnimation");
        StartCoroutine("RespawnAnimation");

        RemoveArrows();

        currentPercentage = 0f;

        UIManager.Instance.GetPlayerUI(playerID).SetHealth(currentHealth);
        UIManager.Instance.GetPlayerUI(playerID).SetPercentage(0f);

        // Réinitialisation de la variable de bump
        bumpSystem.value = Vector2.zero;
    }

    public void Hit(Vector2 arrowVelocity)
    {
        if (isInvicible)
            return;

        source.PlayOneShot(hitClips[Random.Range(0, hitClips.Length)], 0.25f);

        StopCoroutine("DamagedAnimation");
        StartCoroutine("DamagedAnimation");

        // Ajout du bump de la flèche au joueur
        Vector2 arrowDamage = arrowVelocity * (currentPercentage / 100);

        bumpSystem.value = arrowDamage;

        // Etourdi le joueur pendant x secondes selon son pourcentage et les dégâts de la flèche
        //StopCoroutine("DamagedStun");
        //StartCoroutine(DamagedStun(arrowDamage.magnitude / 500));

        currentPercentage += arrowVelocity.magnitude * 0.37f;

        if (currentPercentage > maxPercentage)
            currentPercentage = maxPercentage;

        UIManager.Instance.GetPlayerUI(playerID).SetPercentage(currentPercentage);
    }

    private IEnumerator DamagedStun(float seconds)
    {
        playerController.SetDesactivateState(true);
        yield return new WaitForSeconds(seconds);
        playerController.SetDesactivateState(false);
    }

    private IEnumerator DamagedAnimation()
    {
        isInvicible = true;
        animator.SetBool("isDamaged", true);
        yield return new WaitForSeconds(0.25f);
        isInvicible = false;
        animator.SetBool("isDamaged", false);
    }

    private IEnumerator RespawnAnimation()
    {
        transform.position = playerSpawn;

        playerController.SetDesactivateState(true);
        rigidBody.velocity = Vector2.zero;
        animator.SetBool("isInvincible", false);
        animator.SetBool("isDead", true);
        isInvicible = true;

        yield return new WaitForSeconds(1f);

        isDead = false;
        animator.SetBool("isDead", false);
        animator.SetBool("isInvincible", true);
        playerController.SetDesactivateState(false);

        yield return new WaitForSeconds(2.5f);

        isInvicible = false;
        animator.SetBool("isInvincible", false);
    }

    public void ResetPlayer()
    {
        transform.position = playerSpawn;
        currentHealth = maxHealth;
        rigidBody.velocity = Vector2.zero;
        currentPercentage = 0f;
    }
}