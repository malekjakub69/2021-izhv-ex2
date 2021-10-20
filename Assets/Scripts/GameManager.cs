using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The main game manager GameObject.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// GameObject used for the start text.
    /// </summary>
    public GameObject startText;
    
    /// <summary>
    /// GameObject used for the loss text.
    /// </summary>
    public GameObject lossText;
    
    /// <summary>
    /// GameObject used for the score text.
    /// </summary>
    public GameObject scoreText;
    
    /// <summary>
    /// GameObject representing the main Player character.
    /// </summary>
    public GameObject player;
    
    /// <summary>
    /// GameObject representing the spawner.
    /// </summary>
    public GameObject spawner;

    /// <summary>
    /// Current accumulated score.
    /// </summary>
    private float mCurrentScore = 0.0f;

    /// <summary>
    /// Is the game lost?
    /// </summary>
    private bool mGameLost = false;

    /// <summary>
    /// Did we start the game?
    /// </summary>
    private static bool sGameStarted = false;

    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    private static GameManager sInstance;
    
    /// <summary>
    /// Getter for the singleton GameManager object.
    /// </summary>
    public static GameManager Instance
    { get { return sInstance; } }

    /// <summary>
    /// Called when the script instance is first loaded.
    /// </summary>
    private void Awake()
    {
        // Initialize the singleton instance, if no other exists.
        if (sInstance != null && sInstance != this)
        { Destroy(gameObject); }
        else
        { sInstance = this; }
        
        // Setup the game scene.
        SetupGame();
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { }

    /// <summary>
    /// Update called once per frame.
    /// </summary>
    void Update()
    {
        // Start the game after the first "Jump".
        if (!sGameStarted && Input.GetButtonDown("Jump"))
        { StartGame(); }
        
        // Reset the game if requested.
        if (Input.GetButtonDown("Cancel"))
        { ResetGame(); }

        if (sGameStarted && !mGameLost)
        {
            // Increment the score by elapsed time.
            mCurrentScore += Time.deltaTime;
            // Update the score text.
            GetChildNamed(scoreText, "Value").GetComponent<Text>().text = $"{(int)(mCurrentScore)}";
        }
    }

    /// <summary>
    /// Setup the game scene.
    /// </summary>
    public void SetupGame()
    {
        // Reset the gravity.
        Physics2D.gravity = -new Vector2(
            Math.Abs(Physics2D.gravity.x),
            Math.Abs(Physics2D.gravity.y)
        );
        
        if (sGameStarted)
        { // Setup already started game -> Retry.
            startText.SetActive(false);
            scoreText.SetActive(true);
            lossText.SetActive(false);
        }
        else
        { // Setup a new game -> Wait for start.
            // Don't start spawning until we start.
            spawner.GetComponent<Spawner>().spawnObstacles = false;
            
            // Setup the text.
            startText.SetActive(true);
            scoreText.SetActive(false);
            lossText.SetActive(false);
        }
        
        // Set the state.
        mGameLost = false;
    }

    /// <summary>
    /// Set the game to the "started" state.
    /// </summary>
    public void StartGame()
    {
        // Reload the scene as started.
        sGameStarted = true; 
        ResetGame();
    }
    
    /// <summary>
    /// Reset the game to the default state.
    /// </summary>
    public void ResetGame()
    {
        // Reload the active scene, triggering reset...
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Set the game to the "lost" state.
    /// </summary>
    public void LooseGame()
    {
        // Get the spawner script.
        var sp = spawner.GetComponent<Spawner>();
        // Stop the obstacles.
        sp.ModifyObstacleSpeed(0.0f);
        // Stop spawning.
        sp.spawnObstacles = false;
        // Show the loss text.
        lossText.SetActive(true);
        // Loose the game.
        mGameLost = true;
    }
    
    /// <summary>
    /// Get child of a GameObject by name.
    /// </summary>
    /// <param name="go">Target GameObject.</param>
    /// <param name="name">Target child name.</param>
    /// <returns>Returns the child or null of no child with such name exists.</returns>
    private static GameObject GetChildNamed(GameObject go, string name) 
    {
        var childTransform = go.transform.Find(name);
        return childTransform == null ? null : childTransform.gameObject;
    }
}
