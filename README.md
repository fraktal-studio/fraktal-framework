# Fraktal Framework

A lightweight dependency injection framework for Unity, designed for rapid development and seamless component communication.

## Overview

Fraktal Framework simplifies Unity development by providing automatic dependency injection with minimal boilerplate code. Perfect for game jams, rapid prototyping, and single-player game development where clean architecture and fast iteration are essential.

## Key Features

- **Zero Boilerplate**: Automatic dependency resolution with simple attributes
- **Multiple Injection Strategies**: Flexible dependency discovery options
- **Interface Support**: Built-in support for interface-based dependencies
- **Extensible Pipeline**: Customizable injection pipeline with step-based architecture
- **Unity Integration**: Native Unity editor tools and inspector support
- **Comprehensive Documentation**: Well-documented API for easy adoption

## Quick Start

### Basic Usage

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
    
    // Your component logic here - no initialization boilerplate needed
}
```

### Dependency Injection Process

1. Navigate to **Tools → Fraktal Framework → Inject**
2. Select an implementation of `IFactory<int, InjectionPipeline>` for pipeline configuration
3. Choose an implementation of `IFactory<InjectionContext>` for context building
4. Click **Inject**

The system provides real-time feedback showing which dependencies were successfully injected and which failed.

## Advanced Features

### Custom Pipeline Steps

Extend the injection pipeline with custom processing steps:

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

        Debug.Log($"Processing {service.FieldCount} fields");
        return input;
    }
}
```

## Dependency Attributes

| Attribute | Scope | Description |
|-----------|-------|-------------|
| `[AutoDependency]` | Same GameObject | Automatically finds components on the same GameObject |
| `[AnyDependency]` | Scene-wide | Searches for components throughout the entire scene |
| `[ChildrenDependency]` | Child GameObjects | Looks for components in child GameObjects |
| `[Dependency]` | Manual | Requires manual assignment through Unity Inspector |

## Use Cases

- **Game Jams**: Rapid development with minimal setup overhead
- **Prototyping**: Quick iteration without complex initialization code
- **Single-Player Games**: Clean architecture for manageable codebases
- **Educational Projects**: Learn dependency injection patterns in Unity

## Requirements

- Unity 6000.1.11f1 (tested version)
- .NET Standard 2.1 compatible

> **Note**: While only tested on Unity 6000.1.11f1, the framework may work on other Unity versions. Community testing and feedback on compatibility is welcomed.

## Installation

1. Open the package manager
2. Click the `+` button on top left
3. Click `install package from git url`
4. Paste `https://github.com/fraktal-studio/fraktal-framework.git`

## Community & Support

- **Discord**: Join our community for real-time support, discussions, and updates: [Join Discord Server](https://discord.gg/bM9Kp8wNfw)
- **Issues**: Report bugs and request features on our [issue tracker](https://github.com/fraktal-studio/fraktal-framework/issues)

## Contributing

[Contributing guidelines would go here](CONTRIBUTING.md)

## License

[License information](LICENSE)