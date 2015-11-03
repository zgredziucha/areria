using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { get; private set; }
	public CheckPoint currentCheckPoint; 

	public PlayerController Player { get; private set; }
    public CharactersController Characters { get; private set; }
	public EnergyManager Energy { get; private set; }
	public CameraFollow Camera { get; private set; }
	
	public delegate void KillEvent ();
	public static event KillEvent onPlayerDied;
	public static event KillEvent onPlayerAlive;

	public delegate void TeleportEvent (bool isTeleporting);
	public static event TeleportEvent onPlayerTeleport;

	public int levelIndex = 3;

	//helpers
	public static int OBSTACLE_LAYER = 12;
	public static int PLAYER_LAYER = 11;
	public static int PLATFORM_LAYER = 8;

    public bool isSwitchCharacterPossible;
    public CharacterName startCharacter;


	public void Awake ()
	{
		PLAYER_LAYER = LayerMask.NameToLayer("Player");
		OBSTACLE_LAYER = LayerMask.NameToLayer("Obstacle");
		PLATFORM_LAYER = LayerMask.NameToLayer("Platforms");
		Instance = this;
	}

	public void Start ()
	{
		Camera = FindObjectOfType<CameraFollow> ();
		Player = FindObjectOfType<PlayerController> ();
        Characters = FindObjectOfType<CharactersController>();
		Energy = FindObjectOfType<EnergyManager> ();

		if (currentCheckPoint != null)
		{
			currentCheckPoint.SpawnPlayer(Player);
		}
		EnergyManager.onEnergyChanged += OnEnergyChange;

        Characters.SetCharacterByName(startCharacter);
        Characters.SetSwitchPossibility(isSwitchCharacterPossible);
	}

	public void SetCurrentPoint (CheckPoint checkPoint) 
	{
		currentCheckPoint = checkPoint;
	}

	private void OnEnergyChange (float energy)
	{
		if (energy <= 0 && !Player.IsRespawning) 
		{
			KillPlayer();
		}
	}

	public void TakeDamage(float damage)
	{
		Energy.SubstractEnergy (damage);
	}

	public void AddEnergy(float energy)
	{
		Energy.AddEnergy(energy);
	}

	public void KillPlayer () 
	{
		Player.TurnOff ();
        Player.Deaths++;
		if (onPlayerDied != null)
		{
			onPlayerDied();

		}
		StartCoroutine ("KillPlayerCoroutine");
	}

	private IEnumerator KillPlayerCoroutine () 
	{
		Camera.IsFollowing = false;
		yield return new WaitForSeconds(2f);

		currentCheckPoint.SpawnPlayer (Player);
		Energy.SetMax ();
        Energy.ResetPoints();

		if (onPlayerAlive != null)
		{
			onPlayerAlive();
		}
		
		Camera.IsFollowing = true;
		Player.TurnOn();
	}

	public void TeleportPlayer (Portal portal)
	{
		Player.TurnOff ();
		if (onPlayerTeleport!= null)
		{
			onPlayerTeleport(false);
		}

//		_portal = portal;
//		timerStart = Time.time;
		Camera.IsFollowing = false;
		StartCoroutine (TeleportPlayerCoroutine(portal));
	}

	private IEnumerator TeleportPlayerCoroutine (Portal portal) 
	{
		//Camera.IsFollowing = false;
		yield return new WaitForSeconds(2f);

		portal.SpawnPlayer (Player);		
		Camera.IsFollowing = true;
		Player.TurnOn();
		if (onPlayerTeleport != null)
		{
			onPlayerTeleport(true);
		}
	}

	public void GotoNextLevel ()
	{
		var nextLevelIndex = GameManager.GetNextLevelIndex (levelIndex);
		StartCoroutine (GotoNextLevelCo (nextLevelIndex));
	}

	private IEnumerator GotoNextLevelCo (int levelIndex)
	{
        Camera.IsFollowing = false;
        var text = string.Format("OU WON !\n  Energy: {0} / {1}, \n Points: {2}, \n Collectibles: {3} / {4}, \n Deaths: {5}", Energy.Energy, Energy.maxEnergyVolume, (int)Energy.Points, Player.Collectibles, Collectible.Count, Player.Deaths);
        GUI_Events.DisplayText(text);
		Player.FinishLevel ();
		yield return new WaitForSeconds(2f);

		if ( levelIndex < 0)
		{
			Application.LoadLevel(GameManager.ChoosePanelIndex);
		}
		else 
		{
			Application.LoadLevel(levelIndex);
		}
	}

}
