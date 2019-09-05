using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkManager : MonoBehaviour
{

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

    //from SocketIO API
	public SocketIOComponent socket;

	public GameObject playerPrefab;
	
	public bool onLogged;

    // Start is called before the first frame update
    void Start()
    {
		// if don't exist an instance of this class
		if (instance == null) {


			//it doesn't destroy the object, if other scene be loaded
			DontDestroyOnLoad (this.gameObject);
			instance = this;// define the class as a static variable

			socket = GetComponent<SocketIOComponent> ();

			socket.On ("PONG", OnReceivedPong);
			socket.On ("JOIN_SUCCESS",OnJoinSuccess);
			Debug.Log("start mmo game");



		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}


    }

  
    //receive a "pong" message from nodeJS server
	public void OnReceivedPong(SocketIOEvent message)
	{
	    
		Dictionary<string,string> pack = message.data.ToDictionary ();

		Debug.Log ("message: " + pack ["message"] + "from nodeJS server!");
	}


	// send a "ping" message to NodeJS server
	public void SendPingToServer()
	{
	    //create a data struct like  <key,value> to storage data
		Dictionary<string,string > pack = new Dictionary<string, string> ();

		pack["message"] = "ping!!!";
		
		//send to nodeJS server
		socket.Emit ("PING", new JSONObject (pack));

	}

	

	/// <summary>
	/// Emits the player's name to server.
	/// </summary>
	public void EmitJoin()
	{
		//hash table <key, value>	
		Dictionary<string,string> data = new Dictionary<string,string> ();

		//player's name	
		data ["name"] = CanvasManager.instance.inputLogin.text;

		//sends name to the server through socket
		socket.Emit ("JOIN_ROOM", new JSONObject(data));


	}

	/// <summary>
	/// Joins the local player in game.
	/// </summary>
	void OnJoinSuccess(SocketIOEvent pack)
	{
	    Debug.Log("Login successful, joining game");
		
		// the local player now is logged
		onLogged = true;
		
		Dictionary<string,string> result = pack.data.ToDictionary ();

		PlayerManager newPlayer = Instantiate (playerPrefab,new Vector3(0,0,0),Quaternion.identity).GetComponent<PlayerManager>();

		Debug.Log("player instantiated");

		newPlayer.gameObject.name = result ["id"];// id arived from nodeJS server

		newPlayer.gameObject.GetComponentInChildren<TextMesh> ().text = result ["name"];

		CanvasManager.instance.OpenScreen (1);//just to hide the lobby panel

		Debug.Log("local player in game");


	}

}
