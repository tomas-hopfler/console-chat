using ConsoleNetChat;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ConsoleChat
{
	public static class SavedData
	{
		public static string IP = "---.---.---.---";
		public static string PUBLIC_KEY = "";
		public static string LAST_RECEIVED_PUBLIC_KEY = "";
	}	 

	public class Program
	{
		static UdpClient udpClient;
		const int listenPort = 11000;

		static async Task Main(string[] args)
		{
			udpClient = new UdpClient(listenPort);
			Task receiver = ReceiveMessages();

			RSAEncryption.GenerateKeys();
			SavedData.PUBLIC_KEY = RSAEncryption.GetPublicKey();

			RedrawGUI();

			//MenuGetIP(out SavedData.IP);

			TicTacToe.MakeField();

			while (true)
			{
				string message = Console.ReadLine();

				if (message.ToLower() == "/x")
				{
					try
					{
						MenuChangeIP(out SavedData.IP);
					}
					catch (Exception ex){}
				}
				else if (message.ToLower() == "/confirm")
				{
					SavedData.PUBLIC_KEY = SavedData.LAST_RECEIVED_PUBLIC_KEY;
					RedrawGUI();
				}
				else if (message.ToLower() == "/c")
				{
					MenuClear();
				}
				else if (message.ToLower() == "/r")
				{
					SavedData.PUBLIC_KEY = RSAEncryption.GetPublicKey();
					RedrawGUI();
				}
				else if (message.ToLower() == "/t")
				{
					TicTacToe.view = !TicTacToe.view;
					RedrawGUI();
				}
				else if (message.ToLower() == "/request")
				{
					try
					{
						byte[] bytes = Encoding.UTF8.GetBytes(RSAEncryption.GetPublicKey());
						await udpClient.SendAsync(bytes, bytes.Length, SavedData.IP, listenPort);
					}
					catch (Exception e)
					{
						Print($" Error sending message: {e.Message}\n", ConsoleColor.Red);
					}
				}
				else if (message.ToLower() == "/q")
				{
					udpClient.Close();
					MenuQuit();
				}
				else
				{
					await SendMessage(SavedData.IP, message);
				}
			}
		}
		public static void RedrawGUI()
		{
			Console.Clear();
			Print(" _______________________________________\n", ConsoleColor.Green);
			Print(" Type '/q' to quit.\n", ConsoleColor.Cyan);
			Print(" Type '/x' to set destination IP.\n", ConsoleColor.Cyan);
			Print(" Type '/c' to clear chat history.\n", ConsoleColor.Cyan);
			Print(" Type '/r' to reset encryption key.\n", ConsoleColor.Cyan);
			Print(" Type '/t' to switch TicTacToe view.\n", ConsoleColor.Cyan);
			Print(" Type '/request' to send your public key.\n", ConsoleColor.Cyan);

			if(TicTacToe.view) // TTT
			{
				Print(" _______________________________________\n", ConsoleColor.Green);
				Print(" Type 'TTT x y' to play TicTacToe\n", ConsoleColor.Cyan);
				TicTacToe.DrawField();
			} // end TTT

			Print(" _______________________________________\n", ConsoleColor.Green);
			Print(" Recipient IP: " + SavedData.IP);
			Print("\n _______________________________________\n", ConsoleColor.Green);
		}
		static void MenuQuit()
		{
			Console.Clear();
			Environment.Exit(0);
		}
		static void MenuGetIP(out string ip)
		{
			RedrawGUI();

			Print("\n Enter destination IP: ", ConsoleColor.Cyan);
			ip = Console.ReadLine();

			RedrawGUI();
		}
		static void MenuChangeIP(out string ip)
		{
			RedrawGUI();

			Print("\n Enter new destination IP: ", ConsoleColor.Cyan);
			ip = Console.ReadLine();

			RedrawGUI();
		}
		static void MenuClear()
		{
			RedrawGUI();
		}
		static void Print(object message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor=color;
			Console.Write(message);
			Console.ResetColor();
		}
		static async Task ReceiveMessages()
		{
			while (true)
			{
				try
				{
					UdpReceiveResult result = await udpClient.ReceiveAsync();
					string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
					if (receivedMessage.StartsWith("<RSAKeyValue>"))
					{
						SavedData.LAST_RECEIVED_PUBLIC_KEY = receivedMessage;
						Print(" You have received new Public key, to accept '/confirm'\n", ConsoleColor.Red);
					}
					else
					{
						string decrypted = RSAEncryption.Decrypt(receivedMessage);

						if (decrypted.StartsWith("TTT") && TicTacToe.isTurn == false) // TTT part
						{
							string[] parts = decrypted.Split(' ');

							int x;
							int y;

							int.TryParse(parts[1], out x);
							int.TryParse(parts[2], out y);

							if(TicTacToe.CheckMove(x,y))
							{
								TicTacToe.ReceiveMove(x, y);
							}
						} // end TTT part
						else
						{
							Print($"> {decrypted}\n", ConsoleColor.Green);
						}
					}
				}
				catch (Exception e){}
			}
		}
		static async Task SendMessage(string ip, string message)
		{
			try
			{
				if (message.StartsWith("TTT") && TicTacToe.isTurn == true) // TTT part
				{
					string[] parts = message.Split(' ');

					int x;
					int y;

					int.TryParse(parts[1], out x);
					int.TryParse(parts[2], out y);

					if (TicTacToe.CheckMove(x, y))
					{
						TicTacToe.MakeMove(x, y);

						string encrypted = RSAEncryption.Encrypt(message, SavedData.PUBLIC_KEY);

						byte[] bytes = Encoding.UTF8.GetBytes(encrypted);
						await udpClient.SendAsync(bytes, bytes.Length, ip, listenPort);
					}
					else
					{
						Print(" Invalid Move !\n", ConsoleColor.Red);
					}

				} // end TTT part
				else
				{
					string encrypted = RSAEncryption.Encrypt(message, SavedData.PUBLIC_KEY);

					byte[] bytes = Encoding.UTF8.GetBytes(encrypted);
					await udpClient.SendAsync(bytes, bytes.Length, ip, listenPort);
				}
			}
			catch (Exception e)
			{
				Print($" Error sending message: {e.Message}\n", ConsoleColor.Red);
			}
		}
	}
}
