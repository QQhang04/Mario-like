### Create a Game like Super-Mario
- copyright by qqh
# 类Mario游戏介绍

钱启航



### 1.整体项目

本demo用于展示一个3c系统, 用于模仿本人最喜欢的游戏马里奥奥德赛以及双人成行的操作体验

在开发前好好整理了整体的框架, 秉持单一职责的原则分模块进行开发, 使得项目可扩展性高,  低耦合高内聚, 同时在后期利用了单例, 工厂模式, 策略模式, 观察者模式等设计模式重构整个系统

这套系统的核心在于对角色的控制

### 2.角色流畅丝滑的控制 包括但不限于:

#### 2.1 输入模块的设计

- **统一的 Action Asset 管理**  
  通过 `InputActionAsset` 集中定义并管理所有游戏动作（Movement、Jump、Run、Look、Pause、Spin、Dash 等），可视化、易维护，且无需硬编码按键映射。

- **缓存与启用/禁用控制**  
  - 在 `Awake` 中执行 `CacheActions()`，提前获取并缓存所有 `InputAction` 引用，避免运行时频繁查表带来的性能开销。  
  - 在 `OnEnable/OnDisable` 中统一调用 `actions.Enable()`/`actions.Disable()`，可按需求整组开启或屏蔽所有输入。

- **可扩展的面向对象设计**  
  - 采用 `protected virtual` 方法封装各类输入查询（`GetJumpDown`、`GetMovementDirection` 等），方便子类重写或注入自定义逻辑。  
  - 通过 `LockMovementDirection` 提供简易的移动锁定接口，可与动画事件、技能冷却等系统无缝对接。

- **高质量摇杆与镜头输入**  
  - 自动处理摇杆“十字死区”（Cross Dead Zone），净化小幅度漂移；通过 `RemapToDeadZone` 实现线性映射，兼顾灵敏度与稳定性。  
  - `GetMovementCameraDirection` 方法将输入方向根据主摄像机朝向旋转，支持全3D场景下的角色与视角解耦移动。

- **跳跃缓冲（Jump Buffer）机制**  
  在 `Update` 中记录跳跃按键按下时刻，并在固定时间窗口内判断是否触发真正的跳跃，实现“预按跳跃”与“卡关跳跃”容错，极大提升手感。

- **多平台、异设备自动识别**  
  - 统一支持手柄、键盘、鼠标等多种输入设备；  
  - `IsLookingWithMouse` 可动态判断当前 Look 控制器是否为鼠标，从而分别处理 2D/3D 旋转与摇杆瞄准。

- **丰富的动作接口**  
  提供奔跑（Run）、下蹲（Crouch）、俯冲（Dive）、滑翔（Glide）、踩踏（Stomp）、旋转（Spin）、拿取/丢弃（PickAndDrop）等一键式接口，覆盖大多数动作玩法。

- **清晰简洁的调用方式**  
  通过 `WasPressedThisFrame()`、`WasReleasedThisFrame()`、`IsPressed()` 等直观接口，能在任意逻辑层（如角色控制器、AI、UI 系统）中快速获取输入状态。



#### 2.2 数据stats模块 轻松配置

这个 `PlayerStats` 模块（基于泛型 `EntityStats<T>`）是一个**基于 ScriptableObject 的角色参数配置系统**，它在游戏开发中具有极高的灵活性和实用性

 **ScriptableObject 架构 – 数据驱动设计**

- 所有数值参数（速度、跳跃力、冷却时间等）独立于代码逻辑存在于 `ScriptableObject`，支持**设计师直接调参**，无需编译或改动逻辑代码，极大提升工作流效率。
- 多个角色或模式（如敌人、载具、玩家）可复用泛型基类 `EntityStats<T>`，保证结构一致，扩展性强。

 **参数分组清晰，易于维护**

- 使用 `[Header]` 属性清晰划分模块（如 Jump Stats、Glide Stats、Wall Drag Stats 等），便于在 Inspector 中查找、调试和组织。
- 分区结构即文档化，无需额外注释即可一目了然每个参数的作用与归属。

 **支持动态调参与热更新**

- 可在运行时动态读取或替换 `PlayerStats`，可用于：
  - 难度调节、Buff/Debuff 应用  
  - 自定义角色系统（如玩家选择不同风格角色）  
  - 在线更新数据，不需打包代码

 **高可扩展性与兼容性**

- 泛型继承设计 (`EntityStats<T>`) 便于未来扩展为通用敌人/角色/AI 配置系统；

- 可以轻松与状态机、行为树、角色控制器等系统集成，作为底层数据支持。

  

#### 2.3 状态机模块

基于泛型与事件驱动的状态机模块 `EntityStateManager<T>` 

1. **高度泛型化、强类型安全**  

- 通过 `EntityStateManager<T>` 与 `EntityState<T>` 泛型约束，确保状态机只能与指定类型的实体（`T : Entity<T>`）绑定，避免在运行时出错。  
- 使用 `Dictionary<Type, EntityState<T>>` 快速定位状态实例，支持按类型直接切换，API 简洁且不易出错。

2. **一体化初始化与状态管理**  

- `InitializeStates()` 自动从子类提供的 `GetStateList()` 中收集所有状态实例，并构建状态字典；首个状态自动设为 `current`，省去手动注册与初始切换的麻烦。  
- `InitializeEntity()` 在 `Start()` 时自动获取并缓存实体组件，解耦获取流程。

3. **标准化的生命周期回调**  

- 每个状态继承自 `EntityState<T>`，统一实现 `Enter()`、`Exit()`、`Step()`、`OnContact()` 等方法，保证状态逻辑模块化、职责单一。  
- `Step()` 与 `OnContact()` 方法在主驱动中调用，且都内置 `Time.timeScale > 0` 判断，方便全局暂停控制。

4. **事件驱动的状态通知**  

- `EntityStateManagerEvents`（假定封装 `onEnter(Type)`、`onExit(Type)`、`onChange()` 等委托）能让外部系统（动画、音效、UI 反馈等）轻松响应状态流转，耦合度低。  
- 只关心自己要监听的事件，不需要内部硬编码各种回调，响应机制灵活可插拔。

5. **便捷的状态切换接口**  

- 支持两种调用方式：  
  - `Change<TState>()`：通过泛型参数直接切换到指定类型状态；  
  - `Change(EntityState<T> to)`：通过状态实例切换。  
- 自动处理前一个状态的 `Exit()` 与新状态的 `Enter()`，并维护 `last`、`current` 引用与索引。



#### 2.4 Entity实体在Update的整体更新逻辑

`Entity` 的 `Update()` 逻辑作为整个实体系统的心跳中枢，其结构设计非常清晰、模块化，并具备极强的扩展性和项目适应性。以下是它的核心 **设计优势概述**：

<img src="/Users/qqhang/Downloads/ChatGPT Image 2025年4月26日 15_17_05.png" alt="ChatGPT Image 2025年4月26日 15_17_05" style="zoom:33%;" />

1. **模块化更新结构：职责清晰**

```csharp
HandleState();
HandleController();
HandleGround();
HandleContacts();
HandleSpline();
OnUpdate();
```

每一项功能（状态机、移动控制、地面检测、接触处理、轨道处理、自定义扩展）被拆分为独立方法，让逻辑 **解耦明确，易维护、易调试**，避免代码耦合和巨型函数的维护地狱。

---

2. **通用性与复用性强**

- 该 `Entity<T>` 设计为 **模板基类**，可被任意实体类继承并复用核心功能。
- 如 `Player`, `Enemy`, `NPC` 等只需继承并重写必要函数，即可使用通用的移动检测、轨道系统、接触系统等。

---

3. **地面逻辑鲁棒健壮**

```csharp
HandleGround()
```

- 采用 `SphereCast` + 地面角度判定 + step 判断组合，既可应对平地、斜坡，又能有效区分高台/悬崖等。
- 落地触发事件 + 滑坡处理 + step下方探测，兼顾真实性与系统响应度。

---

4. **支持“滑轨”交互系统**

```csharp
HandleSpline()
```

- 动态切换轨道状态，进入/退出轨道状态自动触发事件，适合轨道类玩法（如滑索、磁悬浮、滑杆）。

---

5. **自定义扩展钩子**

```csharp
OnUpdate()
```

- 虚函数提供继承者自定义逻辑的“插槽”，子类无需重写 `Update` 整体，只需重写特定行为，**遵循开放封闭原则**。

---

7. **支持可移动平台的稳定移动**

```csharp
HandleController()
```

- 检测是否在 `Platform` 上，自动叠加平台的 `moveDelta`，确保乘坐平台时移动平滑无跳帧。

---

8. **对可交互物体产生(物理)效果**

```csharp
HandleContacts()
```

- 对于实现了`IEntityContact`的接口调用其`OnEntityContact`方法
- 对于不是trigger且有`rigidbody`的组件对其施加物理推力





#### 2.5 声音模块

1. 事件驱动（Observer 模式）

- 通过 `playerEvents`、`entityEvents`、`LevelPauser.Instance.onPause/onUnpause` 等 UnityEvent，实现声音播放与游戏逻辑的彻底解耦。  
- Audio 模块只需要订阅感兴趣的事件，无需在各个动作中手动调用播放接口，大幅降低耦合度，便于维护和拓展。

2. 状态感知与全局暂停支持

- 借助 `LevelPauser` 单例的 `onPause`/`onUnpause` 事件，在游戏暂停时同时暂停/恢复所有音源，玩家体验更连贯。  
- “全局暂停”不再只停动画或物理，连同音效一起停下，提升沉浸感。

> **音效模块设计与实现**  
>
> - 基于 Observer 模式订阅角色和系统事件，实现声音播放与游戏逻辑彻底解耦。  
> - 遵循单一职责和开闭原则，通过 `protected virtual` 接口轻松扩展自定义音效策略（随机、序列、3D 音效等）。  
> - 多音源独立管理：主音源用于一时性音效，独立音源用于循环轨道滑行音效；支持全局暂停时自动暂停/恢复。  
> - 动态组件获取 & 生命周期统一管控，保证模块即插即用，无需额外手动挂载依赖。

3. `PlayerFootSteps` 模块

   - **基于距离触发**：通过检测角色水平位移（`stepOffset` 阈值），精准控制脚步声播放频率，避免乱放、过密。

   - **表面材质感知（Surface Detection）**：根据地面 `Tag` 动态切换不同材质的脚步声和落地声（如草地、石头、泥地），提升沉浸感。

   - **数据驱动（Data-Driven）**：使用 `Surface` 配置数组，可通过 Inspector 快速添加、修改地形对应的音效，无需改代码，扩展方便。

   - **事件驱动播放（Observer模式）**：通过订阅 `OnGroundEnter` 事件，在落地瞬间自动播放落地音效，逻辑与角色系统解耦。

     


### 3.持久化保存架构

通过单例模式设计Game类, GameSaver类, GameLoader类来控制游戏中的数据存储, 读取以及场景的异步加载, 支持json, xml和二进制的存储格式

通过MVC的设计思想, 使用GameData和LevelData类作为model类存储数据, 使用Game类和GameLevel类作为controller层管理数据与实际游戏的交互



### 4.关卡管理与UI框架

通过`LevelController`类作为枢纽, 掌管`LevelPauser`, `LevelRespawner`, `LevelFinisher`, `LevelScore`类,  来控制整个游戏的关卡暂停, 重启, 存档

游戏中的ui模块使用UGUI进行开发, 通过利用基本元素致力于打造出美观, 易用高效的ui系统, 通过统一的事件系统联通角色数据和UI HUD的更新



### 4.场景可交互物体框架

通过player的`EntityHitBox`动态管理开启碰撞体, 通过`EntityStateManagerListener`类监听来自状态机的事件, 根据不同状态动态启动碰撞体, 并且动态配置不同碰撞体启动时带来的效果(如是否能伤害, 是否能破坏, 是否回弹等)

与角色可交互的物体实现`IEntityContact`接口处理相应的逻辑, 例如:

​	可以按压的`Panel`类支持动态配置(是否需要是Player, 是否需要重锤跳…….), 并且通过策略模式结合枚举来实现开关方式;

​	`ItemBox`类支持动态配置在Player从下往上顶时弹出金币或爱心等奖励物品, 并且通过使用**对象池**从池里拿、放，极大减少 GC 和性能波动。

使用组件式设计思路, 设计了Collectable, Breakable, Pickable等交互属性类, 需要与角色交互的物体只需附上这些类即可动态添加功能



### 5.图形学

通过手写的屏幕后处理实现屏幕Bloom调节和基础颜色, 对比度, 饱和度条件

通过控制顶点着色器的范围内坐标裁切和缩放实现可生长的藤蔓植物

通过根据时间变换uv坐标来实现游泳池水的扰动, 利用Shader Graph实现简单的燃烧溶解特效
