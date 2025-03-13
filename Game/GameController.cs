using UnityEngine;

public class GameController : MonoBehaviour
{
    protected Game m_game => Game.Instance;
    protected GameLoader m_loader => GameLoader.Instance;

    public virtual void LoadScene(string scene) => m_loader.Load(scene);
}