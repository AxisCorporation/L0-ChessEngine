# L-0 Chess Engine
**Edit Access**: Zain Farhan

This Repo is owned and maintained by some LUMS students for the purposes of using it for their Summer Project. This is a Chess Engine build from scratch using the Avalonia UI with the .NET framework.
## Table of Contends

1. [Introduction](#introduction)
2. [To-Do List](#To-Do)
3. [Guidelines](#Guidelines)
4. [Naming Schemes](#Naming-Schemes)
5. [Resources](#Resources)
6. [Permissions](#Permissions)
## Introduction

We will first have to re-create the entire game of chess in Avalonia, then we will try to create a Chess Bot by implementing a Min-Max Algorithm. This is the baseline for our project requirements, once and if we achieve this, we will try to implement more difficult solution. Like training a Chess Bot using Reinforcement Learning (A type of Machine Learning). These Base and Extra tasks may change over the course of development and can be viewed in the [To-Do List](#To-Do).

Please make sure you have all the required tools installed and ready:
- Framework: .NET 9.0 sdk (I also recommend downloading the .NET 8.0 runtime)
- Library: Avalonia UI 11.3
- Operating System: Windows 10 or newer (Recommended but not required)
- Integrated Development Environment: JetBrains Rider (Recommended but not required)

---
## To-Do

Base Tasks:
- [ ] Main Menu
- [ ] Basic Chess Implementation
- [ ] A Basic Min Max Algorithm

Extra Tasks:
- [ ] A Reinforced Trained Engine
- [ ] Database Integration (Optional)

---
# Guidelines

Below are the guidelines for the code of conduct we recommend that you follow. Though these will not be strictly enforced, we **highly suggest** following them:
- Contributors must make all of their changes on branches.
- We suggest you push your changes to the repository frequently.
- We suggest you only work on **one** feature per branch.
- We suggest informing a Head Developer (or sending a message in the Group Chat if applicable) before starting work on a new branch.
- Document your code in the repository's Wiki.
- In case of ambiguity in some code's functionality, consult the Wiki first, the user responsible for the code second, and the Head Developer or Admin last. 
- Direct changes to the "main" branches are not permitted.
- Forceful changes to the "main" branch are **only** to be made the Admin.
- Forceful changes to the "main" branch are **only** to be made as a Drastic measure and a last resort.
- Any merge request must be approved by their designated code reviewer or the Admin.
- Any pull request must be approved by a code reviewer and one other contributor.
- All code reviewers must provide a Valid Description for any rejection.
- Avoid making self-explanatory comments:
```
  // Counter for Moves
  int moveCount;                                 X

  // Uses Matrix-Exponentiation to find largest
  // Fibonnaci Number
  long LargestFibNum(double time) {...}          O
```
- Avoid leaving large chunks of code commented out, if you wish to leave a backup of your previous code consider creating a separate file locally.
- Avoid ambiguous names (e.g. `Backup2.txt`).
- Unreasonable requests (e.g. a merge request consisting of more than a 1,000 lines) will not be humoured.

A failure to comply may result in your code be rejected or even being taken out of the development team based on how egregious the non-compliance is.

---
## Naming Scheme

Please use the following naming conventions when writing and submitting code, changes which do not follow be **may not be accepted**:
- Variable Name: *camelCase*
- Private Fields:  *\_camelCase* with the prefix \_
- Methods/Properties: *PascalCase*
- Classes/Interfaces: *PascalCase*
- Files: *PascalCase*

Though these will not be strictly enforced, the naming convention used must be sensible and integrate well with the existing code base or it may be rejected.

Additionally, following the official naming scheme would be appreciate but is not required. For additional info, [see here](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)

---
## Resources

Official Documentation:
- [Microsoft's C#](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Avalonia UI](https://docs.avaloniaui.net/docs/overview/what-is-avalonia)

Video Tutorials:
- [C# for Absolute Beginners](https://youtube.com/playlist?list=PLPV2KyIb3jR4CtEelGPsmPzlvP7ISPYzR&si=kc9i4wZJKezL6RRK)
- [C# Basics](https://youtu.be/GhQdlIFylQ8?si=kuTilL_nES_pctFF)
- [Avalonia UI](https://youtube.com/playlist?list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&si=f2XPAjseWJGIPJyP)

---
## Permissions
- Admin/Owner: Zain Farhan
- Head Developer: Daud