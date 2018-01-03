using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using System; 
using System.Net.Sockets; 
using System.Net; 
using System.Linq; 
using System.Text;
using UnityEngine.UI;

public class UpdHandler : MonoBehaviour {

	public GameObject ui;
	public GameObject waiting;


	public InputField input;
	public InputField ip;
	public Text output;
	public static string inputMessage;
	static string mirrorIp;
	public static bool messageReceived;


	string myName = "Unity";
	string myIP;
	string myGameName = "TrialMirror";
	UdpClient sender;
	int remotePort = 19784;
	int localPort = 8080;
	void Start()
	{
		myIP = Network.player.ipAddress;
		StartReceivingIP ();

		//		//SendData ();
		//		InvokeRepeating("SendData",0,5f);
	}

	void SendData ()
	{
		string customMessage = myName+" * "+myIP+" * "+myGameName;
		string newmessage = "This is a connection test";
		if (customMessage != "") {
			//			sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
			sender.Send (Encoding.ASCII.GetBytes (newmessage), newmessage.Length);
		}
	}

	public void SendCustomData()
	{
		string newmessage = input.text;
		sender.Send (Encoding.ASCII.GetBytes (newmessage), newmessage.Length);
	}

	UdpClient receiver;

	public void StartReceivingIP ()
	{
		try 
		{
			if (receiver == null) 
			{
				receiver = new UdpClient (remotePort);
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
		}
		catch (SocketException e)
		{
			Debug.Log (e.Message);
		}
	}

	private void ReceiveData (IAsyncResult result)
	{
		IPEndPoint receiveIPGroup = new IPEndPoint (IPAddress.Any, remotePort);
		byte[] received;
		if (receiver != null) 
		{
			received = receiver.EndReceive (result, ref receiveIPGroup);
		} 
		else 
		{
			return;
		}
		receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
		string receivedString = Encoding.ASCII.GetString (received);
		Debug.Log ("receivedString " + receivedString);
		inputMessage = receivedString;
		messageReceived = true;
	}

	void Update()
	{
		if (messageReceived) 
		{
			MessageReceived ();
		}
	}

	void MessageReceived()
	{
		messageReceived = false;
		if (inputMessage.Contains (".")) 
		{
			mirrorIp = inputMessage;
			ConnectToMirror();
		}
	}


	public void OnClick(Button btn)
	{
		string newmessage = "Try :" + btn.name;
		sender.Send (Encoding.ASCII.GetBytes (newmessage), newmessage.Length);
		Debug.Log ("Message : " + newmessage);
	}

	public void ConnectToMirror()
	{

		waiting.SetActive (false);
		ui.SetActive (true);

		sender = new UdpClient (localPort, AddressFamily.InterNetwork);
		IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, remotePort);
		sender.Connect(IPAddress.Parse(mirrorIp),remotePort);
		string newmessage = "Connection Request";
		sender.Send (Encoding.ASCII.GetBytes (newmessage), newmessage.Length);
	}
}
