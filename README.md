# L-0 Chess Engine
**Edit Access**: Zain Farhan

This Repo is owned and maintained by some LUMS students for the purposes of using it for their Summer Project. This is a Chess Engine build from scratch using the Avalonia UI with the .NET framework.
## Table of Contends

1. [Introduction](#introduction)
2. [To-Do List](#To-Do)
3. [Guidelines](#Guidelines)
4. [Naming Schemes](#Naming-Scheme)
5. [Resources](#Resources)
6. [Permissions](#Permissions)

## Introduction

We will first have to re-create the entire game of chess in Avalonia, then we will try to create a Chess Bot by implementing a Basic Min-Max Algorithm. This is the baseline for our project requirements, once and if we achieve this, we will try to implement more difficult solutions. Like training a Chess Bot using Reinforcement Learning (A type of Machine Learning). These Base and Extra tasks may change over the course of development and can be viewed in the [To-Do List](#To-Do).

Please make sure you have all the required tools installed and ready:
- Framework: .NET 9.0 sdk (I also recommend downloading the .NET 8.0 runtime)
- Library: Avalonia UI 11.3
- Operating System: Windows 10 or newer (Recommended but not required)
- Integrated Development Environment: JetBrains Rider (Recommended but not required)

### MVVM Paradigm

MVVM stands for Model–View–ViewModel. It’s a design pattern used to organize code, especially in apps with a user interface (like mobile or desktop apps). It helps separate the logic of your app (how it works) from the way it looks. For example, let's say we are building a weather app.

MVVM breaks this into 3 parts:

1. **Model:** 
This is your data layer.  It includes the weather data and logic to fetch it from the internet or a database. No UI code here.

2. **View:**
This is the UI — what the user sees and interacts with. Buttons, labels, graphs, etc. It doesn’t know where the data comes from or how it’s processed. It just shows it. So in our case it would just be the weather information.

3. **View-Model:**
This is the middle layer between the View and the Model. It takes raw data from the Model and prepares it for the View. For example, it might convert "23.4°C" into "Warm day" for display. The View-Model also handles UI events, like when the user taps a button to refresh the weather.

## To-Do
The Development Cycles have been divided into various Stages.

### Stage 1
**Deadline:** 19th June 25, 11:59 pm (PST)
- [ ] ~~Main Menu - Ali~~
- [x] Backend
  - [x] Chess Board - Isaac
  - [x] Chess Pieces - Zain
  - [x] Moving Chess Pieces (No Move Validation) - [Reserved - Haya]: Daud
- [x] Frontend
  - [x] Displaying Chess Board - Kalsoom
  - [x] Displaying Chess Pieces - Daud

#### Stage 1.1
- [x] Main Menu - Zain

### Stage 2
**Deadline:** 2 July, 23 59 (PST)

- [ ] ~~Test Cases - Isaac~~
- [X] Interaction for Moves between Frontend and Backend - Daud
- [ ] Backend
  - [X] Sliding Pieces (Rook, Bishop, Queen) (Excluding Special Moves like Castling) - Kalsoom
  - [X] Knight Moves - Zain
  - [X] All Pawn Moves - Daud
  - [X] Starting and running the Game State - Daud
  - [ ] ~~Check Logic (Not CheckMate, just Check) - Kalsoom~~ (Miscommunication)

- [X] Frontend
  - [X] Point-and-Click Action (To Move Pieces) - Zain
  - [X] Addtional Main Menu Options - Isaac

- [ ] ~~Artwork: Main Menu Background - Haya~~

#### Stage 2.1

- [X] Check Logic - Zain
- [X] Bug Fixes and Code Refactor - Zain
- [X] Test Cases (Move Validation, Check, and Checkmate) - Kalsoom

### Stage 3
**Deadline:** 12th July 23:59 PST

- [ ] ~~Checkmate logic - Zain~~
- [X] Integrate PvP and PvE Mode for Main Menu - Zain
- [ ] Castling - Isaac
- [X] Move Validation for Check - Isaac
- [X] Pawn Promotion (Front End and Back End) - Kalsoom
- [X] Game timer & Turn timers - Daud
- [X] Revamp Check Logic - Daud

#### Stage 3.1
**Deadline:** 18th July 23:59 PST

- [X] Checkmate logic - Zain
- [X] Code Cleanup - Daud
- [X] Pawn Promotion Clean Up - Kalsoom
- [X] Castling Clean Up - Isaac
- [ ] AI Practical - @Everyone

#### Stage 3.2
- [X] Bug Fixes - Zain

### Stage 4
**Deadline:** 3rd Aug 23:59 PST

- [X] Evaluation Basic - Isaac
- [X] Evaluation Detailed - Kalsoom
- [ ] MinMax Algorithm - Zain
- [ ] Alpha Beta Pruning + Difficulty - Daud
- [X] Implementing Draw - Daud
#### Stage 4.1
- [X] Bug Fixes - Zain
- [X] MinMax Algorithm - Zain

### Stage 5
**Deadline - Passive Stage:** 18rd Aug 23:59 PST

- [ ] Evaluation Function Sophistication
- [ ] Bug Fixes
  - [X] Pawn Promotion - Zain
  - [X] Board Display - Zain
  - [ ] AI Invalid Moves
- [X] Alpha-Beta Pruning - Zain
- [X] Difficulty Settings - Zain
- [ ] Frontend
  - [X] Font Change - Zain
  - [X] Displaying All Moves - Zain
  - [X] Game Over Screen - Zain
  - [ ] Revamp of Main Game Screen
- [X] Implement Async or Multithreading for AI - Zain
- [X] AI Pawn Promotion Interface - Zain
- [X] Hypothetical Move Functions - Zain
- [ ] Backend
  - [X] Implement Turn Manager - Zain
- [ ] Basic SFX
- [ ] AI Randomization
- [ ] Minor AI Optimizations


### Stage ω
- [ ] A Reinforced Trained Engine
- [ ] Database Integration

## Guidelines

Below are the guidelines for the code of conduct we recommend that you follow, these may be updated or changed throughout development. Though these will not be strictly enforced, we **highly suggest** following them:
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

## Naming Scheme

Please use the following naming conventions when writing and submitting code, changes which do not follow be **may not be accepted**:
- Variable Name: *camelCase*
- Private Fields:  *\_camelCase* with the prefix \_
- Methods/Properties: *PascalCase*
- Classes/Interfaces: *PascalCase*
- Files: *PascalCase*

Though these will not be strictly enforced, the naming convention used must be sensible and integrate well with the existing code base, or it may be rejected.

Additionally, following the official naming scheme would be appreciated but is not required. For additional info, [see here](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)

## Resources

Official Documentation:
- [Microsoft's C#](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Avalonia UI](https://docs.avaloniaui.net/docs/overview/what-is-avalonia)

Video Tutorials:
- [C# for Absolute Beginners](https://youtube.com/playlist?list=PLPV2KyIb3jR4CtEelGPsmPzlvP7ISPYzR&si=kc9i4wZJKezL6RRK)
- [C# Basics](https://youtu.be/GhQdlIFylQ8?si=kuTilL_nES_pctFF)
- [Avalonia UI](https://youtube.com/playlist?list=PLJYo8bcmfTDF6ROxC8QMVw9Zr_3Lx4Lgd&si=lsUY6YDOKVZBhFwv)
- [Avalonia UI Comprehensive](https://youtube.com/playlist?list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&si=f2XPAjseWJGIPJyP)

Chess:
- [How to Play Chess](https://www.chess.com/learn-how-to-play-chess)
- [Forsyth-Edwards Notation (FEN)](https://www.chess.com/terms/fen-chess)
- [Chess Programming WiKi](https://www.chessprogramming.org/Main_Page)
- [Tom Kerrigan’s Simple Chess Program](https://www.tckerrigan.com/Chess/TSCP/)
- [Chess Programming](https://www.chessprogramming.net/)

## Permissions And Roles

- Admin/Owner, CEO: Zain Farhan
- Head Developer, Head Communications, Cult Leader, PTSD (Punctuation Trauma & Syntax Distress) Specialist: Daud Khawaja
- Head Debt Collector, Head Communications: Haya Afareen
- Head Zain Hater, Head Communications, Local Company Extrovert: Kalsoom Nawaz
- Token Non LUMS Student, Head Communications that kalsoom thinks less of: Isaac Ashar
