# Changelog

## [1.1.6] - 2026-01-09

### Added
- **DEvent**:
  - Support Color serialization

## [1.1.5] - 2025-11-21

### Added
- **BlockUI**:
  - Added InputBehaviourState.
  - Added InputBehaviourChanger.
  - Added IBlockingSystemChecker.

## [1.1.4] - 2025-08-28

### Fixed
- **DEvent**:
  - Fixed static extension methods
- **BlockUI**
  - Add child propagation

## [1.1.3] - 2025-04-04

### Added
- **Storage**:
  - ValueChanged event optional OldValue in Dynamic Arguments.

### Fixed
- **Storage**:
  - Multiple ValueChanged event invoke.

## [1.1.2] - 2025-04-03

### Fixed
- **DEvent**:
  - Cant select member with assignable return type.
  - Mismatches between the target and field classes resulted in an Exception.

## [1.1.1] - 2025-04-02

### Fixed
- **Text Formater**:
  - UpdateText(string text) fix get component.

## [1.1.0] - 2025-03-26

### Added
- **DEvent**:
  - Select extensions method for get/set
  - Select fields for get/set.

### Changes
- **DEvent**: total refactor with lose data.