# Contributing to Fraktal Framework

First off, thanks for considering contributing! We welcome contributions of all kinds — bug reports, feature requests, code, documentation improvements, and ideas.

## How to contribute

1. **Fork the repository** and clone it locally.

2. **Create a new branch** for your changes:
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/issue-description
   ```

3. **Make your changes** following our coding standards and guidelines.

4. **Test your changes** thoroughly to ensure they work as expected.

5. **Commit your changes** with clear, descriptive commit messages:
   ```bash
   git commit -m "Add feature: description of what you added"
   # or
   git commit -m "Fix: description of what you fixed"
   ```

6. **Push to your fork** and submit a pull request:
   ```bash
   git push origin your-branch-name
   ```

## Types of contributions

### Bug Reports
- Check existing issues first to avoid duplicates
- Use the bug report template if available
- Include steps to reproduce, expected behavior, and actual behavior
- Provide system information and framework version

### Feature Requests
- Check existing issues and discussions first
- Use the feature request template if available
- Clearly describe the problem you're solving
- Provide examples of how the feature would be used

### Code Contributions
- Start by discussing larger changes in an issue first
- Follow the existing code style and patterns
- Write tests for new functionality
- Update documentation as needed

### Documentation
- Fix typos, improve clarity, or add missing information
- Update examples when APIs change
- Improve code comments and XML documentation

## Development Setup

1. **Clone and build**:
   ```bash
   git clone https://github.com/[username]/fraktal-framework.git
   cd fraktal-framework
   dotnet restore
   dotnet build
   ```

## Coding Standards

### General Guidelines
- Follow C# naming conventions and coding standards
- Use meaningful variable and method names
- Keep methods focused and reasonably sized
- Add comments for complex logic

### XML Documentation
- All public APIs must have XML documentation comments
- Use `<summary>`, `<param>`, `<returns>`, and `<exception>` tags appropriately
- Include code examples in `<example>` tags where helpful

Example:
```csharp
/// <summary>
/// Processes the specified data.
/// </summary>
/// <param name="data">The input data to process.</param>
/// <param name="options">Optional processing parameters.</param>
/// <returns>The processed result.</returns>
/// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
/// <example>
/// <code>
/// var result = processor.Process(inputData, new ProcessingOptions());
/// </code>
/// </example>
public ProcessResult Process(InputData data, ProcessingOptions options = null)
```

## Before submitting a Pull Request (PR), please ensure:

- [ ] Your code compiles without errors and passes all existing tests.
- [ ] You have added or updated XML documentation comments for all **public** classes, methods, and properties, following the style used in the project.
- [ ] Your code follows existing naming conventions and folder structure.
- [ ] You have tested your changes in a relevant example or use case.
- [ ] Your commit messages are clear and descriptive.
- [ ] Your PR description explains what the change does and why.

## Pull Request Process

1. **Create a descriptive PR title** that clearly explains what the change does
2. **Fill out the PR template** with all required information
3. **Link related issues** using "Closes #123" or "Fixes #123"
4. **Request review** from maintainers
5. **Address feedback** promptly and professionally
6. **Keep your branch updated** with the main branch if requested

## Code Review Guidelines

### For Contributors
- Be open to feedback and suggestions
- Respond to comments and questions promptly
- Make requested changes in a timely manner
- Ask questions if feedback isn't clear

### For Reviewers
- Be constructive and respectful in feedback
- Explain the "why" behind suggested changes
- Approve when changes meet project standards
- Test significant changes locally when possible

## Community Guidelines

- Be respectful and inclusive in all interactions
- Follow the project's Code of Conduct
- Help others learn and grow
- Celebrate contributions from all community members

## Getting Help

- **Questions**: Open a discussion or issue
- **Discord**: Join our [Discord Server](https://discord.gg/hf8egzSW29)
- **Documentation**: Check the wiki or docs folder
- **Examples**: Look at the examples folder for usage patterns

Thank you for helping keep Fraktal Framework well-documented and maintainable!