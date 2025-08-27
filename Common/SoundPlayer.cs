using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace L_0_Chess_Engine.Common;

public static class SoundPlayer
{
    public static readonly string  MoveSFXPath = "Assets/Audio/ChessPieceMove.mp3";
    public static readonly string CaptureSFXPath = "Assets/Audio/ChessPieceCapture.mp3";
    public static readonly string ClickSFXPath = "Assets/Audio/ButtonClick.mp3";
    private static readonly LibVLC _libVLC = new();
    private static MediaPlayer? _player;

    public static async Task Play(string filePath)
    {
        await Task.Run(() =>
        {
            _player?.Dispose();
            _player = new MediaPlayer(_libVLC)
            {
                Media = new Media(_libVLC, filePath, FromType.FromPath)
            };
            _player.Play();
        });

    }
}