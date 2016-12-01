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

	public static string serverIP = "192.168.43.134";
	static TcpListener tcpListener = new TcpListener(IPAddress.Parse(serverIP), 1234);

	static public List<string> answerDeck = new List<string>();
	static public List<string> playerHand = new List<string>();
	static public List<string> questionsDeck = new List<string>();
	static public List<Player> players = new List<Player> ();
	static public List<string> usernames = new List<string> (); //list used to store usernames typed in by the clients


	// things that need to be reset after a new game is started 

	static public List <string> listOfAnswers = new List<string> ();
	static public List <string> listOfChosenAnswer = new List<string> ();
	static public List <string> responsFromJudge = new List<string> ();

	static int numOfPlayerThatWantToRestart;

	//

	static public string[] standardPlayerRoles = new string[3]{"Judge","Player","Player"}; 
	static public string[] distributedPlayerRoles = new string[3]; 

	static int numberOfPlayers = 0;
	static int numberOfThreads = 0;
	static bool enoughPlayers = false;
	static bool enoughAnswers = false;
	static bool judgeReady = false;
	static string questionAsked;
	static string winner = "";

	static int TimeToSwichtJugde;





	//vælg hvem der er judge 
	//send til hver player om de judge eller spiller
	//

	public static void Main()
	{

		string[] answers = new String[1000];
		string[] questions = new String[200];


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
		Console.WriteLine("Server started");
		Console.WriteLine ("Waiting for clients..");

		for (int i = 0; i <= 2; i++)
		{
			Thread newThread = new Thread(new ThreadStart(Listeners)); // Create new thread for each client
			newThread.Start();
		}

		for (int i = 0; i <= 2; i++) {
			distributedPlayerRoles [i] = standardPlayerRoles [i];
	
//			streamWriter.WriteLine (playerrole);
		}
			
		questionAsked = askQuestion(questionsDeck);

	} // Main

//------------------------------------------ A new thread is made for each client -------------------------------------------\\
	static void Listeners()
	{
		
		Socket ClientSocket = tcpListener.AcceptSocket();
		bool answersRecieved = false;
		int playerNumber;
		int playerScore = 0;

		if (ClientSocket.Connected)
		{
			numberOfPlayers++;
			playerNumber = numberOfPlayers;
			NetworkStream networkStream = new NetworkStream(ClientSocket);
			StreamWriter streamWriter = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };
			StreamReader streamReader = new StreamReader(networkStream, Encoding.ASCII);

			//the first input line from the client is their username, this is being stored in a string "username",
			//the username is then added to the list "usernames" 
			//this is done for all the clients connected to the server, so the list "usernames" is filled up with the usernames of the clients



			string inputline = streamReader.ReadLine (); // Recieve username from client
			string username = inputline;
			players.Add (new Player (username));
			Console.WriteLine ("Client:" + username + playerNumber + " now connected to server.");
			usernames.Add (username);



			while (!enoughPlayers){
				
				streamWriter.WriteLine ("Waiting for " + (int.Parse("3") - numberOfPlayers) + " more player(s) to join..");

				if (numberOfPlayers >= 3) {
					enoughPlayers = true;

				}
			}

			streamWriter.WriteLine ("Ready!");

		
				
			while (true)
			{
				restartGame ();
				answersRecieved = false;
				Console.WriteLine ("At the start ");

				// Waiting for response from client
				string inputLine = streamReader.ReadLine();

				Console.WriteLine ("Have read the line ");


				//the first username in the usernames-list is the judge to begin with
//				string judge = usernames [0];

				//this prints out all the usernames in the usernames-list when the correct number of players have joined
				//it does however only print when all players have jointed and one have written a message
				//dont know why??
//				Console.WriteLine ("The following players are now ready to play: ");
//				foreach (string names in usernames) {
//					Console.WriteLine (names);
//				}

	
				// If a certain input is recieved from client a hand is dealt 
				if (inputLine == "p" && enoughPlayers == true) 
				{

//					if (streamReader.ReadLine () == "1") {
//
//						for (int i = 0; i < 1; i++)
//						{
////							// Takes the awnsers from the file (in the data folder) and load it into a array. That array is then loaded into a list.
////							answers[i] = awnFile.ReadLine();
////							answerDeck.Add(answers[i]);
//
//							Console.WriteLine ("He want another card");
//
//						}
//
//					} else {
//						Console.WriteLine ("he does not want another card");
//
//					}
//
		
//  ------------------------Here there should be a something that make the judge role swicht when the players comes to this point-------------------------

					streamWriter.WriteLine (usernames [0]);

//  ------------------------Here there should be a something that make a new question being send when the players comes to this point -------------------------

					// Writes the questions found in main to clients
					streamWriter.WriteLine(questionAsked);

					int cardsNedded = int.Parse(streamReader.ReadLine ());
					Console.WriteLine (cardsNedded);

					//Here the playerHand is set equal to the list returned from the method
					playerHand = dealDeack(answerDeck,cardsNedded);

					// Creates one string to send to client instead of list
					string handToSend = string.Join (String.Empty, playerHand.ToArray ());

					streamWriter.WriteLine(handToSend);
				}

//				if (inputLine == "1" || inputline == "2" || inputline == "3" || inputline == "4" || inputline == "5") {
//					Console.WriteLine (playerHand[int.Parse(inputline)]);
//				}


				// Writes the 
//				if (inputLine != "p" && inputLine != "P") {
//					Console.WriteLine("Answer recieved by: " + username + ": " + inputLine);
//					while (listOfAnswers.Count < numberOfPlayers - 1){
//						listOfAnswers.Add (inputLine);
//						Console.WriteLine (inputLine);
//						Console.WriteLine ("Waiting.. count = {0} number of players -1 = {1}", listOfAnswers.Count, numberOfPlayers -1);
//
//						streamWriter.WriteLine ("Waiting..");
//
//					}
//					Console.WriteLine ("Server Ready! all answers submitted");
//					streamWriter.WriteLine ("Ready!");
//
//				}

				while (!answersRecieved) {
					string tempAnswer = streamReader.ReadLine ();
					if (tempAnswer != null) {
						answersRecieved = true;
						Console.WriteLine ("TempAnswer: " + tempAnswer);
						listOfAnswers.Add (tempAnswer + ".");
					}
				}

				while (!enoughAnswers){
					streamWriter.WriteLine ("Waiting..");
					if (listOfAnswers.Count == numberOfPlayers) {
						enoughAnswers = true;
					}
				}
					
				Console.WriteLine ("Recieved enough answers!");
				streamWriter.WriteLine ("Ready!");

				listOfAnswers.Remove ("Judge Reply."); // Remove the answer recieved from the judge (Will always be "Judge Reply")
				string answersSendJudge = string.Join (String.Empty, listOfAnswers.ToArray ()); // Append the two replies
				streamWriter.WriteLine(answersSendJudge); // Send the two replies to the judge

				answersRecieved = false;

			
				

				while (!answersRecieved) {
					string tempAnswer = streamReader.ReadLine ();
					if (tempAnswer != null) {
						answersRecieved = true;
						responsFromJudge.Add (tempAnswer + ".");
					}
				}

				while (!judgeReady){
					streamWriter.WriteLine ("Waiting..");
					if (responsFromJudge.Count == numberOfPlayers) {
						judgeReady = true;
					}
				}
//////////// get winner from list of answers 


					
				
					Console.WriteLine ("Recieved enough answers!" + playerNumber);
					streamWriter.WriteLine ("Ready!");
//
//					for (int i = 0; i < responsFromJudge.Count; i++) {
//						Console.WriteLine ("Before: " + responsFromJudge [i]);
//					}
//
//					for (int i = 0; i < responsFromJudge.Count; i++) {
//						if (responsFromJudge [i] == "waiting") {
//							responsFromJudge.Remove ("waiting");
//						}
//					}
//
//
//					for (int i = 0; i < responsFromJudge.Count; i++) {
//						Console.WriteLine ("After: " + responsFromJudge [i]);
//					}

				    winner = responsFromJudge [responsFromJudge.Count-1];
					Console.WriteLine ("The winning response is: " + winner);


//				string winnerAnswer = streamReader.ReadLine (); // Recieve the chosen answer from the judge
//				listOfChosenAnswer.Add(winnerAnswer);
//				for (int i = 0; i < listOfChosenAnswer.Count; i++) {
//					if (listOfChosenAnswer [i] == "waiting") {
//						Console.WriteLine (listOfChosenAnswer [i]);
//						// streamWriter.WriteLine (listOfChosenAnswer[i]);
//						// Console.WriteLine ("The winner is " + listOfChosenAnswer [i]);
//						// listOfChosenAnswer.RemoveAt(i);
//					} else {
//						streamWriter.WriteLine ("Ready!");
//						winner = listOfChosenAnswer[i];
//						Console.WriteLine (winner + " won the round");
//					}
//				}
				streamWriter.WriteLine (winner + " won the round, Congratulations!, well done mate!, good job, nice!");

				Console.WriteLine (streamReader.ReadLine());
				numOfPlayerThatWantToRestart++; 
				//Console.WriteLine ("value of num of players that want to continue: " + numOfPlayerThatWantToRestart);

				if (numOfPlayerThatWantToRestart == numberOfPlayers) {
					Console.WriteLine ("ONE MORE TIME");
					Console.Clear ();
				}


				// Console.WriteLine (winnerAnswer);
				// streamWriter.WriteLine ("The winner is " + winnerAnswer); // Write the chosen answer to the players
			
//				for (int i = 0; i < playerHand.Count; i++) { // Add score to the winner player
//					if (winnerAnswer == playerHand[i]) {
//						playerScore++;
//					}
//				}
//					
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
	static List <string> dealDeack(List <string> _awnserDeck,int needCards)
	{
		List<string> playerDeck = new List<string>();

		Random random = new Random();
		int ranValue;

		for (int i = 0; i < needCards; i++)
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
		

	static public void restartGame () {



		listOfAnswers = new List<string> ();
		listOfChosenAnswer = new List<string> ();
		responsFromJudge = new List<string> ();

		enoughAnswers = false;
		judgeReady = false;


		numOfPlayerThatWantToRestart = 0;


	}







} // Main class

public class Player
{
	public string name;
	public int score;
	public bool isJudge;
	public List<string> playerHand = new List<string>();


	public Player (string userName)
	{
		name = userName;
		score = 0;
		isJudge = false;
	}



}
