using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace BasicTCPServer
{
	class MainClass
	{
		public static void Main ()
		{

			//setting up a server
			Console.WriteLine ("Starting echo server...");

			int port = 1234;   //choosing a port number higher than 1023

			IPAddress address = IPAddress.Parse ("172.20.10.2");  //you need to create an instance of the class IPAddress and parse your own IP address into this instance so you can use it in the TcpListener

			TcpListener listener = new TcpListener (address, port);

			listener.Start ();

			//putting server into a waiting state
			TcpClient client = listener.AcceptTcpClient ();//accepts request

			//setting up input and output streams
			//creating network stream object
			NetworkStream stream = client.GetStream ();//using GetStream method on client object.GetStream returns NetworkStream used to send and recieve data

			//streamwriter implements a textwriter for writing characters to a stream in ASCII encoding
			StreamWriter writer = new StreamWriter (stream, Encoding.ASCII) { AutoFlush = true }; //same as writer.AutoFlush=true; the streamwriter will flush its buffer to underlying stream after every call to streamwriter.write
			StreamReader reader = new StreamReader (stream, Encoding.ASCII);

			while (true) {
				string inputLine = "";
				int count = 0;
				while (inputLine != null) {
					inputLine = reader.ReadLine ();


					//  stuff to be send and recieved goes in here


					/*
					//printing how many A's and a's are in the input message from the client
					foreach (char letter in inputLine) {

						if ((letter == 'a') || (letter == 'A'))
							count++;
					}

					writer.WriteLine ("Your message: " + inputLine + " contains " + count + " a(s)"); //sending back to the client how many a's are in his message
					Console.WriteLine ("The input mesage: " + inputLine + " contains " + count + " a(s)"); //printing in the server how many a's are in the message received

					count = 0;
					*/

				}
				Console.WriteLine ("Server saw disconnect from client.");
			}
		}
	}
}

