using System.Diagnostics;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using L_0_Chess_Engine.Views;
using L_0_Chess_Engine.AI;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    public bool GameRunning { get; set; }
    public TimeSpan WhiteTimer { get; set; }

    [ObservableProperty]
    private string _whiteTimerText;

    public TimeSpan BlackTimer { get; set; }

    [ObservableProperty]
    private string _blackTimerText;

    [ObservableProperty]
    private string? _turnText;

    private readonly string _timeFormat = @"m\:ss";

    // This is a Temp Variable
    [ObservableProperty]
    private string? _gameStateText = "";

    private ChessBoard Board { get; set; } = ChessBoard.Instance;

    private Ai _ai;

    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];

    private bool _isWhiteTurn;
    public bool IsWhiteTurn
    {
        get => _isWhiteTurn;
        set
        {
            _isWhiteTurn = value;
            NotifyCanClickSquares();
            UpdateTurnText();
        }
    }

    private SquareViewModel? _selectedSquare;

    public GameViewModel(int timeLimit, bool LoadAi = false)
    {
        WhiteTimer = TimeSpan.FromMinutes(timeLimit);
        BlackTimer = TimeSpan.FromMinutes(timeLimit);

        WhiteTimerText = WhiteTimer.ToString(_timeFormat); // Formats the time in 5:00
        BlackTimerText = BlackTimer.ToString(_timeFormat);

        for (int row = 7; row >= 0; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                var piece = Board.Grid[col, row]; // or however your grid is structured

                SquareViewModel square = new(piece)
                {
                    IsLightSquare = (row + col + 1) % 2 == 0
                };

                square.ClickCommand = new RelayCommand(() => OnSquareClick(square), () => CanClickSquare(square));

                GridPieces.Add(square);
            }
        }

        Board.GridUpdated += UpdateGrid;
        GameRunning = true;
        IsWhiteTurn = true;

        _ = UpdateTurnTimersAsync();
        
        if (LoadAi)
        {
            LoadAiModule();
        }
    }


    private void UpdateGrid()
    {
        for (int i = 0; i < 64; i++)
        {
            int row = i / 8;
            int col = 7 - (i % 8);

            var updatedPiece = Board.Grid[col, row];
            var existingSquare = GridPieces[63 - i];

            existingSquare.Piece = updatedPiece;
            existingSquare.UpdateImage();
        }
    }


    private void OnSquareClick(SquareViewModel squareClicked)
    {
        Debug.WriteLine($"DEBUG: {squareClicked.Piece.Type} | {squareClicked.Piece.Coordinates}");

        if (_selectedSquare is null)
        {
            _selectedSquare = squareClicked;
            _selectedSquare.IsSelected = true;
            NotifyCanClickSquares();
        }
        else
        {
            Move move = new(_selectedSquare!.Piece, squareClicked.Piece);

            if (!move.IsValid || move.TargetsKing || ChessBoard.Instance.WouldCauseCheck(move))
            {
                _selectedSquare.IsSelected = false;
                _selectedSquare = null;
                return;
            }
            
            if (IsPawnPromotionMove(_selectedSquare.Piece, squareClicked.Piece))
            {
                ShowPawnPromotionDialog(_selectedSquare.Piece.IsWhite, () =>
                {
                    // This will be called after promotion piece is selected
                    Board.MakeMove(move);
                    IsWhiteTurn = !IsWhiteTurn;
                    _selectedSquare.IsSelected = false;
                    _selectedSquare = null;
                    UpdateGameStateText();
                });
                return;
            }

            Board.MakeMove(move);

            IsWhiteTurn = !IsWhiteTurn;

            if (_ai is not null && !IsWhiteTurn)
            {
                MakeAiMove();
                IsWhiteTurn = true;
            }

            _selectedSquare.IsSelected = false;
            _selectedSquare = null;

            UpdateGameStateText();
        }
    }

    private bool IsPawnPromotionMove(ChessPiece initPiece, ChessPiece destPiece)
    {
        // Check if the piece is a pawn
        if (!initPiece.EqualsUncolored(PieceType.Pawn))
        {
            return false;
        }
            
        int destY = destPiece.Coordinates.Y;
        return (initPiece.IsWhite && destY == 7) || (!initPiece.IsWhite && destY == 0);
    }

    private void ShowPawnPromotionDialog(bool isWhite, Action onPieceSelected)
    {
        var promotionView = new PawnPromotionView(isWhite);
        var viewModel = (PawnPromotionViewModel)promotionView.DataContext!;
        
        viewModel.PieceSelected += (pieceType) =>
        {
            ClosePromotionDialog();
            onPieceSelected?.Invoke();
        };
        
        ShowPromotionDialog(promotionView);
    }
    private void ShowPromotionDialog(PawnPromotionView view)
    {
        var window = new Window
        {
            Content = view,
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SystemDecorations = SystemDecorations.None,
            CanResize = false,
            ShowInTaskbar = false,
            Title = "Pawn Promotion"
        };
        
        _promotionWindow = window;
        window.Show();
    }
    private Window? _promotionWindow;
    private void ClosePromotionDialog()
    {
        _promotionWindow?.Close();
        _promotionWindow = null;
    }

    private void NotifyCanClickSquares()
    {
        foreach (var piece in GridPieces)
        {
            ((RelayCommand) piece.ClickCommand!).NotifyCanExecuteChanged();
        }
    }

    private bool CanClickSquare(SquareViewModel squareClicked)
    {
        if (!GameRunning)
        {
            return false;
        }

        if (_ai is not null && !IsWhiteTurn)
        {
            return false;
        }

        if (_selectedSquare is null && squareClicked.Piece != PieceType.Empty && squareClicked.Piece.IsWhite == IsWhiteTurn)
        {
            return true;
        }
        else if (_selectedSquare is not null)
        {
            return true;
        }

        return false;
    }

    private void LoadAiModule()
    {
        _ai = new Ai();
    }

    private void MakeAiMove()
    {
        Board.MakeMove(_ai.GenerateMove());
    }

    private void UpdateTurnText() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";

    private void UpdateGameStateText()
    {
        if (Board.IsCheck)
        {
            GameStateText = IsWhiteTurn ? "White is in Check!" : "Black is in Check!";
        }
        else if (Board.IsCheckMate)
        {
            GameStateText = IsWhiteTurn ? "Checkmate for Black!" : "Checkmate for White!";
        }
        else if (WhiteTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for White - Black wins!";
        }
        else if (BlackTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for Black - White wins!";
        }
        else
        {
            GameStateText = "";
        }
    }

    private async Task UpdateTurnTimersAsync()
    {
        while (GameRunning)
        {
            await Task.Delay(100);
            if (IsWhiteTurn)
            {
                WhiteTimer -= TimeSpan.FromMilliseconds(100);
                WhiteTimerText = WhiteTimer.ToString(_timeFormat);
            }
            else
            {
                BlackTimer -= TimeSpan.FromMilliseconds(100);
                BlackTimerText = BlackTimer.ToString(_timeFormat);
            }

            if (WhiteTimer <= TimeSpan.Zero || BlackTimer <= TimeSpan.Zero)
            {
                GameRunning = false;

                NotifyCanClickSquares();
                UpdateGameStateText();
            }
        }
    }
}