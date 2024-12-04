<div align="center">

[![Develop Build Status](https://github.com/realdegrees/ur-game-engineering/actions/workflows/build.yml/badge.svg?branch=develop)](https://github.com/realdegrees/ur-game-engineering/actions/workflows/build.yml?query=branch%3Adevelop)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3.53f1-blue.svg)](https://unity3d.com/get-unity/download)
[![GitHub issues](https://img.shields.io/github/issues/realdegrees/ur-game-engineering)](https://github.com/realdegrees/ur-game-engineering/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/realdegrees/ur-game-engineering)](https://github.com/realdegrees/ur-game-engineering/pulls)
[![GitHub repo size](https://img.shields.io/github/repo-size/realdegrees/ur-game-engineering)](https://github.com/realdegrees/ur-game-engineering)

[![PR Build Status](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)

[![Scene Conflict Stats](https://unity-badges.realdegrees.dev/scene-changes/realdegrees/ur-game-engineering?label=Scene%20Conflict%20Status)](https://github.com/realdegrees/unity-badges)

# 🎮 Game Engineering Unity Project

## 📖 Table of Contents

| Section                                                    | Description                              |
| ---------------------------------------------------------- | ---------------------------------------- |
| [Description](#-description)                               | Overview of the project                  |
| [Getting Started](#-getting-started)                       | Steps to set up the project locally      |
| [Branching Strategy](#-branching-strategy)                 | Guidelines for branching and merging     |
| [Working with Unity and Git](#-working-with-unity-and-git) | Best practices for Unity projects in Git |
| [Contribution Guidelines](#-contribution-guidelines)       | Rules and tips for contributing          |
| [Troubleshooting](#-troubleshooting)                       | Solutions for common issues              |

---

</div>

## 📝 Description

This repository contains the Unity project for a 2D game, developed as part of the **Game Engineering course at the University of Regensburg**. It’s a collaborative effort with guidelines for setup, contributions, and special considerations when working with Unity and Git.

👉 **Please read this README before collaborating.**

---

## 🚀 Getting Started

Follow these instructions to set up the project on your local machine.

### Prerequisites

| Requirement | Version / Link                                                                                              |
| ----------- | ----------------------------------------------------------------------------------------------------------- |
| Unity       | [2022.3.53f1](https://unity3d.com/get-unity/download) _(Any 2022.3.\* should work)_                         |
| Git         | [Latest version](https://git-scm.com/downloads)                                                             |
| Editor      | Use any editor. Recommended: [VSCode](https://code.visualstudio.com/) or Visual Studio (bundled with Unity) |

---

### 🛠️ Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/realdegrees/ur-game-engineering.git
   ```

   ```bash
   git clone git@github.com:realdegrees/ur-game-engineering.git
   ```

   [Follow these instructions for GitHub Desktop.](https://docs.github.com/en/desktop/adding-and-cloning-repositories/cloning-a-repository-from-github-to-github-desktop)

2. **Navigate to the Project Directory**

   ```bash
   cd ur-game-engineering
   ```

3. **Open the Project in Unity**

   - Open Unity Hub.
   - Click **"Add" -> "Add project from disk"** and select the project directory.
   - Open the project.

4. **Start a Feature Branch**
   Refer to the [Branching Strategy](#-branching-strategy) section to create a new branch.

---

### 🛠️ Sample Scenes

To help you get started quickly, the project includes sample scenes that demonstrate various features. These scenes can be found in the following directory:

**`Assets/Scenes/Samples`**

| Scene        | Description                                          |
| ------------ | ---------------------------------------------------- |
| **Input**    | Examples of player input handling and key bindings.  |
| **Camera**   | Demonstrates camera movement and transitions.        |
| **Movement** | Showcases player movement mechanics.                 |
| **Tilemaps** | Includes examples of creating and managing tilemaps. |

Open these scenes in Unity to explore their functionality and use them as a reference for your development.

---

### Ignore Playground Scenes

- Name personal test scenes with `-dev` (e.g., `lighting-scene-dev.unity`) to exclude them via `.gitignore`.
- Use these scenes to prototype and test before applying your changes to game scenes.

## 🌲 Branching Strategy

| Strategy           | Description                                                               |
| ------------------ | ------------------------------------------------------------------------- |
| Protected Branches | Direct pushes to `develop` and `main` are restricted.                     |
| Feature Branches   | Work on new features/fixes in a branch created from `develop`.            |
| Pull Requests      | Merge your feature branch into `develop` via a Pull Request after review. |

### Creating a Feature Branch

```bash
git checkout develop
```

```bash
git pull
```

```bash
git checkout -b feature/your-feature-name
```

### 🔎 Review Process

- Describe your changes briefly in the PR so others can understand your changes.
- PRs _should_ be reviewed by **at least 1 team member**.
- Address feedback before requesting a merge.

---

## 🛠️ Working with Unity and Git

Unity projects require specific version control practices.

### Scene Management

| Tip                   | Explanation                                                         |
| --------------------- | ------------------------------------------------------------------- |
| **Merging Scenes**    | Scene files (`.unity`) are hard to merge. Avoid simultaneous edits. |
| **Rebasing**          | Frequently rebase your branch onto develop to avoid conflicts       |
| **Exclusive Editing** | Only one person should work on a scene at a time.                   |

### ⚠️ Handling Scene Conflicts

- **Do not attempt to merge scene files.**

  - If a conflict occurs, coordinate with the team to resolve it.
  - Overwrite one version and reapply the other’s changes later.

- **Regularly pull changes** to avoid conflicts.

  ```bash
  git pull origin develop
  ```

---

## 🤝 Contribution Guidelines

### Commit Messages

Use clear, descriptive commit messages.  
**Examples**:

- feat: adds double jump
- fix: adjusts player speed to 30
- refactor: reorganizes player input system

### Commit Types

| Type       | Description                                               |
| ---------- | --------------------------------------------------------- |
| `feat`     | New feature.                                              |
| `fix`      | Bug fix.                                                  |
| `docs`     | Documentation changes.                                    |
| `style`    | Code style changes (no functional changes).               |
| `refactor` | Code restructuring.                                       |
| `test`     | Adding tests.                                             |
| `chore`    | Build tasks, config changes (no production code changes). |

---

## 🛠️ Troubleshooting

### Git Clone or Pull Failed

If you encounter authentication issues, use the GitHub CLI:

- [Install GitHub CLI](https://cli.github.com/)

### .NET SDK Missing

Errors about the .NET SDK may occur, especially in VSCode. Install the required SDK:

- [Download .NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)

---
