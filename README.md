# Game Engineering Unity Project

## Table of Contents

- [Description](#description)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Branching Strategy](#branching-strategy)
- [Working with Unity and Git](#working-with-unity-and-git)
  - [Scene Management](#scene-management)
- [Contribution Guidelines](#contribution-guidelines)

## Description

This repository contains the Unity project for a 2D game, developed as part of the Game Engineering course at University of Regensburg. The project is a collaborative effort, and this README outlines important information regarding project setup, contribution guidelines, and special considerations when working with Unity and Git.

*Please read it before starting to collaborate.*

## Getting Started

These instructions will help you get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- **Unity**: 2022.3.53f1 *(Any 2022.3.* should work)*  
  Ensure you have the correct Unity version installed. You can download Unity Hub from [Unity Download](https://unity3d.com/get-unity/download).
- **Git**: Latest version  
  Download from [Git Downloads](https://git-scm.com/downloads).

### Installation

1. **Clone the Repository**

   http: 
   ```bash
   git clone https://github.com/realdegrees/ur-game-engineering.git
   ```  
   ssh: 
   ```bash
   git clone git@github.com:realdegrees/ur-game-engineering.git
   ```

2. **Navigate to the Project Directory**

   ```bash
   cd ur-game-engineering
   ```

3. **Checkout the `develop` Branch**

   ```bash
   git checkout develop
   ```

4. **Open the Project in Unity**

   - Open Unity Hub.
   - Click on **"Add"** and select the project directory.
   - Open the project.
   - See if everything works

5. **Start a feature branch**

    Follow the [Branching Strategy](#branching-strategy) section below to create a new feature branch.

## Branching Strategy

- **Protected Branches**: To keep the git tree clean, the `develop` and `main` branches are protected. Direct pushes to these branches are restricted. 
- **Feature Branches**: When working on new features or fixes, create a new branch from `develop`.

  ```bash
  git checkout develop
  git pull
  git checkout -b feature/your-feature-name
- **Pull Requests (PRs)**: Once your feature is ready, push your branch, open a [Pull Request](https://github.com/realdegrees/ur-game-engineering/pulls) to merge your branch back into `develop`.


- **Review Process**

  - All PRs *should* be reviewed by at least 1 other teammember.
  - Address any feedback before requesting a merge.


## Working with Unity and Git

Unity projects have specific considerations when used with version control systems like Git.

### Scene Management

- **Merging Scene Files**: Unity scene files (`.unity` files) are extremely hard to merge
- **Exclusive Editing**: To prevent conflicts, **only one person** should work on a scene at any given time.

The  [.gitignore](.gitignore) includes a rule to exclude scenes that end in *"-dev"*.  
You can name your own scenes e.g. `lighting-scene-dev.unity` to exclude them from the repository as a playground to test your code.

### Handling Conflicts

- **Do Not Attempt to Merge Scene Files**

  - If a conflict occurs in a scene file, communicate with the team member involved to decide whose changes to keep.
  - One option is for one person to overwrite the scene file and the other to reapply their changes afterward.

- **Regularly Pull Changes**

  - Frequently pull changes from `develop` to minimize the risk of conflicts.


## Contribution Guidelines

- **Commit Messages**

  - Use clear and descriptive commit messages.
  - **Examples**
    - "feature: adds double jump"
    - "adjusts player speed to 30"
    - "increases boss loot drop chance to 10%"  

You can also add a commit type in front of the message.
  - **Types**:
    - `feat`: New feature
    - `fix`: Bug fix
    - `docs`: Documentation changes
    - `style`: Formatting, missing semi colons, etc.; no code change
    - `refactor`: Refactoring code
    - `test`: Adding tests
    - `chore`: Updating build tasks, package manager configs, etc.; no production code change