# Fraktal Framework

A lightweight dependency injection framework for Unity, designed for rapid development and seamless component communication.

## Overview

Fraktal Framework simplifies Unity development by providing automatic dependency injection with minimal boilerplate code. Perfect for game jams, rapid prototyping, and single-player game development where clean architecture and fast iteration are essential.

## Key Features

- **Zero Boilerplate** – Automatic dependency resolution with simple attributes
- **Multiple Injection Strategies** – Flexible dependency discovery options
- **Interface Support** – Built-in support for interface-based dependencies
- **Extensible Pipeline** – Customizable injection pipeline with step-based architecture
- **Unity Integration** – Native Unity editor tools and inspector support
- **Comprehensive Documentation** – Well-documented API for easy adoption

---

## Quick Start

### Minimal Example
```csharp
public class PlayerController : FraktalBehaviour
{
    [AnyDependency] private InventoryManager inventory;

    void Start() => inventory.PrintStatus();
}
```

When you run injection, `inventory` will be automatically populated with the scene's `InventoryManager` instance.

---

### Full Example with All Attributes

```csharp
public class PlayerController : FraktalBehaviour
{
    [AutoDependency] // Finds from the same GameObject
    private IDamageable damageable;

    [AnyDependency] // Searches entire scene
    private InventoryManager inventorySystem;

    [ChildrenDependency] // Searches child GameObjects
    private IGroundDetector groundDetector;

    [Dependency] // Manual assignment via Inspector
    private IInteractor interactor;

    // No boilerplate initialization required
}
```

---

## Dependency Attributes

| Attribute              | Scope              | Description                             |
| ---------------------- | ------------------ | --------------------------------------- |
| `[AutoDependency]`     | Same GameObject    | Finds components on the same GameObject |
| `[AnyDependency]`      | Scene-wide         | Finds components anywhere in the scene  |
| `[ChildrenDependency]` | Child GameObjects  | Finds components in child GameObjects   |
| `[Dependency]`         | Inspector-assigned | Explicit reference via Unity Inspector  |

---

## Running Injection

1. Navigate to **Tools → Fraktal Framework → Inject**.
2. Select:
    - **Pipeline Builder** – Implementation of `IFactory<int, InjectionPipeline>`
    - **Context Builder** – Implementation of `IFactory<InjectionContext>`
3. Click **Inject** to populate all eligible dependencies in the scene.

> ⚡ Fraktal will display real-time feedback about successful and failed injections.

---

## Advanced Usage & Customization

Fraktal is highly extensible. You can define your own strategies, pipeline steps, pipeline builders, and context builders.

### 1. Custom Strategy Types

Strategies define **how a dependency is located**.

```csharp
public class CustomStrategy : IFieldStrategy
{
    public bool Process(UnityEngine.Object obj, IField field, InjectionContext ctx)
    {
        if (obj is GameObject go)
        {
            if (go.CompareTag("Injectable"))
            {
                field.SetValue(go);
                return true;
            }
            return false;
        }

        var comp = (Component)obj;
        if (comp.gameObject.CompareTag("Injectable"))
        {
            field.SetValue(obj);
            return true;
        }

        return false;
    }
}
```

> **Note:** Simply implementing `IFieldStrategy` automatically registers your strategy — no extra setup required.

### 2. Pipeline Customization

The injection pipeline is a sequence of **steps** that transform or consume the `InjectionContext`.

```csharp
public class ValidationStep : IPipelineStep<InjectionContext>
{
    public InjectionContext Process(InjectionContext input)
    {
        if (!input.Services.Get<IEmptyFieldsService>(out var service))
        {
            Debug.LogWarning("Validation service not found");
            return input;
        }

        Debug.Log($"Processing {service.FieldCount} uninitialized fields");
        return input;
    }
}
```

### 3. Custom Pipeline Builder

Steps don't get registered automatically — you need a custom **pipeline builder** to control the injection process.

```csharp
public class CustomPipelineBuilder : IFactory<int, InjectionPipeline>
{
    public InjectionPipeline Create(int phase)
    {
        var pipeline = new InjectionPipeline();
        pipeline.AddStep(new ValidationStep());
        pipeline.AddStep(new DefaultInjectionStep());
        return pipeline;
    }
}
```

### 4. Custom Context Builder

The context builder configures the **services and data** available during injection.

```csharp
public class CustomContextBuilder : IFactory<IjectionContext>
{
    public InjectionContext Create()
    {
        var context = new InjectionContext();

        // Example: Register a service used by pipeline steps
        context.Services.Register<IEmptyFieldsService>(new EmptyFieldsService());

        return context;
    }
}
```

### 5. Putting It All Together

When you select **Tools → Fraktal Framework → Inject**, the menu allows you to:

- **Pipeline Builder**: Choose any type implementing `IFactory<int, InjectionPipeline>`
- **Context Builder**: Choose any type implementing `IFactory<InjectionContext>`

Click **Inject** and Fraktal will run your custom pipeline using your custom context.

---

## Use Cases

- **Game Jams** – Rapid development with minimal setup
- **Prototyping** – Iterate without repetitive initialization code
- **Single-Player Games** – Keep architecture clean and maintainable
- **Educational Projects** – Learn and teach DI patterns in Unity

---

## Compatibility

**Confirmed Working:**
- Unity 6000.1.11f1 with .NET Framework

**Likely Compatible:**
- Unity 2021.2+ (untested - please report results)

**Known Issues:**
- Requires .NET Framework API compatibility level
- Will not work with .NET Standard 2.0

> 💡 **Setup Instructions**: Set **Edit → Project Settings → Player → Other Settings → Api Compatibility Level** to **.NET Framework**

---

## Installation

1. Open Unity's **Package Manager**.
2. Click **+** → *Install package from Git URL…*
3. Paste:

```
https://github.com/fraktal-studio/fraktal-framework.git
```

---

## Community & Support

- **Discord**: [Join our community](https://discord.gg/hf8egzSW29)
- **Issues**: [Report bugs & request features](https://github.com/fraktal-studio/fraktal-framework/issues)

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md)

---

## License

[MIT License](LICENSE)