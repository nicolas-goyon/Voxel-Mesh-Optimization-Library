# Contributing to Voxel Mesh Optimization Library

Thank you for considering contributing to the Voxel Mesh Optimization Library! We welcome your contributions, whether they're feature requests, bug fixes, documentation improvements, or other enhancements. This document outlines the guidelines for contributing to our project.

---

## Table of Contents

- [Getting Started](#getting-started)
- [How to Report a Bug](#how-to-report-a-bug)
- [How to Suggest a Feature](#how-to-suggest-a-feature)
- [Pull Request Guidelines](#pull-request-guidelines)
- [Coding Standards & Best Practices](#coding-standards--best-practices)
- [Branching & Workflow](#branching--workflow)
- [Code of Conduct](#code-of-conduct)
- [License](#license)

---

## Getting Started

1. **Fork the Repository:**  
   Click the "Fork" button at the top right of the repository page to create your own copy of the project.
   
2. **Clone Your Fork Locally:**  
   ```bash
   git clone https://github.com/your-username/VoxelMeshOptimizerLibrary.git
   cd VoxelMeshOptimizerLibrary
   ```

3. **Set Up the Development Environment:**  
   - Ensure you have the required version of .NET installed.
   - Open the solution file (`VoxelMeshOptimizerLibrary.sln`) in your preferred IDE (e.g., Visual Studio).
   - Build the project to ensure all dependencies are correctly configured.

4. **Read the Documentation:**  
   Familiarize yourself with the [README.md](README.md) and our architectural documents to understand the project structure, core concepts (voxels, chunks, mesh optimization), and design rationale.

---

## How to Report a Bug

1. **Check Existing Issues:**  
   Before filing a new issue, please search the [issues section](https://github.com/your-username/VoxelMeshOptimizerLibrary/issues) to see if the bug has already been reported.

2. **Create a Detailed Bug Report:**  
   Include the following information:
   - **Description:** A clear and concise description of the bug.
   - **Steps to Reproduce:** Detailed steps to reproduce the issue.
   - **Expected Behavior:** What you expected to happen.
   - **Actual Behavior:** What actually happened.
   - **Environment:** Any relevant details such as operating system, .NET version, and configuration.
   - **Screenshots/Logs:** Any relevant output or error messages.

3. **Submit the Issue:**  
   Create a new issue on GitHub with the title prefixed by `[BUG]`.

---

## How to Suggest a Feature

1. **Search for Existing Ideas:**  
   Look through the current issues to ensure your idea has not been proposed already.

2. **Open a Feature Request:**  
   - Provide a detailed description of the proposed feature.
   - Explain the need or problem that the feature addresses.
   - Describe potential implementation ideas or design considerations if possible.
   - Optionally, include examples or mockups if relevant.

3. **Submit the Feature Request:**  
   Create a new issue with the title prefixed by `[FEATURE]` to start a discussion.

---

## Pull Request Guidelines

1. **Fork & Branch:**  
   - Fork the repository and create a new branch for your changes.
   - Use a descriptive branch name (e.g., `fix/rendering-bug`, `feature/add-new-optimizer`).

2. **Commit Conventions:**  
   - Write clear, concise commit messages.
   - Use the imperative mood (e.g., "Add", "Fix", "Update").

3. **Testing:**  
   - Ensure that new code includes unit tests or updates to existing tests when applicable.
   - Run all tests locally before submitting your pull request.

4. **Submit a Pull Request (PR):**  
   - Open a PR to the `main` or designated development branch.
   - Include a summary of changes, reference related issues (e.g., “Closes #XX”), and provide context for reviewers.
   - Be open to feedback or questions—collaboration is key!

5. **Review Process:**  
   - Your pull request may go through review and discussion.
   - Address any feedback by making the required changes and updating your PR.

---

## Coding Standards & Best Practices

- **Code Style:**  
  Adhere to existing code style guidelines used in the project (e.g., naming conventions, file organization).  
  *Note:* Our project intentionally omits the "I" prefix for interfaces (e.g., `Chunk` instead of `IChunk`).

- **Documentation:**  
  Write or update documentation comments where necessary.  
  Keep inline documentation clear to help others understand the intent of your code.

- **Testing:**  
  Ensure all new features or bug fixes include appropriate tests.
  Use the test frameworks already in place for consistency.

- **Code Reviews:**  
  Respect the code review process and be open to suggestions.  
  Maintain clear, constructive, and respectful communication.

---

## Branching & Workflow

- **Main Branches:**
  - `main`: Contains the stable version of the project.
  - Feature branches should be created off of `main`.
  
- **Merging:**  
  - All contributions should be merged via pull requests.
  - Ensure your branch is up to date with the latest changes from the master before merging.

- **Rebasing vs. Merging:**  
  Follow project guidelines on whether to rebase your branch (to maintain a clean history) or merge. Clarify with maintainers if unsure.

---

## Code of Conduct

We expect all contributors to follow our [Code of Conduct](CODE_OF_CONDUCT.md). Please be respectful, inclusive, and considerate in all interactions.  
*Violations of the code of conduct will not be tolerated.*

---

## License

By contributing to this project, you agree that your contributions will be licensed under the MIT License, as stated in the [LICENSE](LICENSE) file.

---

## Final Notes

Your contributions, no matter how big or small, are valuable and appreciated. Thank you for being a part of the Voxel Mesh Optimization Library community! If you have any questions, feel free to open an issue or contact the project maintainers.
