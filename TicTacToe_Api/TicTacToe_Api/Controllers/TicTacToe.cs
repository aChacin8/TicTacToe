using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Controllers
{
    [ApiController]
    [Route("api/tic-tac-toe/")]
    public class GameController : ControllerBase
    {
        private const char PlayerChar = 'X';
        private const char AiChar = 'O';
        private const char EmptyChar = ' ';
        private static readonly Random _random = new Random();

        [HttpPost("next-move")]
        public IActionResult GetNextMove([FromBody] char[] board)
        {
            if (board == null || board.Length != 9)
                return BadRequest("Tablero inválido.");

            if (IsTerminal(board, out string initialResult))
                return Ok(new { board, status = "Finished", result = initialResult });

            bool isFirstMove = !Array.Exists(board, c => c != EmptyChar); //Funcion para verificar si es el primer movimiento (tablero vacío)

            int move;
            if (isFirstMove)
            {
                move = _random.Next(0, 9);
            }
            else
            {
                //Se agrega un valor aleatorio para que la maquina tenga un 10% de probabilidad de cometer un error, eligiendo una casilla aleatoria en lugar de la mejor jugada
                //Logrando así que la maquina no sea tan predecible y tenga un comportamiento más "natural", haciendo el juego más entretenido para el usuario
                bool shouldMakeMistake = _random.Next(0, 100) < 10;

                if (shouldMakeMistake)
                {
                    move = GetRandomAvailableMove(board);
                }
                else
                {
                    move = FindBestMove(board);
                }
            }

            if (move != -1) board[move] = AiChar;
            double probability = CalculateWinProbability(board, PlayerChar);

            string status = IsTerminal(board, out string finalResult) ? "Finished" : "Playing";

            return Ok(new { board, status, result = finalResult, winProbability = probability  });
        }

        //Esta función implementa una estrategia básica para la maquina, priorizando ganar, bloquear al jugador, tomar el centro, luego las esquinas y finalmente los lados
        private int FindBestMove(char[] board)
        {
            int winMove = GetWinningMove(board, AiChar);
            if (winMove != -1) return winMove;

            int blockMove = GetWinningMove(board, PlayerChar);
            if (blockMove != -1) return blockMove;

            if (board[4] == EmptyChar) return 4; //La mejor jugada es siempre tomar el centro si está disponible

            int[] corners = { 0, 2, 6, 8 };
            int[] sides = { 1, 3, 5, 7 };

            int cornerMove = GetRandomFromSet(board, corners);
            if (cornerMove != -1) return cornerMove;

            return GetRandomFromSet(board, sides);
        }

        private int GetRandomFromSet(char[] board, int[] positions)
        {
            var available = positions.Where(p => board[p] == EmptyChar).ToList();
            if (available.Count == 0) return -1;
            return available[_random.Next(available.Count)];
        }

        private int GetRandomAvailableMove(char[] board)
        {
            var available = board.Select((value, index) => new { value, index })
                                 .Where(x => x.value == EmptyChar)
                                 .Select(x => x.index)
                                 .ToList();
            return available.Count > 0 ? available[_random.Next(available.Count)] : -1;
        }

        private int GetWinningMove(char[] board, char player)
        {
            int[,] lines = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            for (int i = 0; i < 8; i++)
            {
                int a = lines[i, 0], b = lines[i, 1], c = lines[i, 2];
                if (board[a] == player && board[b] == player && board[c] == EmptyChar) return c;
                if (board[a] == player && board[c] == player && board[b] == EmptyChar) return b;
                if (board[b] == player && board[c] == player && board[a] == EmptyChar) return a;
            }
            return -1;
        }

        private bool IsTerminal(char[] b, out string res)
        {
            int[,] lines = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            for (int i = 0; i < 8; i++)
            {
                if (b[lines[i, 0]] != EmptyChar && b[lines[i, 0]] == b[lines[i, 1]] && b[lines[i, 0]] == b[lines[i, 2]])
                {
                    res = b[lines[i, 0]] == AiChar ? "Gana la IA (O)" : "¡Ganaste tú (X)!";
                    return true;
                }
            }
            if (!Array.Exists(b, cell => cell == EmptyChar)) { res = "Empate"; return true; }
            res = "None";
            return false;
        }

        //La probabilidad se calcula contando cuántas líneas de victoria posibles quedan para el jugador después del movimiento de la maquina, y se expresa como un porcentaje del total de líneas
        private double CalculateWinProbability(char[] board, char player)
        {
            int[,] lines = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            int availableLines = 0;
            char opponent = (player == PlayerChar) ? AiChar : PlayerChar;

            for (int i = 0; i < 8; i++)
            {
                bool hasOpponent = board[lines[i, 0]] == opponent ||
                                   board[lines[i, 1]] == opponent ||
                                   board[lines[i, 2]] == opponent;

                if (!hasOpponent) availableLines++;
            }

            return Math.Round((availableLines / 8.0) * 100, 2);
        }
    }
}