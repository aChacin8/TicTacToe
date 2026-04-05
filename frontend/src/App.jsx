import { useState, useEffect } from 'react';

const TicTacToe = () => {
  const [board, setBoard] = useState(Array(9).fill(' '));
  const [status, setStatus] = useState("Iniciando...");
  const [gameOver, setGameOver] = useState(false);
  const [loading, setLoading] = useState(false);
  const [probability, setProbability] = useState(0);

  useEffect(() => {
    startNewGame();
  }, []);

  const startNewGame = async () => {
    const emptyBoard = Array(9).fill(' ');
    setGameOver(false);
    setBoard(emptyBoard);

    fetchNextMove(emptyBoard); //Tablero vacio para que la maquina realice el primer movimiento
  };

  const fetchNextMove = async (currentBoard) => {
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5299/api/tic-tac-toe/next-move', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(currentBoard)
      });

      const data = await response.json();
      setBoard(data.board);
      setProbability(data.winProbability);

      if (data.status === "Finished") {
        setStatus(data.result);
        setGameOver(true);
        if (data.result.includes("Ganaste")) setProbability(100);
        else if (data.result.includes("Gana la IA")) setProbability(0);
      } else {
        setStatus("Tu turno (X)");
      }
    } catch (error) {
      setStatus("Error de comunicación");
    } finally {
      setLoading(false);
    }
  };

  const handleCellClick = (index) => {
    if (board[index] !== ' ' || gameOver || loading) return;

    const updatedBoard = [...board];
    updatedBoard[index] = 'X';
    setBoard(updatedBoard);
    setStatus("Maquina pensando...");

    fetchNextMove(updatedBoard);
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h2>Usuario vs Maquina</h2>
      <div style={{ marginBottom: '20px' }}>
        <p>Probabilidad de victoria (Usuario): <strong>{probability}%</strong></p>
        <div style={{
          width: '300px', height: '20px', backgroundColor: '#eee',
          margin: '0 auto', borderRadius: '10px', overflow: 'hidden'
        }}>
          <div style={{
            width: `${probability}%`, height: '100%',
            backgroundColor: probability > 50 ? '#28a745' : '#ffc107',
            transition: 'width 0.5s ease-in-out'
          }} />
        </div>
      </div>
      <p style={{ fontWeight: 'bold', color: gameOver ? 'red' : 'gray' }}>{status}</p>

      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(3, 100px)',
        gap: '8px',
        justifyContent: 'center'
      }}>
        {board.map((cell, i) => (
          <button
            key={i}
            onClick={() => handleCellClick(i)}
            disabled={cell !== ' ' || gameOver || loading}
            style={{
              width: '100px', height: '100px', fontSize: '2rem',
              cursor: cell === ' ' ? 'pointer' : 'default',
              backgroundColor: cell === 'X' ? '#468065' : cell === 'O' ? '#da3d4a' : '#fdfdfd'
            }}
          >
            {cell}
          </button>
        ))}
      </div>

      <button
        onClick={startNewGame}
        style={{ marginTop: '30px', padding: '10px 20px', cursor: 'pointer' }}
      >
        Reiniciar y que empiece la maquina
      </button>
    </div>
  );
};

export default TicTacToe;