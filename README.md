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
- **Auto-Injection** – Automatic dependency resolution on hierarchy changes
- **Visual Debugging** – Editor windows showing injection results and failures

---

## Quick Start

### Minimal Example
```csharp
public class PlayerController : FraktalBehavior
{
    [ByAny] private InventoryManager inventory;

    void Start() => inventory.PrintStatus();
}
```

When you run injection, `inventory` will be automatically populated with the scene's `InventoryManager` instance.

---

### Full Example with All Attributes

```csharp
public class PlayerController : FraktalBehavior
{
    [BySelf] // Finds from the same GameObject
    private IDamageable damageable;

    [ByAny] // Searches entire scene
    private InventoryManager inventorySystem;

    [ByChild] // Searches child GameObjects
    private IGroundDetector groundDetector;

    [ByTag("Player")] // Finds by GameObject tag
    private PlayerController otherPlayer;

    [Dependency] // Manual assignment via Inspector
    private IInteractor interactor;

    // No boilerplate initialization required
}
```

---

## Dependency Attributes

| Attribute          | Scope              | Description                                |
| ------------------ | ------------------ | ------------------------------------------ |
| `[BySelf]`         | Same GameObject    | Finds components on the same GameObject    |
| `[ByAny]`          | Scene-wide         | Finds components anywhere in the scene     |
| `[ByChild]`        | Child GameObjects  | Finds components in child GameObjects      |
| `[ByTag("tag")]`   | Tagged objects     | Finds objects with specified tag           |
| `[Dependency]`     | Inspector-assigned | Explicit reference via Unity Inspector     |

---

## Running Injection

### Manual Injection
1. Navigate to **Tools → Fraktal Framework → Injection**.
2. Configure:
   - **Injection Pipeline Builder** – Configures the injection process
   - **Injection Context Builder** – Sets up services and context
3. Click **Inject** to populate all eligible dependencies in the scene.

### Auto-Injection
Enable automatic injection in **Project Settings → Fraktal**:
- **Automatically Inject On Changes** – Runs injection when hierarchy changes
- **Show Results** – Displays injection results window

> ⚡ Fraktal displays real-time feedback about successful and failed injections.

---

## Advanced Usage & Customization

Fraktal is highly extensible. You can define your own strategies, pipeline steps, pipeline builders, and context builders.

### 1. Custom Strategy Types

Strategies define **how a dependency is located**.

```csharp
public class CustomStrategy : IFieldStrategy
{
    public bool Process(UnityEngine.Object obj, IField field, 
                       InjectionContext ctx, UnityEngine.Object instance)
    {
        if (obj is GameObject go && go.CompareTag("Injectable"))
        {
            if (field.IsAssignable(go))
            {
                field.SetValue(go, instance);
                return true;
            }
        }

        if (obj is Component comp && comp.gameObject.CompareTag("Injectable"))
        {
            if (field.IsAssignable(obj))
            {
                field.SetValue(obj, instance);
                return true;
            }
        }

        return false;
    }
}
```

> **Note:** Create a custom attribute extending `AutoDependencyAttribute` to use your strategy.

### 2. Pipeline Customization

The injection pipeline is a sequence of **steps** that process the `InjectionContext`.

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

        var fields = service.GetFields();
        Debug.Log($"Processing {fields.Count} objects with unresolved fields");
        return input;
    }
}
```

### 3. Custom Pipeline Builder

Pipeline builders control the injection process by configuring pipeline steps.

```csharp
public class CustomPipelineBuilder : IInjectionPipelineFactory
{
    public InjectionPipeline Create(int phase)
    {
        var pipeline = new InjectionPipeline();
        
        pipeline.Add(new TrackHierarchyStep());
        
        if (phase == 0)
        {
            pipeline.Add(new CollectFieldStep());
            pipeline.Add(new ValidationStep()); // Custom step
        }
        
        pipeline.Add(new ProcessFieldStep());
        pipeline.Add(new ApplySavesStep());
        
        return pipeline;
    }
}
```

### 4. Custom Context Builder

The context builder configures the **services and data** available during injection.

```csharp
public class CustomContextBuilder : IInjectionContextFactory
{
    public InjectionContext Create()
    {
        var context = new InjectionContext();
        
        // Register required services
        context.Services.Register<IFieldFactory>(new ReflectionFieldFactory());
        context.Services.Register<IHierarchyTracker>(new HashSetHierarchyTracker());
        context.Services.Register<IFailedFieldsService>(new FailedFieldsService());
        context.Services.Register<IChangesTracker>(new ChangesTracker());
        context.Services.Register<IEmptyFieldsService>(new EmptyFieldsService());
        context.Services.Register<ISucceededFieldsService>(new SucceededFieldsService());
        
        // Add custom services
        context.Services.Register<IMyCustomService>(new MyCustomService());
        
        return context;
    }
}
```

### 5. Using Custom Components

Configure your custom builders in the injection window or set them as defaults in **Project Settings → Fraktal**.

---

## Architecture Overview

### Pipeline Architecture
The framework uses a configurable pipeline with these standard steps:

1. **TrackHierarchyStep** – Manages object hierarchy state
2. **CollectFieldStep** – Discovers dependency fields via reflection (phase 0 only)
3. **ProcessFieldStep** – Resolves dependencies using strategies
4. **ApplySavesStep** – Persists changes to Unity assets

### Service System
Core services manage injection state:
- `IEmptyFieldsService` – Tracks unresolved fields
- `ISucceededFieldsService` – Tracks successful injections
- `IFailedFieldsService` – Tracks failed injections
- `IChangesTracker` – Manages Unity object modifications

### Custom Serialization
The framework includes custom serialization to handle Unity's limitations with dependency references, ensuring proper persistence across editor sessions.

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
- Unity 2021.3+ (based on package requirements)

**Known Issues:**
- Requires .NET Framework API compatibility level
- Will not work with .NET Standard 2.0

> 💡 **Setup Instructions**: Set **Edit → Project Settings → Player → Other Settings → Api Compatibility Level** to **.NET Framework**

---

## Installation

1. Open Unity's **Package Manager**.
2. Click **+** → *Install package from Git URL…*
3. Paste: `https://github.com/fraktal-studio/design-patterns.git`
4. Click Install
5. Click **+** → *Install package from Git URL…* 
6. Paste: `https://github.com/fraktal-studio/fraktal-framework.git`
7. Click Install

---

## Community & Support

- **Discord**: [Join our community](https://discord.gg/hf8egzSW29)
- **Issues**: [Report bugs & request features](https://github.com/fraktal-studio/fraktal-framework/issues)

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on:
- Code standards and documentation requirements
- Development setup and testing
- Pull request process
- Community guidelines

---

## License

[MIT License](LICENSE)