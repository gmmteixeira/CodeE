using Unity.Entities;

public partial class GameSystem : SystemBase
{
    protected override void OnUpdate()
    {
        GameComponentData game = default;
        if (SystemAPI.HasSingleton<GameComponentData>())
        {
            game = SystemAPI.GetSingleton<GameComponentData>();
        }
        else return;
        if (game.tutorial == -1)
        {
            game.tutorial = TutorialBootstrap.value;
        }
        SystemAPI.SetSingleton(game);
    }
    
}