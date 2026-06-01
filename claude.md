## Architectureal Guide
Please consult Architectural_Guide.md for clarity on on Architecture

## Style Guide
Please consult UI_Skill.md for UI Style Guide

# Test-Driven Development (TDD) Guide

## Scope

The following components **must be developed using TDD**:

### Required TDD Coverage
- **Validators**
- **Repositories**
- **AggregatedDataAccessObjects**

### Conditional TDD Coverage
- **Blazor Services**
  - MUST be TDD’d **if they do NOT call external APIs**
  - Services that call APIs are excluded from mandatory TDD (unless explicitly required)

---

## TDD Process

All development must strictly follow this cycle:

### 1. Write a Failing Test
- Create a unit test **before writing any implementation code**
- Use:
  - **xUnit**
  - **Moq** for mocking dependencies
- Only use **standard xUnit assertions**
- The test must fail initially

---

### 2. Make the Test Pass
- Write the **minimum amount of code required** to make the test pass
- Avoid over-engineering or premature optimisation

---

### 3. Refactor
- Improve:
  - **Readability**
  - **Performance**
- Ensure:
  - All tests remain **green**
  - No behavioural changes are introduced

---

## Rules & Principles

- **No production code without a failing test**
- **Keep tests simple and focused**
- **Mock external dependencies only**
- **Avoid testing implementation details — test behaviour**
- **Refactoring is mandatory, not optional**

---

## Tooling Standards

- Testing Framework: **xUnit**
- Mocking Library: **Moq**
- Assertions: **xUnit built-in assertions only**

---

## Definition of Done

A task is only complete when:
- All required components are developed using TDD
- Tests:
  - Are written first
  - Are passing
  - Cover expected behaviour
- Code has been refactored for clarity and performance
- No unnecessary complexity has been introduced