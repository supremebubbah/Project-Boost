
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{
    // Control cohete
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    //Sonidos cohete
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip sucess;

    //Particulas cohete
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem sucessParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    bool collisionDisable = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
        
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisable = !collisionDisable; //toggle
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state!= State.Alive || collisionDisable) { return; }
       
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                StartSucessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    private void StartSucessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(sucess);
        sucessParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {
        
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
            Debug.Log("Thrust= " + mainThrust); //añadido nuevo para solucion problema
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
               
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        mainEngineParticles.Play();

    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control 

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics
    }

    
}
