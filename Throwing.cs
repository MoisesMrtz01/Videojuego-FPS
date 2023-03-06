using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Throwing : MonoBehaviour
{

bool throwing = false, readyToThrow = true, reloading = false;

private void GameOver()
{
    text_info.SetText("Game Over");
    reloading = true;
    SceneManager.LoadScene("MenuPrincipal");
}

private void CheckWinCondition()
{
    if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
    {
        text_info.SetText("You win!");
        reloading = true;
        SceneManager.LoadScene("MenuPrincipal");
    }
    else
    {
        throwsLeft = totalThrows;
    }
}


    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public TextMeshProUGUI text_info;

    [Header("Stats")]
    public int totalThrows = 6;
    public float throwCooldown = 1f;
    public float timeBetweenThrows = 0.5f;

    [Header("Throwing")]
    public float throwForce = 20f;
    public float throwUpwardForce = 5f;
    public float spread = 1f;
    public int throwsPerTap = 1;

    int throwsLeft, throwsToExecute;

    [Header("RayCasting")]
    public bool useRaycasts;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    [Header("Extra Settings")]
    public bool allowButtonHold;

    private void Start()
    {
        throwsLeft = totalThrows;
        readyToThrow = true;
    }

    private void Update()
    {
        MyInput();
        text_info.SetText("Throws left: " + throwsLeft);
    }

    private void MyInput()
    {
        if (allowButtonHold)
            throwing = Input.GetKey(KeyCode.Mouse0);
        else
            throwing = Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToThrow && throwing && !reloading && throwsLeft > 0)
        {
            throwsToExecute = throwsPerTap;
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward + new Vector3(x, y, 0);

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized + new Vector3(x, y, 0);
        }

        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        if (useRaycasts)
        {
            if (Physics.Raycast(cam.transform.position, forceDirection, out rayHit, whatIsEnemy))
            {
                Debug.Log(rayHit.collider.name);
            }
        }

        throwsLeft--;
        throwsToExecute--;

        if (throwsToExecute > 0 && throwsLeft > 0)
        {
            Invoke(nameof(Throw), timeBetweenThrows);
        }
        else if (throwsToExecute <= 0)
        {
            Invoke(nameof(ResetThrow), throwCooldown);

            if (throwsLeft == 1)
            {
                text_info.SetText("Last throw!");
            }
            else if (throwsLeft <= 0)
            {
                GameOver();
            }

        }

    }

private void QuitGame()
{
    Application.Quit();
}
    private void ResetThrow()
    {
        CheckWinCondition();
    }
}



