# Contributing

## Developer Certificate of Origin (DCO)

All contributions to this project must be signed off.

- Use `git commit -s` for every commit.
- Your commit message must include a `Signed-off-by:` line.

Commits without the required DCO sign-off will be rejected by the repository's git hooks.

## Conventional Commits

All commit messages must follow the Conventional Commits specification.

- Format: `<type>(<scope>): <description>`
- Examples:
  - `feat(ui): add task list filter`
  - `fix(api): handle empty response`
  - `chore: update dependencies`

Use the following types where appropriate: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`.
