# Avalonia.GuideControl

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3.6-purple)](https://avaloniaui.net/)
[![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-green)](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)

A powerful Avalonia UI guide control library for creating interactive user onboarding and tutorial experiences.

## Features

- **Smart Mask System** - Complex transparent area creation with geometric boolean operations
- **Customizable Guide Cards** - Fully customizable content display and navigation buttons
- **Flexible Control Targeting** - Precise control positioning based on visual/logic tree paths
- **Async Flow Management** - Modern async programming patterns with validation and cancellation
- **Visual Development Tools** - F1-triggered real-time guide configuration editor
- **AOT Friendly** - Supports .NET AOT compilation with source generators
- **Responsive Design** - Smart position calculation and boundary detection

## Quick Start

### Installation

```bash
# Clone the repository
git clone https://github.com/pchuan98/Avalonia.GuideControl.git
cd avalonia-guide-control
```

### Basic Usage

```csharp
// 1. Create mask to highlight target controls
var mask = new Mask();
mask.HoleItems = [new Hole(new Rect(100, 100, 200, 150))];
mask.Show(targetWindow);

// 2. Use guide manager for complete flow
var manager = new GuideManager(mainWindow);
manager.ValidMethods["user_clicked_button"] = () => buttonClicked;
await manager.TestRunStep(stepId, timeout: 30);

// 3. Integrate development tools (optional)
this.AttachGuideDevTools();  // F1 to activate recording tools
```

## Project Structure

```
Avalonia.GuideControl/
├── Controls/
│   ├── Mask.cs              # Mask control - canvas overlay with transparent areas
│   └── GuideCard.cs         # Guide card - content display with navigation buttons
├── Models/
│   ├── Guide.cs             # Guide flow container
│   ├── GuideStep.cs         # Individual guide step configuration
│   ├── Hole.cs              # Transparent area definition
│   └── ...                  # Other data models
├── AvaloniaUtils/
│   ├── AvaloniaUtils.VisualTree.cs    # Visual tree path operations
│   ├── AvaloniaUtils.LogicTree.cs     # Logic tree path operations
│   └── ...                            # Other utility classes
├── Extensions/
│   ├── CardExtension.cs     # Card position calculation extensions
│   ├── GuideStepExtension.cs # Step conversion extensions
│   └── MaskExtensions.cs    # Mask operation extensions
├── DevTools/                # Visual development tools
└── GuideManager.cs          # Guide flow manager
```

## Core Concepts

### Mask System

Create complex transparent areas using geometric boolean operations:

```csharp
var mask = new Mask();
mask.HoleItems = [
    new Hole(new Rect(100, 100, 200, 100)) { CornerRadius = new CornerRadius(8) },
    new Hole(new Rect(400, 200, 150, 80))  { BorderThickness = 2 }
];
```

### Control Path Targeting

Precisely target UI controls using string paths:

```csharp
// Generate path
var path = targetButton.VisualTreeString(mainWindow);
// Result: "Grid[0]/StackPanel[1]/Button[0]"

// Find control by path
var control = AvaloniaUtils.FromVisualTree(path, mainWindow);
```

### Smart Positioning

Nine-grid position definition with automatic boundary detection:

```csharp
var config = new GuideCardConfig {
    Position = CardPosition.Top | CardPosition.Right,  // Top-right corner
    Offset = new Offset(10, -5),  // Fine-tune offset
    Header = "Step Title",
    Content = "Click this button to continue"
};
```

## Development Tools

### F1 Recording Tool

Integrated visual configuration editor supporting:

- **Alt + Mouse Hover**: Select target controls
- **F4 Key**: Add additional highlight areas
- **Drag & Drop**: Adjust step execution order
- **JSON Import/Export**: Configuration file management

```csharp
// Enable development tools
protected override void OnOpened(EventArgs e) {
    base.OnOpened(e);
    this.AttachGuideDevTools();  // F1 to activate
}
```

## Advanced Usage

### Custom Validation Mechanism

```csharp
var manager = new GuideManager(window);

// Register validation methods
manager.ValidMethods["form_completed"] = () => {
    return !string.IsNullOrEmpty(nameInput.Text) && 
           !string.IsNullOrEmpty(emailInput.Text);
};

// Manually trigger validation
manager.UpdateValid("form_completed");
```

### Async Step Execution

```csharp
var result = await manager.TestRunStep(stepId, timeout: 30);
switch (result) {
    case GuideOperation.Finished:
        // Validation condition met, proceed to next step
        break;
    case GuideOperation.Skip:
        // User chose to skip
        break;
    case GuideOperation.Previous:
        // Go back to previous step
        break;
}
```

## Build & Development

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Avalonia.GuideControl/Avalonia.GuideControl.csproj

# Run demo application
dotnet run --project src/Demo/Demo.csproj

# Run all tests
dotnet test

# Run single test
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

## Tech Stack

- **Target Framework**: .NET 9.0
- **UI Framework**: Avalonia UI 11.3.6
- **MVVM**: CommunityToolkit.Mvvm 8.4.0
- **Property Generation**: DependencyPropertyGenerator 1.5.0
- **Serialization**: System.Text.Json (AOT compatible)
- **Testing**: xUnit + Avalonia.Headless.XUnit
- **Hot Reload**: HotAvalonia 3.0.0

## Architecture Highlights

- **Geometry Calculation Engine**: Uses Avalonia's PathGeometry and boolean operations
- **Coordinate System Management**: Smart coordinate transformation and multi-layer Panel support
- **Async Execution Model**: Based on Task and CancellationToken
- **Extensible Design**: Fluent API based on extension methods
- **Event-Driven**: Flexible validation mechanism and state management
- **Resource Management**: Complete IDisposable implementation

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Avalonia UI](https://avaloniaui.net/) - Powerful cross-platform UI framework
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - Excellent MVVM toolkit
- All contributors and users

---

⭐ If this project helps you, please give it a star!
