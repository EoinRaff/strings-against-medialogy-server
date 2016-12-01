using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace BasicTCPServer
{
	public class Player
	{
		string name;
		int score;
		string role;
		public List<string> playerHand = new List<string>();


		public Player (string userName)
		{
			name = userName;
			score = 0;
		}



	}
}

