using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class MainForm : Form
    {
        private Board _board;
        private PieceColor _currentTurn = PieceColor.Black;
        private AI _ai = new AI();
        private Piece _selectedPiece;
        private List<Move> _validMoves;
        private int _selectedX = -1;
        private int _selectedY = -1;
        private ComboBox algorithmComboBox;
        private Label algorithmLabel;
        private Label winnerLabel;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            _board = new Board();
            _ai = new AI();
            _currentTurn = PieceColor.Black;
            _selectedPiece = null;
            _validMoves = new List<Move>();
            boardPanel.Paint += BoardPanel_Paint;
            boardPanel.MouseClick += BoardPanel_MouseClick;
            this.Resize += MainForm_Resize;
            Button resetButton = new Button();
            resetButton.Text = "Reset";
            resetButton.Location = new Point(750, 150); 
            resetButton.Click += ResetButton_Click;

            Button StartAI = new Button();
            StartAI.Text = "StartAI";
            StartAI.Location = new Point(750, 450);
            StartAI.Click += StartAI_Click;
            this.Controls.Add(StartAI);
            this.Controls.Add(resetButton);
        }

        private void StartAI_Click(object sender, EventArgs e)
        {
            ExecuteAIMove(_currentTurn);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            _board.Reset();

            _currentTurn = PieceColor.Black;
            _selectedPiece = null;
            _validMoves = new List<Move>();

            boardPanel.Invalidate();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            boardPanel.Invalidate(); // Redraw the board on resize
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            int cellSize = Math.Min(boardPanel.Width, boardPanel.Height) / Board.Size;

            for (int y = 0; y < Board.Size; y++)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    Brush brush = ((x + y) % 2 == 0) ? Brushes.White : Brushes.Gray;
                    g.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);

                    Piece piece = _board.Squares[x, y];
                    if (piece.Color != PieceColor.None)
                    {
                        Brush pieceBrush = (piece.Color == PieceColor.Red) ? Brushes.Red : Brushes.Black;
                        g.FillEllipse(pieceBrush, x * cellSize + 10, y * cellSize + 10, cellSize - 20, cellSize - 20);

                        if (piece.Type == PieceType.King)
                        {
                            g.DrawString("D", new Font("Arial", 16), Brushes.White, x * cellSize + 10, y * cellSize + 10);
                        }
                    }
                }
            }

            // Highlight valid moves
            foreach (Move move in _validMoves)
            {
                int centerX = (move.EndX * cellSize) + (cellSize / 2);
                int centerY = (move.EndY * cellSize) + (cellSize / 2);
                g.FillEllipse(Brushes.Green, centerX - 10, centerY - 10, 20, 20);
            }
        }

        private void CalculateValidMoves(int x, int y)
        {
            _validMoves.Clear();
            if (_board.Squares[x, y].Color == _currentTurn)
            {
                _validMoves = _ai.GetValidMoves(_board, x, y, _currentTurn);
            }
            boardPanel.Invalidate();
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            winnerLabel.Text = "";
            int cellSize = Math.Min(boardPanel.Width, boardPanel.Height) / Board.Size;
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (_selectedX == -1 && _selectedY == -1)
            {
                if (_board.Squares[x, y].Color == _currentTurn)
                {
                    _selectedX = x;
                    _selectedY = y;
                    CalculateValidMoves(_selectedX, _selectedY);
                    boardPanel.Invalidate();
                }
            }
            else
            {
                Move move = new Move(_selectedX, _selectedY, x, y);
                if (_board.IsValidMove(move, _currentTurn))
                {
                    if (_board.ApplyMove(move) == false) {
                        _validMoves.Clear();
                        boardPanel.Invalidate();

                        _selectedX = -1;
                        _selectedY = -1;
                    }

                    else {
                        _validMoves.Clear();
                        boardPanel.Invalidate();

                        if (_currentTurn == PieceColor.Red)
                        {
                            _currentTurn = PieceColor.Black;
                            ExecuteAIMove(_currentTurn);
                            
                        }
                        else {
                            _currentTurn = PieceColor.Red;
                            ExecuteAIMove(_currentTurn);
                        }
                        _selectedX = -1;
                        _selectedY = -1;
                    }
                }
                else
                {
                    _selectedX = -1;
                    _selectedY = -1;
                }
            }

            boardPanel.Invalidate();
        }

        private void InitializeCustomComponents()
        {
            algorithmComboBox = new ComboBox();
            algorithmComboBox.Items.AddRange(new string[] { "Combined", "Alpha-Beta", "Minimax" });
            algorithmComboBox.SelectedIndex = 0;
            algorithmComboBox.Location = new Point(725, 215);
            algorithmComboBox.SelectedIndexChanged += AlgorithmComboBox_SelectedIndexChanged;
            this.Controls.Add(algorithmComboBox);

            algorithmLabel = new Label();
            algorithmLabel.Text = "Current Algorithm: Minimax";
            algorithmLabel.Location = new Point(750, 190);
            this.Controls.Add(algorithmLabel);

            winnerLabel = new Label();
            winnerLabel.Text = "";
            winnerLabel.Location = new Point(750, 300);
            this.Controls.Add(winnerLabel);
        }

        private void AlgorithmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedAlgorithm = algorithmComboBox.SelectedItem.ToString();
            algorithmLabel.Text = $"Current Algorithm: {selectedAlgorithm}";
        }

        private void ExecuteAIMove(PieceColor color)
        {
            if (color == PieceColor.Black)
            {
                Move aiMove = _board.GetBestMove(_currentTurn, _board, "Alpha-Beta");
                if (aiMove != null)
                {

                    if (_board.ApplyMove(aiMove) == true)
                    {
                        _currentTurn = PieceColor.Red;
                        boardPanel.Invalidate();
                    }
                    else { ExecuteAIMove(_currentTurn); }
                }
                CheckGameEnd();
            }
            if (color == PieceColor.Red)
            {
                Move aiMove = _board.GetBestMove(_currentTurn, _board, "");
                if (aiMove != null)
                {

                    if (_board.ApplyMove(aiMove) == true)
                    {
                        _currentTurn = PieceColor.Black;
                        boardPanel.Invalidate();
                    }
                    else { ExecuteAIMove(_currentTurn); }
                }
                CheckGameEnd();
            }
           
        }

        private void CheckGameEnd()
        {
            if (_board.IsGameOver(_board))
            {
                PieceColor winner = _board.GetWinner();
                if (winner == PieceColor.None)
                {
                    winnerLabel.Text = "It's a draw!";
                }
                else
                {
                    winnerLabel.Text = $"{winner} wins!";
                }
            }
        }
    }
}