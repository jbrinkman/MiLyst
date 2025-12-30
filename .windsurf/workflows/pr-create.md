---
auto_execution_mode: 3
description: Prepare my changes for a pull request
---

**Steps**
Track these steps as TODOs and complete them one by one.
1. Ask the user if they want a new branch from `main` or they want to use their current branch.
2. If they want a new branch, create a new branch from `main`.
3. If they want to use their current branch, ensure it is up to date with `main`.
4. Commit all changes to the branch.
5. Push the branch to the remote repository.
6. Create a pull request from the branch to `main`. Use the Github MCP tools to create the pull request. If the tools are not available, ask the user to enable the tools and then the user should respond with `continue pr-create`.
7. Ask the user whether Copilot should perform a review of the pull request. If the user agrees, use the Github MCP tools to ask Copilot to perform the review (request_copilot_review). 