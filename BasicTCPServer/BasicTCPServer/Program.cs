using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;


public class MultithreadTCPServer
{

	public static string serverIP = "127.0.0.1";
	static TcpListener tcpListener = new TcpListener(IPAddress.Parse(serverIP), 1234);

	static public List<string> answerDeck = new List<string>();
	static public List<string> playerHand = new List<string>();
	static public List<string> questionsDeck = new List<string>();
	static public List<string> questionsForJudge = new List<string> ();

	static int numberOfPlayers = 0;
	static bool enoughPlayers = false;
	static bool allAnswersCollected = false;
	static string questionAsked;


	public static void Main()
	{
		string[] answers = new String[10];
		string[] questions = new String[4];

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
		Console.WriteLine("Server started");
		Console.WriteLine ("Waiting for clients..");
		for (int i = 0; i < 2; i++)
		{
			Thread newThread = new Thread(new ThreadStart(Listeners)); // Create new thread for each client
			newThread.Start();
		}

		questionAsked = askQuestion(questionsDeck);

		if (questionsForJudge.Count >= 2){
			string stringForJudge = string.Join (String.Empty, questionsForJudge.ToArray ());
			allAnswersCollected = true;
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

		//------------------------------------------ Waits for enough players to join
//				while (enoughPlayers == false) {
//					streamWriter.WriteLine ("Waiting for " + (int.Parse("3") - numberOfPlayers) + " more player(s) to join..");
//
//					if (numberOfPlayers == 3) {
//						enoughPlayers = true;
//					}
//				}

				// If a certain input is recieved from client a hand is dealt 
				if (inputLine == "p") 
				{
					// Writes the questions found in main to clients
					streamWriter.WriteLine(questionAsked);

					//Here the playerHand is set equal to the list returned from the method
					playerHand = dealDeack(answerDeck);

					// Creates one string to send to client instead of list
					string stringToSend = string.Join (String.Empty, playerHand.ToArray ());

					streamWriter.WriteLine(stringToSend);

				}
				if (inputLine == "1" || inputLine == "2" || inputLine == "3" || inputLine == "4" || inputLine == "5") {
					Console.WriteLine (playerHand [int.Parse (inputLine) - 1]);
					questionsForJudge.Add (playerHand [int.Parse (inputLine) - 1]);
				}

				if (allAnswersCollected == true) { // Add so it only sends to judge
					Console.WriteLine (questionsForJudge);
					streamWriter.WriteLine (questionsForJudge);
				}

				Console.WriteLine("Message recieved by client:" + inputLine);


				// Information back and forward between client and server goes here

			

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

		for (int i = 0; i <= 4; i++)
		{
			//this is what "shuffles the card"
			ranValue = random.Next(0, _awnserDeck.Count);

			//Here it is added to the players hand.
			playerDeck.Add (_awnserDeck [ranValue]);

			//Here it delete it from the "main" answerDeck which holds all the possible answer 
			answerDeck.RemoveAt(ranValue);
		}

		return playerDeck;
	} // dealDeack


	//In this method a random question is found, and send to the console
	static string askQuestion(List <string> _questions)
	{
		Random random = new Random();

		//this is what "shuffles the card"
		int ranVaulue = random.Next(0, _questions.Count);

		//Here the question is then removed from the main question holder
		questionsDeck.RemoveAt(ranVaulue);

		// The chosen question is returned
		return _questions [ranVaulue];
	} // askQuestion
		
} // Class