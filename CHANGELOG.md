# Changelog

All notable changes to Fraktal Framework will be documented here.

### Added
- Initial project setup and folder structure
- Core dependency injection pipeline and attributes
- Editor integration and custom drawers
- Basic services and field management

### Changed
- N/A

### Fixed
- N/A

---

## [1.0.0] - 2025-08-10

### Added
- First stable release of Fraktal Framework
- Modular DI with injection strategies and pipeline steps
- Editor tooling for injection debugging and settings
- Support for Unity 2021.3 LTS and later

## [1.1.0] - 2025-08-22

### Added
- Prefab support
- Custom serialization logic instead of using OdinSerializer
- Automatic dependency resolution that tracks editor changes

### Changed
- Removed unmaintainable XML docs
- Some optimizations in injection logic

### Fixed
- Prefab overrides don’t get serialized by Unity