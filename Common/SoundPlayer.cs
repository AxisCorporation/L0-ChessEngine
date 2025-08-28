using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace L_0_Chess_Engine.Common;

public static class SoundPlayer
{
    public static readonly string MoveSFXPath = "Assets/Audio/Move.mp3";
    public static readonly string IllegalMoveSFXPath = "Assets/Audio/IllegalMove.mp3";
    public static readonly string CaptureSFXPath = "Assets/Audio/Capture.mp3";
    public static readonly string CheckSFXPath = "Assets/Audio/Check.mp3";
    public static readonly string CheckmateSFXPath = "Assets/Audio/Checkmate.mp3";
    public static readonly string CastleSFXPath = "Assets/Audio/Castle.mp3";
    public static readonly string ClickSFXPath = "Assets/Audio/Click.mp3";
    public static readonly string PromotionSFXPath = "Assets/Audio/Promotion.mp3";
    public static readonly string TenSecondsLeftSFXPath = "Assets/Audio/TenSecondsLeft.mp3";

    private static Lock Lock { get; set; } = new();
    private static readonly LibVLC _libVLC = new();
    private static MediaPlayer? _player;

    public static async Task Play(string filePath)
    {
        await Task.Run(() =>
        {
            lock (Lock) 
            {
                _player?.Dispose();
                _player = new MediaPlayer(_libVLC)
                {
                    Media = new Media(_libVLC, filePath, FromType.FromPath)
                };
                _player.Play();
            }

        });

    }
}