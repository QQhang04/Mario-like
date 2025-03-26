using UnityEngine;

/// <summary>
/// 负责关卡流程控制（如暂停、重生、得分、完成关卡等）。
/// 处理关卡状态的动态变化。
/// 调度其他模块，如 LevelPauser、LevelFinisher、LevelRespawner 等。
/// </summary>
public class LevelController : MonoBehaviour
{
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelFinisher m_finisher => LevelFinisher.Instance;
    protected LevelRespawner m_respawner => LevelRespawner.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;
    
    public virtual void AddCoins(int amount) => m_score.coins += amount;
    public virtual void Exit() => m_finisher.Exit();
    public virtual void Finish() => m_finisher.Finish();
    public virtual void Respawn(bool consumeRetries) => m_respawner.Respawn(consumeRetries);
    public virtual void Restart() => m_respawner.Restart();
    // public virtual void CollectStar(int index) => m_score.CollectStar(index);
    public virtual void ConsolidateScore() => m_score.Consolidate();
    public virtual void Pause(bool value) => m_pauser.Pause(value);
}