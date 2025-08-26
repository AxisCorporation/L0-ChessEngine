using LibVLCSharp.Shared;

namespace L_0_Chess_Engine.Common;

public static class SoundPlayer
{
    private static LibVLC _libVLC = new LibVLC();
    private static MediaPlayer? _player;

    public static void Play(string filePath)
    {
        _player?.Dispose();
        _player = new MediaPlayer(_libVLC)
        {
            Media = new Media(_libVLC, filePath, FromType.FromPath)
        };
        _player.Play();
    }
}