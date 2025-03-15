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
    
    public virtual void AddCoins(int amount) => m_score.coins += amount;
    public virtual void Exit() => m_finisher.Exit();

    public virtual void Finish() => m_finisher.Finish();
}