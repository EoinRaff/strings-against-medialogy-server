using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class MultithreadTCPServer
{

	public static string serverIP = "127.0.0.1";
	static TcpListener tcpListener = new TcpListener(IPAddress.Parse(serverIP), 1234);


	static public List<string> answerDeck = new List<string>();
	static public List<string> playerHand = new List<string>();
	static public List<string> questionsDeck = new List<string>();
	static public List<string> usernames = new List<string> (); //list used to store usernames typed in by the clients
	static int numberOfPlayers = 0;
	static bool enoughPlayers = false;



	//vælg hvem der er judge 
	//send til hver player om de judge eller spiller
	//

	public static void Main()
	{


		string[] answers = new String[10];
		string[] questions = new String[4];


		// The file directory should be change, when on a new computer!!!!!!!!!!!!!!!!!!!
		StreamReader awnFile = new StreamReader("answers.txt");
		StreamReader questFile = new StreamReader("questions.txt");


		for (int i = 0; i < questions.Length; i++)
		{
			//Takes the questions from the txt file (in date folder) and load it into a array. That array is then loaded into a list.
			questions[i] = questFile.ReadLine();
			questionsDeck.Add(questions[i]);
		}



		for (int i = 0; i < answers.Length; i++)
		{
			// Takes the awnsers from the file (in the data folder) and load it into a array. That array is then loaded into a list.
			answers[i] = awnFile.ReadLine();
			answerDeck.Add(answers[i]);
		}


		awnFile.Close();
		questFile.Close();



		tcpListener.Start();
		Console.WriteLine("How many clients are going to connect to this server?:");
		int numberOfClientsYouNeedToConnect = int.Parse(Console.ReadLine());
		Console.WriteLine ("Waiting for clients..");
		for (int i = 0; i < numberOfClientsYouNeedToConnect; i++)
		{
			Thread newThread = new Thread(new ThreadStart(Listeners)); // Create new thread for each client
			newThread.Start();
		}

		// Here the main calls the ask question method
		askQuestion(questionsDeck);

		//Here the playerHand is set equal to the list returned from the method
		playerHand = dealDeack(answerDeck);

		foreach (string word in playerHand)
		{
			Console.WriteLine(word);
		}

			
	} // Main

	static void Listeners()
	{


		Socket ClientSocket = tcpListener.AcceptSocket();
		if (ClientSocket.Connected)
		{
			numberOfPlayers++;
			NetworkStream networkStream = new NetworkStream(ClientSocket);
			StreamWriter streamWriter = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };
			StreamReader streamReader = new StreamReader(networkStream, Encoding.ASCII);

			//the first input line from the client is their username, this is being stored in a string "username",
			//the username is then added to the list "usernames" 
			//this is done for all the clients connected to the server, so the list "usernames" is filled up with the usernames of the clients
			string inputline = streamReader.ReadLine ();
			string username = inputline;
			Console.WriteLine ("Client:" + username + " now connected to server.");
			usernames.Add (username);

			//the first username in the usernames-list is the judge to begin with
			string judge = usernames [0];

			while (true)
			{
				string inputLine = streamReader.ReadLine();

				// Information back and forward between client and server goes here

				while (enoughPlayers == false){


					streamWriter.WriteLine ("Waiting for " + (int.Parse("3") - numberOfPlayers) + " more player(s) to join..");
				
					if (numberOfPlayers == 2) {
						enoughPlayers = true;

						//this prints out all the usernames in the usernames-list when the correct number of players have joined
						//it does however only print when all players have jointed and one have written a message
						//dont know why??
							Console.WriteLine ("The following players are now ready to play: ");
							foreach (string names in usernames) {
								Console.WriteLine (names);
							}
						//writes the name of the judge in the console
							Console.WriteLine ("the following player is the judge: " + judge);


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

	//This method deals the card to the player and returns the via a string. 
	static List <string> dealDeack(List <string> _awnserDeck)
	{
		List<string> playerDeck = new List<string>();

		Random random = new Random();
		int ranValue;

		for (int i = 0; i <= 6; i++)
		{
			//this is what "shuffles the card"
			ranValue = random.Next(0, _awnserDeck.Count);

			//Here it is added to the players hand.
			playerDeck.Add (_awnserDeck [ranValue]);

			//Here it delete it from the "main" answerDeck which holds all the possible answer 
			answerDeck.RemoveAt(ranValue);
		}

		return playerDeck;
	}

	//In this method a random question is found, and send to the console
	static void askQuestion(List <string> _questions)
	{
		Random random = new Random();

		//this is what "shuffles the card"
		int ranVaulue = random.Next(0, _questions.Count);

		//This prints the question 
		System.Console.WriteLine(_questions[ranVaulue]);

		//Here the question is then removed from the main question holder
		questionsDeck.RemoveAt(ranVaulue);
	}



} // Class