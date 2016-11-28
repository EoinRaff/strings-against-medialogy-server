using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

public class MultithreadTCPServer
{

	public static string serverIP = "127.0.0.1";
	static TcpListener tcpListener = new TcpListener(IPAddress.Parse(serverIP), 1234);

	static int numberOfPlayers = 0;
	static bool enoughPlayers = false;

	public static void Main()
	{
		tcpListener.Start();
		Console.WriteLine("How many clients are going to connect to this server?:");
		int numberOfClientsYouNeedToConnect = int.Parse(Console.ReadLine());
		Console.WriteLine ("Waiting for clients..");
		for (int i = 0; i < numberOfClientsYouNeedToConnect; i++)
		{
			Thread newThread = new Thread(new ThreadStart(Listeners)); // Create new thread for each client
			newThread.Start();
		}
	} // Main

	static void Listeners()
	{

		Socket ClientSocket = tcpListener.AcceptSocket();
		if (ClientSocket.Connected)
		{
			Console.WriteLine("Client:" + ClientSocket.RemoteEndPoint + " now connected to server.");
			numberOfPlayers++;
			NetworkStream networkStream = new NetworkStream(ClientSocket);
			StreamWriter streamWriter = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };
			StreamReader streamReader = new StreamReader(networkStream, Encoding.ASCII);

			while (true)
			{
				string inputLine = streamReader.ReadLine();

				// Information back and forward between client and server goes here

				while (enoughPlayers == false){
					streamWriter.WriteLine ("Waiting for " + (int.Parse("3") - numberOfPlayers) + " more player(s) to join..");
				
					if (numberOfPlayers == 3) {
						enoughPlayers = true;
					}
				}

				if (inputLine == "exit")
					break;
			}

			streamReader.Close();
			networkStream.Close();
			streamWriter.Close();
		}

		ClientSocket.Close();
		Console.WriteLine("Client(s) disconnected! : Press any key to exit program");
		Console.ReadKey();

	} // Listener
} // Class