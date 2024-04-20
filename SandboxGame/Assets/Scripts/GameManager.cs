using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;


public class GameManager : MonoBehaviour
{
    public Canvas canvas;
    public int m_NumRoundsToWin = 5;     
    public GameObject nameObject;
    public MallardManager[] mallard_list; // This is the list of all mallards
    public GameObject mallard_prefab; // This is the mallard prefab to instantiate when a player joins
    private PlayerJoin[] playerJoins;
    private bool m_GameStarted = false;
    public string m_RoomCode;
    public TextMeshProUGUI m_RoomCodeText;
    public TextMeshProUGUI m_RoomCodePrefaceText;
    private MallardManager m_GameWinner;
    private bool handlingPlus = false;
    private bool handlingMinus = false;
    public TextMeshProUGUI m_HostStartText;
    public CameraControl m_CameraControl;
    public GameObject[] m_SpawnPoints;
    public TextMeshProUGUI m_HostExitText;
    public TextMeshProUGUI m_HostPlayAgainText;
    public TextMeshProUGUI m_MessageText;
    private WaitForSeconds m_StartWait;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    private WaitForSeconds m_EndWait;
    private int m_PlayerCount = 0;
    private int m_RoundNumber;
    private bool m_RoundActive = false;
    private MallardManager m_RoundWinner;
    // Madder functions that you may call
    // These functions should be conditionally called based on if this is inside a WebGL build, not the editor
    [DllImport("__Internal")]
    private static extern void MessageToPlayer(string userName, string message);
    [DllImport("__Internal")]
    private static extern void MessageToAllPlayers(string message);
    [DllImport("__Internal")]
    private static extern void Exit();
    [DllImport("__Internal")]
    private static extern void UpdateStats(string userName, string stats);

    void Start()
    {
        playerJoins = new PlayerJoin[0];

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        StartCoroutine(GameLoop());
    }

    void Update()
    {
        // Testing Madder functions
        // TODO: This code should be commented out or removed before submission

        // Test RoomCode
        // TODO: Any of the following code may be modified or deleted
        if (Input.GetKeyDown(KeyCode.R))
        {
            RoomCode("ABCD");
        }

        // Test PlayerJoined
        // TODO: Any of the following code may be modified or deleted
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerJoin playerJoin = new PlayerJoin();
            playerJoin.name = "Player " + playerJoins.Length;
            playerJoin.stats = new GameStats();
            string jsonPlayerJoin = JsonUtility.ToJson(playerJoin);
            PlayerJoined(jsonPlayerJoin);
        }

        // Test PlayerLeft
        // TODO: Any of the following code may be modified or deleted
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (playerJoins.Length == 0)
            {
                return;
            }
            PlayerLeft("Player 0");
        }

        // Test PlayerControllerState for Player 0
        // TODO: Any of the following code may be modified or deleted
        if (playerJoins.Length > 0)
        {
            Joystick joystick = new Joystick(0, 0);
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                joystick.y = 100;
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                joystick.y = -100;
            }
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                joystick.x = -100;
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                joystick.x = 100;
            }
            ControllerState controllerState = new ControllerState();
            controllerState.name = playerJoins[0].name;
            controllerState.joystick = joystick;
            controllerState.circle = false;
            controllerState.triangle = false;
            controllerState.plus = false;
            string jsonControllerState = JsonUtility.ToJson(controllerState);
            PlayerControllerState(jsonControllerState);
        }

        // Test HandleExit
        // TODO: Any of the following code may be modified or deleted
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleExit();
        }

        // Test the plus button
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleHostControls(false, true);
        }

    }

    // TODO: The following function may be modified or deleted
    void HandleExit()
    {
        // Remove all player names from canvas
        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }

        // Reset playerJoins array
        playerJoins = new PlayerJoin[0];
    }

    /*
    * Madder function: RoomCode
    * This function is called when the uniquely generated code is received from the server
    * You will typically use this code to display the room code on the screen
    */
    public void RoomCode(string roomCode)
    {
        // TODO: Any of the following code may be modified or deleted
        Debug.Log("Room Code: " + roomCode);

        // Display room code on canvas
        m_RoomCode = roomCode;
        m_RoomCodeText.text = roomCode;

    }

    /*
    * Madder function: PlayerJoined
    * This function is called when a new player joins the game
    * You will typically use this function to create a character instance for this player
    *   and keep track of the player's stats
    */

    public void PlayerJoined(string jsonPlayerJoin)
    {
        // Destructure jsonPlayerJoin
        PlayerJoin playerJoin = JsonUtility.FromJson<PlayerJoin>(jsonPlayerJoin);

        // TODO: Any of the following code may be modified or deleted

        // Initialize player stats if they are null or have missing fields
        if (playerJoin.stats == null)
        {
            playerJoin.stats = new GameStats();
        }
        if (playerJoin.stats.gamesPlayed == null)
        {
            playerJoin.stats.gamesPlayed = new Stat("Games Played", 0);
        }

        // Add player to playerJoins array
        PlayerJoin[] newPlayerJoins = new PlayerJoin[playerJoins.Length + 1];
        for (int i = 0; i < playerJoins.Length; i++)
        {
            newPlayerJoins[i] = playerJoins[i];
        }
        newPlayerJoins[playerJoins.Length] = playerJoin;
        playerJoins = newPlayerJoins;

        // Add mallard to mallard_list
        AddMallard(playerJoin);
        m_PlayerCount = newPlayerJoins.Length;

        // Add game played to player stats
        playerJoin.stats.addGamePlayed();
        // Update player stats on server
#if UNITY_WEBGL && !UNITY_EDITOR // Only call this function if this is a WebGL build
        string jsonStats = JsonUtility.ToJson(playerJoin.stats);
        UpdateStats(playerJoin.name, jsonStats);
#endif
    }

    /*
    * Madder function: PlayerLeft
    * This function is called when a player leaves the game
    * You will typically use this function to remove the character instance of this player
    */
    public void PlayerLeft(string playerName)
    {
        // TODO: Any of the following code may be modified or deleted
        // Remove mallard from mallard_list
        RemoveMallard(playerName);

        // Remove player from playerJoins array
        PlayerJoin[] newPlayerJoins = new PlayerJoin[playerJoins.Length - 1];
        int j = 0;
        for (int i = 0; i < playerJoins.Length; i++)
        {
            if (playerJoins[i].name != playerName)
            {
                newPlayerJoins[j] = playerJoins[i];
                j++;
            }
            else
            {
                // Exit game if first player (host) leaves
#if UNITY_WEBGL && !UNITY_EDITOR // Only call this function if this is a WebGL build
                if (i == 0)
                {
                    Exit();
                }
#endif
                HandleExit();
                return;
            }
        }
        playerJoins = newPlayerJoins;

        // Remove player name from canvas
        foreach (Transform child in canvas.transform)
        {
            if (child.GetComponent<NameScript>().GetName() == playerName)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /*
    * Madder function: PlayerControllerState
    * This function is called when the controller state of a player is updated
    * You will typically use this function to move the character instance of this player
    *   or perform any other action based on button activity
    */
    public void PlayerControllerState(string jsonControllerState)
    {
        // Destructure jsonControllerState
        ControllerState controllerState = JsonUtility.FromJson<ControllerState>(jsonControllerState);
        // TODO: Any of the following code may be modified or deleted

        // Move player based on joystick

        // Empty mallard to hold the matching mallard
        MallardManager matchingMallard = null;

        // Check the name of all the mallards for a matching name
        foreach (MallardManager mallard in mallard_list)
        {
            if (mallard.m_PlayerName == controllerState.name)
            {
                matchingMallard = mallard;
                break;
            }
        }

        if (matchingMallard == null)
        {
            // Print error message if no matching mallard is found
            Debug.LogError("No matching mallard found for player name: " + controllerState.name);
            return;
        }

        // Move the mallard
        float x = controllerState.joystick.x;
        float y = controllerState.joystick.y;
        float distance = Mathf.Sqrt(x * x + y * y);
        distance = distance / 100f;
        matchingMallard.m_Movement.m_MovementInputValue = distance;

        // Handle rotation
        if (distance > 0f)
        {
            float angleRadians = Mathf.Atan2(x, y);
            float angleDegrees = angleRadians * Mathf.Rad2Deg;
            matchingMallard.m_Movement.m_AngleInputValue = angleDegrees;
        }

        if (controllerState.name == mallard_list[0].m_PlayerName)
        {
            HandleHostControls(false, controllerState.plus);
        }

    }

    private void AddMallard(PlayerJoin playerJoin)
    {
        // Instantiate a new mallard
        GameObject newMallard = Instantiate(mallard_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        MallardManager newMallardManager = new MallardManager();
        newMallardManager.m_Instance = newMallard;
        newMallardManager.m_PlayerName = playerJoin.name;
        newMallardManager.Setup();
        MallardManager[] newMallardList = new MallardManager[mallard_list.Length + 1];
        for (int i = 0; i < mallard_list.Length; i++)
        {
            newMallardList[i] = mallard_list[i];
        }
        if (mallard_list.Length == 0)
        {
            newMallardManager.m_IsHost = true;
        }
        newMallardList[mallard_list.Length] = newMallardManager;
        mallard_list = newMallardList;

    }

    private void RemoveMallard(string playerName)
    {
        MallardManager[] newMallardList = new MallardManager[mallard_list.Length - 1];
        int j = 0;
        for (int i = 0; i < mallard_list.Length; i++)
        {
            if (mallard_list[i].m_PlayerName != playerName)
            {
                newMallardList[j] = mallard_list[i];
                j++;
            }
            else
            {
                Destroy(mallard_list[i].m_Instance);
            }
        }
        mallard_list = newMallardList;
    }

    private void HandleHostControls(bool minus, bool plus)
    {
        // handle minus
        if (minus == true && !handlingMinus && m_GameWinner != null)
        {
            handlingMinus = true;
            HandleExit();
        }
        else if (minus == false)
        {
            handlingMinus = false;
        }

        // handle plus
        if (plus == true && !handlingPlus && (m_GameWinner != null || m_GameStarted == false) && m_PlayerCount > 1)
        {
            handlingPlus = true;
            if (m_GameStarted == false)
            {
                HandleBegin();
            }
            else
            {
                PlayAgain();
            }
        }
        else if (plus == false)
        {
            handlingPlus = false;
        }
    }

    public void HandleBegin()
    {
        m_GameStarted = true;
    }

    private IEnumerator WaitingForPlayersLoop()
    {
        while (!m_GameStarted)
        {
            yield return null;
        }
    }

    private IEnumerator GameLoop()
    {
        if (m_GameStarted == false)
        {
            yield return StartCoroutine(WaitingForPlayersLoop());

            m_HostStartText.gameObject.SetActive(false);
            // m_RequiredPlayersText.gameObject.SetActive(false);
            // m_TitleImage.gameObject.SetActive(false);

            // SpawnAllPlayers();

            SetCameraTargets();
        }

        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameStarted)
        {
            if (m_GameWinner != null)
            {
                StartCoroutine(EndOfGameLoop());
            }
            else
            {
                StartCoroutine(GameLoop());
            }
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllPlayers();
        DisablePlayerControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        m_RoundActive = true;
        EnablePlayerControl();

        m_MessageText.text = string.Empty;

        while (!IsDuckCaught())
        {
            yield return null;
        }
    }

    private bool IsDuckCaught()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            if (mallard_list[i].m_Movement.m_Caught)
            {
                return true;
            }
        }
        return false;
    }

    private MallardManager GetRoundWinner()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            if (mallard_list[i].m_Instance.activeSelf)
                return mallard_list[i];
        }

        return null;
    }

    private IEnumerator RoundEnding()
    {
        m_RoundActive = false;
        DisablePlayerControl();

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        if (m_RoundWinner != null)
        {
            for (int i = 0; i < mallard_list.Length; i++)
            {
                if (mallard_list[i].m_PlayerName == m_RoundWinner.m_PlayerName)
                {
                    mallard_list[i].m_GameStats.addRoundWin();
                    UpdateStats(mallard_list[i].m_PlayerName, JsonUtility.ToJson(mallard_list[i].m_GameStats));
                    break;
                }
            }
        }
#endif

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    private MallardManager GetGameWinner()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            if (mallard_list[i].m_Wins == m_NumRoundsToWin)
            {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
                    mallard_list[i].m_GameStats.addGameWin();
                    UpdateStats(mallard_list[i].m_PlayerName, JsonUtility.ToJson(mallard_list[i].m_GameStats));
#endif
                return mallard_list[i];
            }
        }

        return null;
    }
    private string EndMessage()
    {
        if (m_GameStarted == false)
        {
            return string.Empty;
        }

        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_PlayerName + " WINS THE ROUND!";

        message += "\n\n";

        for (int i = 0; i < mallard_list.Length; i++)
        {
            message += mallard_list[i].m_PlayerName + ": " + mallard_list[i].m_Wins + " WINS";
            if (i < mallard_list.Length - 1)
            {
                if (i % 2 == 1)
                {
                    message += "\n";
                }
                else
                {
                    message += "   |   ";
                }
            }
        }

        if (m_GameWinner != null)
        {
            message = "";
            // m_WinnerText.text = m_GameWinner.m_PlayerName;
            // m_WinnerAnnouncementImage.gameObject.SetActive(true);
        }

        return message;
    }

    private IEnumerator EndOfGameLoop()
    {
        // m_WinnerText.text = string.Empty;
        // m_WinnerAnnouncementImage.gameObject.SetActive(false);
        // m_MessageText.text = string.Empty;
        // m_TitleImage.gameObject.SetActive(true);
        m_HostPlayAgainText.gameObject.SetActive(true);
        m_HostExitText.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
        }
    }

    private void SpawnAllPlayers()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            mallard_list[i].m_SpawnPoint = m_SpawnPoints[i].transform;
        }
    }
    private void ResetAllPlayers()
    {
        // randomize spawn points
        // for (int i = 0; i < m_SpawnPoints.Length; i++)
        // {
        //     int randomIndex = Random.Range(i, m_SpawnPoints.Length);
        //     GameObject temp = m_SpawnPoints[i];
        //     m_SpawnPoints[i] = m_SpawnPoints[randomIndex];
        //     m_SpawnPoints[randomIndex] = temp;
        // }
        for (int i = 0; i < mallard_list.Length; i++)
        {
            // assign new spawn point
            // mallard_list[i].m_SpawnPoint = m_SpawnPoints[i].transform;
            mallard_list[i].Reset();
        }
    }

    private void EnablePlayerControl()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            mallard_list[i].EnableControl();
        }
    }


    private void DisablePlayerControl()
    {
        for (int i = 0; i < mallard_list.Length; i++)
        {
            mallard_list[i].DisableControl();
        }
    }

    public void PlayAgain()
    {
        // m_HostPlayAgainText.gameObject.SetActive(false);
        // m_HostExitText.gameObject.SetActive(false);
        // m_TitleImage.gameObject.SetActive(false);
        // for (int i = 0; i < m_Tanks.Length; i++)
        // {
        //     m_Tanks[i].m_Wins = 0;
        //     #if UNITY_WEBGL == true && UNITY_EDITOR == false
        //         m_Tanks[i].m_GameStats.addGamePlayed();
        //         UpdateStats(m_Tanks[i].m_PlayerName, JsonUtility.ToJson(m_Tanks[i].m_GameStats));
        //     #endif
        // }
        // m_RoundNumber = 0;
        StartCoroutine(GameLoop());
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[mallard_list.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = mallard_list[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }
    /*
    * Madder class: Message
    * This class is used to serialize messages sent to controllers (both individual and all controllers)
    * The following Message names work with Madder controllers:
    *   - "vibrate": Vibrate the player's controller
    * The message parameter has no current use for Madder controllers
    */
    public class Message
    {
        public string name;
        public string message;
    }

    /*
    * Madder class: Stat
    * This class is used to store and update a stat of a player across sessions
    * The title is the name of the stat and is REQUIRED
    * The value is the value of the stat and is REQUIRED
    * You may create children classes of Stat to store more complex stats
    */
    [System.Serializable]
    public class Stat
    {
        public string title;
        public int value;

        public Stat(string initTitle, int initValue)
        {
            title = initTitle;
            value = initValue;
        }
    }
    // TODO: Add any additional children classes of Stat here

    /*
    * Madder class: GameStats
    * This class is used to store and update the stats of a player for your game across sessions
    * All fields must be of type Stat or a child class of Stat
    * No fields or methods are required and you can add any additional fields or methods
    */
    [System.Serializable]
    public class GameStats
    {
        // TODO: Add/Remove any fields of type Stat or a child class of Stat here
        public Stat gamesPlayed;
        public GameStats()
        {
            gamesPlayed = new Stat("Games Played", 0);
        }
        public void addGamePlayed()
        {
            gamesPlayed.value++;
        }
    }

    /*
    * Madder class: PlayerJoin
    * This class is used to serialize the data sent to the PlayerJoined function
    * This class should not be altered
    */
    public class PlayerJoin
    {
        public string name;
        public GameStats stats;
    }

    /*
    * Madder class: Joystick
    * This class is used to serialize the joystick data sent to the PlayerControllerState function
    * This class should not be altered for the Madder controller
    */
    [System.Serializable]
    public class Joystick
    {
        public int x;
        public int y;
        public Joystick(int initX, int initY)
        {
            x = initX;
            y = initY;
        }
    }

    /*
    * Madder class: ControllerState
    * This class is used to serialize the data sent to the PlayerControllerState function
    * This class should not be altered for the Madder controller
    */
    public class ControllerState
    {
        public string name;
        public Joystick joystick;
        public bool circle;
        public bool triangle;
        public bool plus;
    }
}
