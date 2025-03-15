#### **依赖倒置原则 (Dependency Inversion Principle, DIP) 概念回顾**



依赖倒置原则是 **SOLID** 原则中的 “D”（DIP），其核心思想是：



✅ 高层模块（如游戏控制器、业务逻辑）不应该依赖于低层模块（如数据处理、具体实现）；两者都应该依赖于抽象。

✅ 抽象不应该依赖于具体实现，具体实现应依赖于抽象。



**在 LevelController 中如何体现了依赖倒置原则？**



在 LevelController 的实现中，我们可以看到以下特征：



**1. 依赖于抽象而非具体实现**



LevelController 通过以下属性引用了 LevelFinisher、LevelRespawner、LevelScore、LevelPauser 等对象：

```
protected LevelFinisher m_finisher => LevelFinisher.instance;
protected LevelRespawner m_respawner => LevelRespawner.instance;
protected LevelScore m_score => LevelScore.instance;
protected LevelPauser m_pauser => LevelPauser.instance;
```

✅ 这些属性本质上依赖的都是抽象（LevelFinisher、LevelRespawner 等类的公共 API），而不是其具体实现细节。

✅ LevelController 并不关心 LevelFinisher 的内部是如何完成关卡结束或退出的。它只知道该类提供了 .Finish() 和 .Exit() 方法，并通过这些方法触发对应的逻辑。

✅ 这样，LevelController 只需要调用这些抽象接口，不依赖其具体实现。



------



**2. 控制反转 (Inversion of Control, IoC)**



LevelController 并没有主动创建 LevelFinisher、LevelRespawner 等实例，而是通过单例 (.instance) 获取它们：

```
protected LevelFinisher m_finisher => LevelFinisher.instance;
```

✅ 这种方式避免了 LevelController 直接依赖于具体的构造函数（如 new LevelFinisher()），而是依赖于一个全局的实例入口。

✅ 如果以后 LevelFinisher 的实例化逻辑变更，LevelController 无需修改自身代码，只要 LevelFinisher.instance 的返回值仍符合 LevelFinisher 的定义。

✅ 这种做法符合 DIP 的核心思想：**“高层模块不依赖于低层模块的创建细节”**。



------



**3. 易于扩展和维护**



假设游戏需求发生变化，我们决定将 LevelFinisher 替换为一个新的 AdvancedLevelFinisher（例如添加额外的动画或音效），那么只需修改 LevelFinisher 的单例入口：



**替换前：**

```
public static LevelFinisher instance { get; } = new LevelFinisher();
```

**替换后：**

```
public static LevelFinisher instance { get; } = new AdvancedLevelFinisher();
```

✅ LevelController 无需更改任何代码，因为它只依赖 LevelFinisher 的公共 API，而非具体的实例化细节。

✅ 这种解耦让代码更容易测试、维护和扩展。



------



**3. 实际好处总结**



通过遵循依赖倒置原则，LevelController 带来了以下优势：



**🚀 提升了模块化**

​	•	LevelController 的功能独立于 LevelFinisher、LevelRespawner 等具体实现。

​	•	每个类专注于自己的职责，遵循了 **单一职责原则 (SRP)**。



**🧩 增强了代码的灵活性**

​	•	如果未来需要更换、重构或增强 LevelFinisher 等类，只需调整其内部实现，而无需修改 LevelController。

​	•	例如：将 LevelScore 替换为 AdvancedLevelScore 以增加更多计分规则时，LevelController 不需重写逻辑。



**🧪 提高了可测试性**

​	•	在单元测试时，可以使用**模拟对象 (Mock)**来替代 LevelFinisher、LevelRespawner 等类。

​	•	这样测试 LevelController 时无需依赖实际的游戏场景或复杂的游戏状态。



**🔍 增强了代码的可维护性**

​	•	由于 LevelController 仅依赖公共 API，代码结构更清晰。未来的更新或改动更易于理解和管理。



------



**4. 示例：使用依赖注入进一步优化 (Dependency Injection)**



若希望进一步增强 LevelController 的灵活性，可以引入 **依赖注入 (DI)**，将依赖项作为参数传入。这样可以轻松切换实现版本，特别适用于测试环境。



**示例代码：**

```
public class LevelController : MonoBehaviour
{
    private ILevelFinisher _finisher;
    private ILevelRespawner _respawner;

    public LevelController(ILevelFinisher finisher, ILevelRespawner respawner)
    {
        _finisher = finisher;
        _respawner = respawner;
    }

    public void Finish() => _finisher.Finish();
    public void Restart() => _respawner.Restart();
}
```

在 Unity 中初始化时：

```
void Start()
{
    var controller = new LevelController(new LevelFinisher(), new LevelRespawner());
}
```

✅ 这种方法更符合 DIP 的核心理念，彻底解除了对具体实现的耦合。

✅ 便于在不同环境（如调试模式、正式环境、测试环境）下自由切换实现。



------



**总结**



在 LevelController 中，虽然没有完全采用接口来遵循 DIP（如 ILevelFinisher），但它通过依赖单例实例和只依赖公共 API，已经很好地体现了**依赖倒置原则**。这使得代码更灵活、更易于扩展，并且提高了模块化和测试性，是 Unity 开发中较为优雅的设计模式之一。