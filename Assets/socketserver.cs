using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
//using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using System;
using UnityEngine.UI;

public class socketserver : MonoBehaviour {
	
	public int lives = 0;
	public float speed = 0;
	private bool holding = false; 
	public Vector3 playerpos;
	private int completed = 0;

	public Text positiontext_x;
	public Text positiontext_y;
	public Text positiontext_z;
	public Text lifetext;
	public Text holdingtext;
	public Text completedtext;

	/*public class MyMsgType {
        public static short Player = MsgType.Highest + 1;
    };
    public class MyMsgType2 {
        public static short Player = MsgType.Highest + 2;
    };*/
	
	
	void OnGUI()
	{
		string ipaddress = Network.player.ipAddress;
		GUI.Box (new Rect(10,Screen.height -50, 100, 50),ipaddress);
		GUI.Label (new Rect (20,Screen.height - 35, 100, 20),"Status:" +NetworkServer.active);
		GUI.Label (new Rect (20,Screen.height - 20, 100, 20),"Connected:" +NetworkServer.connections.Count);

		GUI.Box (new Rect(10,0, 100, 50),"Connected:");
		for(int i=0; i < NetworkServer.connections.Count; i++)
			GUI.Label (new Rect(20,0, 100, 50),"P: " +NetworkServer.connections[i]);

	}
	
	public class PlayerMessage : MessageBase
	{
		public Vector3 position;
		public int lives;
		public bool holding;
		public int completed;

		public Vector3 forward;
		public Vector3 right;
		public float h;
		public float v;
	}

	public class CubeMessage : MessageBase
	{
		public int numcube;
		public Vector3 position;
	}

	public class EnemyMessage : MessageBase
	{
		public Vector3 position;
		public string name;
	}

	public class PlayerUpdateMessage : MessageBase
	{
		public Vector3 position;
		public float speed;
		public int lives;
		public float remain;
	}

	public class PlayerTypeMessage : MessageBase
	{
		public bool type;
	}
	
	// Use this for initialization
	void Start () {

		NetworkServer.Listen (25000);
		NetworkServer.RegisterHandler (888, ServerReceiveMessage);
		//NetworkServer.RegisterHandler (MsgType.Connect, ServerReceiveUpdate); //enviar estado actual del jugador
		NetworkServer.RegisterHandler (MsgType.Animation, ServerReceiveMessageCube);

		NetworkServer.RegisterHandler (MsgType.Command, ServerReceiveMessageEnemy);
	}

	private void ServerReceiveMessage(NetworkMessage message)
	{
		PlayerMessage msg = message.ReadMessage<PlayerMessage>();
		playerpos = msg.position;
		lives = msg.lives;
		holding = msg.holding;
		completed = msg.completed;
	
		NetworkServer.SendToAll(888, msg);
	}

	private void ServerReceiveMessageEnemy(NetworkMessage message)
	{
		EnemyMessage msg = message.ReadMessage<EnemyMessage>();
	
		NetworkServer.SendToAll(MsgType.Command, msg);
	}

	private void ServerReceiveMessageCube(NetworkMessage message)
	{
		CubeMessage msg = message.ReadMessage<CubeMessage>();
		NetworkServer.SendToAll(MsgType.Animation, msg);
	}


	private void ServerReceiveUpdate(NetworkMessage message) //cuando se conecta algun jugador
	{
		//PlayerTypeMessage msg = message.ReadMessage<PlayerTypeMessage>();

		PlayerUpdateMessage msg = new PlayerUpdateMessage ();
		msg.position = playerpos;
		msg.speed = speed;
		msg.lives = lives;

		//NetworkServer.SendToAll(MsgType.Connect, msg);
	}

	// Update is called once per frame
	void Update () {
		positiontext_x.text = "X: "+playerpos.x;
		positiontext_y.text = "Y: "+playerpos.y;
		positiontext_z.text = "Z: "+playerpos.z;

		lifetext.text = "VIDAS: "+lives;

		if(holding == true)
			holdingtext.text = "SOSTIENE: SI";
		else
			holdingtext.text = "SOSTIENE: NO";

		completedtext.text = "COMPLETOS: "+completed;

		/*if (Input.GetButton ("Fire1")) {
			EnemyMessage msg = new EnemyMessage();
			//msg.pos = go.transform.position;
			NetworkServer.SendToAll(MyMsgType2.Player, msg);
		}*/
	}
}
