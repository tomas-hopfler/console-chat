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

	class Program
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
			Console.WriteLine(RSAEncryption.GetPublicKey());

			MenuGetIP(out SavedData.IP);

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
		static void RedrawGUI()
		{
			Console.Clear();
			Print(" ███████████████████████████████████████\n", ConsoleColor.Green);
			Print(" Type '/q' to quit.\n", ConsoleColor.Cyan);
			Print(" Type '/x' to change destination IP.\n", ConsoleColor.Cyan);
			Print(" Type '/c' to clear chat history.\n", ConsoleColor.Cyan);
			Print(" Type '/r' to reset encryption key.\n", ConsoleColor.Cyan);
			Print(" Type '/request' to send your public key.\n", ConsoleColor.Cyan);
			Print(" ███████████████████████████████████████\n", ConsoleColor.Green);
			Print(" Recipient IP: " + SavedData.IP);
			Print("\n");
			PrintClients();
			Print(" ███████████████████████████████████████\n", ConsoleColor.Green);
			Print("\n");
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
		static void PrintClients()
		{
			Print("\n Network clients: \n");
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

			foreach (NetworkInterface networkInterface in interfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up &&
					networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				{
					IPInterfaceProperties properties = networkInterface.GetIPProperties();

					foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
					{
						if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							Print(" ");
							try
							{
								string hostName = Dns.GetHostEntry(ip.Address).HostName;
								Print(hostName);
							}
							catch (Exception ex)
							{
								Print("Unknown\n");
							}
							Print("\t\t" + ip.Address + "\n");
						}
					}
				}
			}
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
						Print(" You have received new Public key, to accept send /confirm\n", ConsoleColor.Red);
					}
					else
					{
						string decrypted = RSAEncryption.Decrypt(receivedMessage);
						Print($"> {decrypted}\n", ConsoleColor.Green);
					}
				}
				catch (Exception e){}
			}
		}
		static async Task SendMessage(string ip, string message)
		{
			try
			{
				string encrypted = RSAEncryption.Encrypt(message, SavedData.PUBLIC_KEY);

				byte[] bytes = Encoding.UTF8.GetBytes(encrypted);
				await udpClient.SendAsync(bytes, bytes.Length, ip, listenPort);
			}
			catch (Exception e)
			{
				Print($" Error sending message: {e.Message}\n", ConsoleColor.Red);
			}
		}
	}
}
