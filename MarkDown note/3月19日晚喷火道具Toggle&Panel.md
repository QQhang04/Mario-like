## 喷火道具实现

### Toggle 脚本功能分析



这个 Toggle 脚本的核心功能是**切换一个开关的状态**（true 或 false），并在状态变化时触发相应的事件 (onActivate 或 onDeactivate)。



此外，它还具有以下功能特性：

✅ 支持**延迟触发**（delay）

✅ 可与其他 Toggle 组件**联动**（multiTrigger）

✅ 使用 **UnityEvent** 来处理激活和停用时的回调，便于在 Unity 编辑器中配置事件



------



**🧩 代码功能逐步拆解**



------



**🔹 成员变量**

```
public bool state = true;     // 当前 Toggle 的初始状态
public float delay;           // 切换状态时的延迟时间
public Toggle[] multiTrigger; // 关联的 Toggle 组件
```

​	•	**state** — 标志当前 Toggle 的开/关状态。

​	•	**delay** — 状态切换时的延迟（以秒为单位）。

​	•	**multiTrigger** — 存储一组其他 Toggle 实例，允许**联动触发**它们的状态变化。



------



**🔹 UnityEvent 事件**

```
public UnityEvent onActivate;   // 激活 (true) 时触发的事件
public UnityEvent onDeactivate; // 停用 (false) 时触发的事件
```

​	•	**onActivate** — 当 Toggle 被激活时（state = true）触发的回调。

​	•	**onDeactivate** — 当 Toggle 被停用时（state = false）触发的回调。



> 🎯 UnityEvent 允许在 Unity 编辑器中轻松配置回调函数，便于设计师或关卡设计师无代码地设置触发器。



------



**🔹 Set() 方法 — 入口函数**

```
public virtual void Set(bool value)
{
    StopAllCoroutines();          // 停止可能正在运行的协程，避免重复触发
    StartCoroutine(SetRoutine(value)); // 启动协程，处理延迟和事件触发
}
```

✅ **StopAllCoroutines()** 确保每次调用 Set() 时，之前未完成的状态切换操作会被中断。

✅ 使用 StartCoroutine() 启动 SetRoutine()，处理带有延迟的状态切换。



------



**🔹 SetRoutine() 方法 — 状态切换逻辑**

```
protected virtual IEnumerator SetRoutine(bool value)
{
    yield return new WaitForSeconds(delay);  // 等待指定延迟时间

    if (value) // 请求激活
    {
        if (!state) // 只有当前状态是 "未激活" 时，才能激活
        {
            state = true;

            foreach (var toggle in multiTrigger)
            {
                toggle.Set(state); // 触发其他关联的 Toggle
            }

            onActivate?.Invoke(); // 调用激活回调
        }
    }
    else if (state) // 请求停用
    {
        state = false;

        foreach (var toggle in multiTrigger)
        {
            toggle.Set(state); // 触发其他关联的 Toggle
        }

        onDeactivate?.Invoke(); // 调用停用回调
    }
}
```

**🔎 关键点分析**



✅ **延迟机制**：

​	•	yield return new WaitForSeconds(delay);

→ 保证 Set() 被调用后经过指定时间才触发切换。



✅ **激活逻辑 (if (value))**：

​	•	仅在 state == false 时执行激活，避免重复触发事件。

​	•	onActivate?.Invoke() 确保 onActivate 不为空时再触发回调。



✅ **停用逻辑 (else if (state))**：

​	•	仅在 state == true 时执行停用。

​	•	同样通过 onDeactivate?.Invoke() 来触发停用回调。



✅ **联动机制 (multiTrigger)**：

​	•	每次状态切换时，遍历 multiTrigger 列表，并对每个 Toggle 递归调用 Set()，实现链式联动。



------



**🚀 脚本的典型使用场景**



------



**🎯 1. 触发机关**



✅ 在关卡中设置 Toggle 作为机关触发器，控制**门、陷阱、升降平台**等对象。

✅ onActivate / onDeactivate 可分别用于播放动画、音效或更改灯光等效果。



> **示例**：按下开关后，3秒后门打开。

```
public class Door : MonoBehaviour
{
    public Animator animator;

    public void OpenDoor() => animator.SetTrigger("Open");
    public void CloseDoor() => animator.SetTrigger("Close");
}
```

🔧 Unity 编辑器配置示例：

​	•	Toggle 的 onActivate → **Door.OpenDoor()**

​	•	Toggle 的 onDeactivate → **Door.CloseDoor()**



------



**🎯 2. 联动开关**



✅ 使用 multiTrigger 可创建复杂的连锁反应机制，例如：

​	•	打开第一个开关 → 自动触发其他开关

​	•	机关 A 激活后 → 5 秒后机关 B 激活



> **示例：打开一个主机关，触发多个子机关**



​	•	主Toggle 的 multiTrigger 中包含 3 个子 Toggle

​	•	子 Toggle 负责触发各自的物件（门、灯光、机关等）



------



**🎯 3. 延迟触发机制**



✅ 在游戏中用于营造节奏感，比如：

​	•	玩家踩中地雷，2 秒后爆炸

​	•	游戏倒计时结束后，触发 GameOver 事件



> **示例：踩中陷阱 3 秒后触发爆炸动画**

```
public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
```

🔧 Unity 编辑器配置示例：

​	•	Toggle 设置 delay = 3

​	•	onActivate → **Bomb.Explode()**



------



**🔥 总结**



✅ Toggle 通过 Set() 方法控制状态切换，SetRoutine() 实现延迟与事件触发。

✅ 支持 multiTrigger 实现多 Toggle 组件的**链式联动**。

✅ 使用 UnityEvent 提供灵活的**事件回调机制**，便于非编程人员在 Unity 编辑器内进行设置。

✅ 适合用作游戏中的**机关触发器**、**联动系统**、**延迟机制**等。



**🎯 Panel 类功能概述**



Panel 类实现了一个可交互的触发器机制，类似于游戏中的**压力板**、**按钮**或**踏板机关**。它会在指定条件下触发激活 (Activate) 或停用 (Deactivate) 事件，同时支持音效、玩家/非玩家检测及自动切换功能。



------



**🚀 核心功能**

​	1.	**激活/停用机制**

​	•	提供 Activate() 和 Deactivate() 方法，用于控制面板的激活和停用状态。

​	•	每次切换状态时可播放对应的音效 (activateClip / deactivateClip)。

​	2.	**触发条件控制**

​	•	可设置触发器是否需要特定条件：

​	•	requirePlayer → 仅允许玩家激活

​	•	requireStomp → 仅允许玩家使用“踩踏”技能激活

​	3.	**自动切换 (autoToggle)**

​	•	激活后，当触发条件不再满足时，是否自动回到停用状态。

​	4.	**事件回调**

​	•	OnActivate → 激活时触发的事件回调

​	•	OnDeactivate → 停用时触发的事件回调

​	5.	**碰撞检测与实体交互**

​	•	通过 OnEntityContact() 和 OnCollisionStay() 处理来自实体或物体的触发逻辑。



------



### Panel脚本功能分析



**➤ 1. Activate() 方法 (激活)**

```
public virtual void Activate()
{
    if (!activated) // 防止重复激活
    {
        if (activateClip) 
        {
            m_audio.PlayOneShot(activateClip); // 播放激活音效
        }

        activated = true;    // 标记为已激活
        OnActivate?.Invoke(); // 触发激活回调
    }
}
```

✅ 若 activated == false（尚未激活），则：

​	•	播放激活音效

​	•	设置 activated = true，标记为激活状态

​	•	执行 OnActivate 事件回调（例如：开启大门、激活平台等）



------



**➤ 2. Deactivate() 方法 (停用)**

```
public virtual void Deactivate()
{
    if (activated) // 防止重复停用
    {
        if (deactivateClip)
        {
            m_audio.PlayOneShot(deactivateClip); // 播放停用音效
        }

        activated = false;   // 标记为未激活
        OnDeactivate?.Invoke(); // 触发停用回调
    }
}
```

✅ 若 activated == true（已激活），则：

​	•	播放停用音效

​	•	设置 activated = false，标记为未激活

​	•	执行 OnDeactivate 事件回调（例如：关闭大门、停用激光等）



------



**➤ 3. Start() 方法 (初始化)**

```
protected virtual void Start()
{
    gameObject.tag = GameTags.Panel; // 设置标签，方便识别
    m_collider = GetComponent<Collider>(); // 获取自身 Collider
    m_audio = GetComponent<AudioSource>(); // 获取音频组件
}
```

✅ 设置初始标签、碰撞器及音频组件，确保 Panel 正常工作。



------



**➤ 4. Update() 方法 (检测触发条件)**

```
protected virtual void Update()
{
    if (m_entityActivator || m_otherActivator)
    {
        var center = m_collider.bounds.center;
        var contactOffset = Physics.defaultContactOffset + 0.1f;
        var size = m_collider.bounds.size + Vector3.up * contactOffset;
        var bounds = new Bounds(center, size);

        var intersectsEntity = m_entityActivator && bounds.Intersects(m_entityActivator.bounds);
        var intersectsOther = m_otherActivator && bounds.Intersects(m_otherActivator.bounds);

        if (intersectsEntity || intersectsOther)
        {
            Activate(); // 满足触发条件时激活
        }
        else
        {
            m_entityActivator = intersectsEntity ? m_entityActivator : null;
            m_otherActivator = intersectsOther ? m_otherActivator : null;

            if (autoToggle) // 自动切换机制
            {
                Deactivate();
            }
        }
    }
}
```

✅ 检测触发器是否有**玩家或其他物体**位于面板内

✅ 计算范围 (Bounds) 以判断触发器与实体的**碰撞检测**

✅ 若条件满足 → 执行 Activate()

✅ 若触发条件消失 → 执行 Deactivate()（仅当 autoToggle == true）



------



**➤ 5. OnEntityContact() 方法 (实体交互检测)**

```
public void OnEntityContact(Entity entity)
{
    if (entity.velocity.y <= 0 && entity.IsPointUnderStep(m_collider.bounds.max))
    {
        if ((!requirePlayer || entity is Player) &&
            (!requireStomp || (entity as Player).states.IsCurrentOfType(typeof(StompPlayerState))))
        {
            m_entityActivator = entity.controller; // 将实体标记为激活者
        }
    }
}
```

✅ 检测实体是否符合以下条件：

​	•	**垂直速度 ≤ 0**（确保实体从上方压下）

​	•	requirePlayer → 若启用，则仅允许玩家触发

​	•	requireStomp → 若启用，则仅允许玩家使用“踩踏”技能触发



------



**➤ 6. OnCollisionStay() 方法 (物体碰撞检测)**

```
protected virtual void OnCollisionStay(Collision collision)
{
    if (!(requirePlayer || requireStomp) && !collision.collider.CompareTag(GameTags.Player))
    {
        m_otherActivator = collision.collider; // 标记其他物体为触发器
    }
}
```

✅ 检测非玩家物体是否与 Panel 保持接触（用于非玩家激活的场景）



------



**🧩 应用场景示例**



**🎯 1. 游戏场景 - 激活大门**

​	•	玩家踩踏地板机关后，大门打开，离开时大门自动关闭。



**🔹 配置示例**

​	•	autoToggle = true

​	•	requirePlayer = true

​	•	OnActivate → 打开大门

​	•	OnDeactivate → 关闭大门



------



**🎯 2. 机关陷阱 - 延时触发**

​	•	玩家站在面板上 2 秒后，触发陷阱，离开时陷阱重置。



**🔹 配置示例**

​	•	autoToggle = true

​	•	requirePlayer = true

​	•	OnActivate → 启动陷阱（如尖刺、火焰等）

​	•	OnDeactivate → 停止陷阱



------



**🎯 3. 重物机关 - 仅物品触发**

​	•	大石块或箱子推到机关上，触发隐藏通道。



**🔹 配置示例**

​	•	autoToggle = false

​	•	requirePlayer = false

​	•	OnActivate → 打开隐藏门



------



**🔥 总结**



✅ Panel 类是一个灵活的触发器组件，专为游戏场景中的机关、陷阱、谜题等设计。

✅ 提供了**激活/停用**、**延迟检测**、**自动切换**等多种机制，满足多样化的交互需求。

✅ 通过 UnityEvent 提供高度可扩展性，易于整合到不同的游戏场景中。



**❓ 为什么 Update() 中还要使用 Bounds 检测？不能仅依赖 OnEntityContact() 和 OnCollisionStay() 吗？**



这是一个很棒的问题！Update() 中使用 Bounds 检测并不是多余的，反而是解决了一些**关键问题**。以下是它的必要性及优势的详细解释：



------



**🚨 1. OnEntityContact() 和 OnCollisionStay() 的局限性**



OnEntityContact() 和 OnCollisionStay() 各自的触发机制存在以下问题：



**🔹 OnEntityContact() 的局限性**

​	•	仅在**首次接触**时触发。

​	•	如果实体在面板上持续停留，不会再次调用。

​	•	如果玩家缓慢地“滑动”到面板外，无法检测离开。



**🔹 OnCollisionStay() 的局限性**

​	•	仅在物理引擎的碰撞检测有效时触发。

​	•	**非物理实体**（如trigger collider、特定脚本控制的对象）可能不会触发此方法。

​	•	若物体以较高速度进入面板，可能在物理更新的间隙中错过检测。



> 🚫 **问题示例：**

> 假设玩家缓慢移动到压力板边缘，碰撞器的边界可能只在物理帧的某个瞬间检测到接触，但不会在玩家离开时触发任何事件。



------



**✅ 2. Bounds 检测的优势**



Bounds 提供了一种更**稳定**、**连续**的触发机制，具有以下优点：



✅ **每帧检测**，确保触发器的状态始终精准。

✅ Bounds.Intersects() 可检测**持续接触**，无论是刚体物体还是非刚体物体。

✅ 即使触发器外形不规则，Bounds 的扩展范围 (contactOffset) 也可以更灵活地控制触发条件。



------



**🧩 3. 代码流程中的作用**



**➤ OnEntityContact() 和 OnCollisionStay() 的职责**

​	•	这两个方法的核心功能是**标记触发者**（m_entityActivator 或 m_otherActivator）。

​	•	它们只负责记录谁触发了面板，但不负责管理触发条件的**持续检测**。

```
public void OnEntityContact(Entity entity)
{
    if (entity.velocity.y <= 0 && entity.IsPointUnderStep(m_collider.bounds.max))
    {
        if ((!requirePlayer || entity is Player) &&
            (!requireStomp || (entity as Player).states.IsCurrentOfType(typeof(StompPlayerState))))
        {
            m_entityActivator = entity.controller;  // 仅负责标记触发器
        }
    }
}
```





------



**➤ Update() 的职责 (重点)**

​	•	每帧检查 m_entityActivator 和 m_otherActivator 是否仍然**位于面板上方**。

​	•	使用 Bounds 提供更精确的范围检测，避免玩家或物体在物理帧遗漏时的触发失败。

```
protected virtual void Update()
{
    if (m_entityActivator || m_otherActivator)
    {
        var center = m_collider.bounds.center;
        var contactOffset = Physics.defaultContactOffset + 0.1f;
        var size = m_collider.bounds.size + Vector3.up * contactOffset;
        var bounds = new Bounds(center, size);  // 动态计算面板边界

        var intersectsEntity = m_entityActivator && bounds.Intersects(m_entityActivator.bounds);
        var intersectsOther = m_otherActivator && bounds.Intersects(m_otherActivator.bounds);

        if (intersectsEntity || intersectsOther)
        {
            Activate();  // 持续接触 → 保持激活
        }
        else
        {
            m_entityActivator = intersectsEntity ? m_entityActivator : null;
            m_otherActivator = intersectsOther ? m_otherActivator : null;

            if (autoToggle)  // 若允许自动切换，则触发 Deactivate
            {
                Deactivate();
            }
        }
    }
}
```





------



**🚀 4. 示例场景 - Bounds 的作用**



**🎯 问题场景**

​	•	玩家缓慢走上面板，物理帧可能漏掉 OnCollisionStay()。

​	•	玩家在面板边缘反复移动，容易产生错误激活或未及时停用的情况。



**🟩 使用 Bounds 解决问题**



Bounds 可以在每帧动态计算范围，即便玩家缓慢地滑动到面板边缘，依然能够精准地检测触发条件。



------



**🔍 5. 总结：为什么要结合 Bounds？**

| **特性**                    | OnEntityContact() **/** OnCollisionStay() | Bounds       |
| --------------------------- | ----------------------------------------- | ------------ |
| **首次接触检测**            | ✅                                         | ✅            |
| **持续接触检测**            | 🚫 (依赖物理引擎触发)                      | ✅ (每帧检测) |
| **非物理对象检测**          | 🚫 (非刚体可能失败)                        | ✅ (更稳定)   |
| **缓慢移动检测 (滑动边缘)** | 🚫 (可能漏检)                              | ✅ (更精准)   |
| **实体离开时的检测**        | 🚫 (不会触发事件)                          | ✅ (确保触发) |

**🔥 最佳实践**



✅ 使用 OnEntityContact() / OnCollisionStay() 来**标记触发者**。

✅ 使用 Update() + Bounds 来**稳定判断是否仍在触发范围内**。



这种组合可以最大程度确保触发器的精准性和稳定性，是一种更健壮的设计模式。💪