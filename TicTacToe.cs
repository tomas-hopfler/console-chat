using ConsoleChat;

namespace ConsoleNetChat
{
	public static class TicTacToe
	{
		public static bool view = false;
		public static string[,] field;
		public static bool isTurn = true;
		public static string Player = "X";
		public static string Enemy = "O";

		public static void MakeField()
		{
			field = new string[3, 3];
			for(int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					field[i, j] = " ";
				}
			}
		}
		public static void MakeMove(int x, int y)
		{
			try
			{
				field[x, y] = Player;
				isTurn = false;
				CheckGameState();
			}
			catch(Exception e)
			{

			}
		}
		public static void ReceiveMove(int x, int y)
		{
			try
			{
				field[x, y] = Enemy;
				isTurn = true;
				CheckGameState();
			}
			catch (Exception e)
			{

			}
		}
		public static bool CheckMove(int x, int y)
		{
			if (field[x,y] != " ")
			{
				return false;
			}
			return true;
		}
		public static void CheckGameState()
		{
			Program.RedrawGUI();
			if (CheckEndGame(Player))
			{
				Console.WriteLine("\n\t You have WON !");
				MakeField();
			}
			if (CheckEndGame(Enemy))
			{
				Console.WriteLine("\n\t You have LOST !");
				MakeField();
			}
		}
		public static bool CheckEndGame(string player)
		{
			for (int i = 0; i < 3; i++)
			{
				if (field[i, 0] == player && field[i, 1] == player && field[i, 2] == player)
					return true;
			}
			for (int i = 0; i < 3; i++)
			{
				if (field[0, i] == player && field[1, i] == player && field[2, i] == player)
					return true;
			}
			if (field[0, 0] == player && field[1, 1] == player && field[2, 2] == player)
			{
				return true;
			}
			if (field[0, 2] == player && field[1, 1] == player && field[2, 0] == player)
			{
				return true;
			}
			return false;
		}
		public static void DrawField()
		{
			Console.Write("\n     0   1   2  \n");
			Console.Write("   -------------\n");
			for (int i = 0; i < 3; i++)
			{
				Console.Write(" " + i + " |");
				for (int j = 0; j < 3; j++)
				{
					Console.Write(" " + field[i, j] + " |");
				}
				Console.Write("\n   -------------\n");
			}
		}
	}
}
