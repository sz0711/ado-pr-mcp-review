# 🔍 ado-pr-mcp-review

> Copilot-powered pull request review for Azure DevOps, enriched with live Dataverse schema validation via MCP.

[![Azure DevOps](https://img.shields.io/badge/Azure%20DevOps-pipeline-0078D7?logo=azure-devops)](https://azure.microsoft.com/en-us/products/devops/)
[![Extension](https://img.shields.io/badge/extension-ado--copilot--code--review-informational)](https://marketplace.visualstudio.com/items?itemName=LittleFortSoftware.ado-copilot-code-review)
[![Status](https://img.shields.io/badge/status-stable-brightgreen)]()

---

## Overview

A portable setup package that wires GitHub Copilot-based pull request reviews into an Azure DevOps pipeline and connects Copilot to a live Dataverse MCP server — so every review validates actual entity schemas, not just code style.

### Key Capabilities

- **MCP-Backed Schema Validation** — Copilot queries the live Dataverse MCP during review to confirm entity and field names exist
- **Plugin Runnability Checks** — validates pre-image usage, `Target` payload semantics, and safe-write patterns
- **Performance Analysis** — flags N+1 call patterns, `AllColumns` retrieves, synchronous-stage latency risks
- **Strict Review Prompt** — ASCII-safe, no-praise, no-speculation prompt enforces actionable findings only
- **Production-Ready Pipeline** — parameterised `azure-pipelines.yml` with input validation and secure secret handling

---

## Architecture

```
templates/
├── azure-pipelines.yml          # Pipeline: installs Copilot CLI, configures MCP, triggers review
└── copilotScripts/
    └── review-prompt.md         # Copilot review prompt (schema + runnability + performance rules)
```

### Pipeline stages

```
Install Copilot CLI -> Configure Dataverse MCP Server -> CopilotCodeReview task
```

The `Configure Dataverse MCP Server` step builds `~/.copilot/mcp-config.json` from pipeline variables and writes the authenticated Dataverse endpoint. Copilot then uses this MCP server for all schema lookups during review.

---

## Getting Started

### Prerequisites

- Azure DevOps organization with a pipeline linked to this repository
- [ado-copilot-code-review](https://marketplace.visualstudio.com/items?itemName=LittleFortSoftware.ado-copilot-code-review) extension installed in your Azure DevOps organization
- Microsoft Entra app registration with Dataverse `user_impersonation` permission, registered as Application User in Power Platform Admin Center
- Dataverse MCP enabled in the target environment and the app allowed in Dataverse MCP client settings

### Install into a repository

1. Copy `templates/azure-pipelines.yml` to the repository root as `azure-pipelines.yml` (or merge into your existing pipeline).
2. Copy `templates/copilotScripts/review-prompt.md` to `copilotScripts/review-prompt.md`.
3. Create the Azure DevOps variable group `vg-dataverse-mcp-ci` (see below).
4. Link the variable group to the pipeline in Azure DevOps.
5. Run the pipeline and verify the log line `Configured endpoint:` resolves to a real URL path (for example `/api/mcp`).

---

## Variable Group

Create variable group: `vg-dataverse-mcp-ci`

| Variable | Required | Description |
|----------|----------|-------------|
| `dataverseClientId` | ✅ | Entra app registration client ID |
| `dataverseClientSecret` | ✅ | Client secret (mark as secret) |
| `dataverseTenantId` | ✅ | Entra ID tenant ID |
| `dataverseUrl` | ✅ | Dataverse environment URL, e.g. `https://yourorg.crm.dynamics.com` |
| `dataverseMcpEndpointPath` | | MCP endpoint path (default: `/api/mcp`) |
| `githubpat` | | GitHub PAT if not managed elsewhere |

---

## Validation Checklist

- Pipeline succeeds in the `Configure Dataverse MCP Server` step.
- Log line `Configured endpoint:` shows a real URL (not a `$(...)` placeholder).
- MCP config file exists on the agent at `~/.copilot/mcp-config.json`.
- Copilot code review starts without prompt format errors.
- Dataverse schema findings are reported for any invalid entity fields in reviewed code.

---

## Common Issues

### Endpoint shows placeholder instead of real path

**Symptom:** `Configured endpoint: https://org.crm.dynamics.com/$(dataverseMcpEndpointPath)`

**Cause:** Optional variable not set; unresolved macro remained literal.

**Fix:** The pipeline template already contains fallback logic that defaults to `/api/mcp`. Ensure the variable group is linked to the pipeline.

---

### Prompt rejected due to invalid characters

**Symptom:** The `CopilotCodeReview` task fails with a prompt format or encoding error.

**Fix:** Keep the review prompt ASCII-only. Do not introduce smart quotes, long dashes, or other non-ASCII characters.

---

### MCP cannot validate schema

**Symptom:** Review output states that Dataverse MCP is not reachable.

**Fix:** Verify `dataverseClientId`, `dataverseClientSecret`, `dataverseTenantId`, and `dataverseUrl` values. Confirm the Entra app is registered as an Application User in Power Platform and is allowed in Dataverse MCP client settings.

---

## Security Notes

- Never print `dataverseClientSecret` in pipeline logs.
- Store all secrets in Azure DevOps secret variables or Key Vault-linked variable groups.
- Use least privilege for the Entra app registration.

---

## Technology Stack

| Component | Detail |
|-----------|--------|
| Azure Pipelines | Pipeline runtime |
| GitHub Copilot CLI | `@github/copilot` npm package |
| ado-copilot-code-review | LittleFortSoftware marketplace extension |
| Dataverse MCP | Live schema server (`/api/mcp`) |
| PowerShell | Pipeline scripting (`pwsh`) |
| Node.js | 20.x (Copilot CLI dependency) |

---

## License

MIT

## Disclaimer

Private project. Provided without warranty. Use at your own risk.

---

> Built with ❤️ for Dynamics 365 developers and Azure DevOps teams.
