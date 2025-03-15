/// <summary>
/// 提供对关卡（Level）相关的静态数据和引用的访问，例如 Player 引用。
/// 扮演全局信息管理者，对关卡中的关键元素（如玩家、环境、起点、终点）进行访问或缓存。
/// 专注于与关卡数据相关的逻辑，而非关卡控制流程。
/// </summary>
public class Level : Singleton<Level>
{
    protected Player m_player;

    public Player player
    {
        get
        {
            if (!m_player)
            {
                m_player = FindObjectOfType<Player>();
            }

            return m_player;
        }
    }
}