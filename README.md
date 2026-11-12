# Bloom Tales

A 2D Match-3 game built with Unity and C#, featuring an adaptive hint system and heuristic move search. Developed as a bachelor's diploma project on **Match-3 game development with a hint system and a search algorithm**.

## Features

- Three levels: **Morning Dew**, **Golden Garden**, and **Moonlit Magic**.
- An 8×8 board with matches, cascades, obstacles, and level goals.
- Move-limited and timed gameplay.
- Special pieces with row, column, area, and color-clearing effects, including chain activations.
- Manual and automatic hints with visual highlights and short explanations.
- Animations for swaps, falling pieces, appearance, and clearing.
- Music and sound effects with adjustable volume.
- Pause, restart, level selection, and victory/defeat panels.
- Local settings, progress counters, and per-level statistics.

## Hint system

The hint system collects valid moves and evaluates their predicted effects using a heuristic score. Evaluation considers cleared pieces, special pieces, obstacle damage, and progress toward level goals. The recommendation also accounts for whether a move is predicted to complete the remaining goals.

The search evaluates individual moves rather than planning a complete sequence to finish a level. Hints can be requested manually or shown automatically after inactivity. The system adapts the hint presentation to the current context and provides an explanation for the recommended move.

## Getting started

### Requirements

- Unity Hub
- **Unity 2022.3.62f3**
- Git, or a downloaded ZIP of this repository

### Open the project

1. Clone the repository:

   ```bash
   git clone https://github.com/anastasialesiv19-del/bloom-tales.git
   ```

2. Add the cloned `bloom-tales` folder to Unity Hub as an existing project.
3. Open it with Unity **2022.3.62f3** and wait for package resolution and asset import to finish.
4. Open `Assets/Scenes/MainMenuScene.unity`.
5. Press **Play** in the Unity Editor.

The repository contains the Unity project. The `Library` folder is generated locally during import.

## Controls

| Action | Control |
| --- | --- |
| Swap pieces | Click a piece, then click an adjacent piece |
| Cancel selection | Click the selected piece again |
| Request a hint | Click the question-mark button |
| Pause the game | Click the pause button |
| Adjust audio or automatic hints | Open Settings from the main menu or pause panel |

## Project structure

```text
Assets/
├── Art/                    Visual and audio assets
├── Prefabs/                Reusable game objects
├── Scenes/                 Main menu, level selection, and gameplay
├── Scripts/
│   ├── Core/               Game rules, board logic, and hint algorithms
│   └── Match3.Unity/       Unity integration, UI, audio, and playback
└── Tests/
    └── EditMode/           Automated tests
Packages/                   Package dependencies
ProjectSettings/            Unity project configuration
```

The game logic is separated from Unity presentation code. `Core` handles board processing and hint search, while `Match3.Unity` connects that logic to scenes, input, animations, and interface panels.

## Tests

The EditMode suite covers board operations, matching, special effects, cascades, goals, hint selection, and other game logic. It also includes hint-search timing tests.

To run the tests, open the Unity Test Runner, select **EditMode**, and choose **Run All**.

## Saved data

The game stores local JSON files in Unity's `Application.persistentDataPath` directory:

- `player_progress.json` — progress counters.
- `settings.json` — audio volume and automatic hint preferences.
- `level_results.json` — completed game results used for statistics.

These files store counters, preferences, and results; they do not restore an unfinished board.
