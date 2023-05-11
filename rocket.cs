using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{
    [SerializeField]float rcsThrust = 100f;
    [SerializeField]float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    
    {
        if(state == State.Alive)
        {
        RespondToThrustInput();
        RespondToRotateInput(); 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
            {
                return;
            }  
        switch(collision.gameObject.tag)
        {

            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();   
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
                
    void StartSuccessSequence()
    {
         state = State.Transcending;
         audioSource.Stop();
         audioSource.PlayOneShot(success);
         successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartDeathSequence()
    {
         state = State.Dying;
         audioSource.Stop();
         audioSource.PlayOneShot(death);
         deathParticles.Play();
         Invoke("LoadFirstLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1; 
      
        if (currentSceneIndex == 4)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    void LoadFirstLevel()
    {
         SceneManager.LoadScene(0);
    }

    void RespondToThrustInput()
    {
            if (Input.GetKey(KeyCode.Space)) // can trust while rotating
        {
           ApplyThrust();
        }
            else 
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    void ApplyThrust()
    { 
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            if(!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        
    }

    void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) 
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}