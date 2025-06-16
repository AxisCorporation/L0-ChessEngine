using CommunityToolkit.Mvvm.ComponentModel;
using L_0_Chess_Engine.SampleModels;

namespace L_0_Chess_Engine.SampleViewModels;

public partial class SChessBoardViewModel : SViewModelBase
{
    [ObservableProperty]
    private SChessBoard _sampleBoard = new();
    
    
}