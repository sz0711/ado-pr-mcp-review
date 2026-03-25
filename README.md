# 🔍 ado-pr-mcp-review

> AI-powered pull request review for Azure DevOps, backed by Dataverse MCP schema validation.

[![License](https://img.shields.io/badge/license-MIT-green.svg)](#license)
[![Status](https://img.shields.io/badge/status-active-blue)]()

---

## Overview

Portable setup package that wires **GitHub Copilot** into an **Azure DevOps pipeline** and points it at the **Dataverse MCP server** for live schema-aware code review. Drop the templates into any repository, configure four pipeline variables, and every pull request gets an automated review that validates Dataverse entity fields, plugin runnability, and performance risks against the real environment schema.

### Key Capabilities

- **Schema validation** — Copilot queries the Dataverse MCP to verify every field name, type, and relationship used in plugin code
- **Runnability checks** — validates pre-image requirements, target payload semantics, and safe-write patterns
- **Performance analysis** — flags N+1 patterns, `AllColumns` usage, synchronous-stage cost, and throttling risks
- **Strict output rules** — ASCII-only, no style comments, no praise; only actionable findings
- **Zero-config endpoint fallback** — pipeline defaults to `/api/mcp` when `dataverseMcpEndpointPath` is not set

<img width="1528" height="786" alt="Pipeline screenshot" src="https://github.com/user-attachments/assets/6333bd44-1344-445b-ae39-cf5666129339" />

---

## Getting Started

### Prerequisites

1. **Dataverse MCP** enabled in the target Power Platform environment
2. **Microsoft Entra app registration** with `Dynamics CRM / user_impersonation` permission, registered as Application User in Power Platform Admin Center
3. **Azure DevOps extension** `ado-copilot-code-review` by LittleFortSoftware installed in your organization
   - Marketplace: <https://marketplace.visualstudio.com/items?itemName=LittleFortSoftware.ado-copilot-code-review>
4. **Variable group** `vg-dataverse-mcp-ci` linked to the pipeline (see below)

### Variable Group

Create variable group `vg-dataverse-mcp-ci` in Azure DevOps:

| Variable | Required | Description |
|----------|----------|-------------|
| `dataverseClientId` | ✅ | Entra app client ID |
| `dataverseClientSecret` | ✅ | Client secret (mark as secret) |
| `dataverseTenantId` | ✅ | Entra tenant ID |
| `dataverseUrl` | ✅ | Environment URL, e.g. `https://yourorg.crm.dynamics.com` |
| `dataverseMcpEndpointPath` | | MCP path (default: `/api/mcp`) |
| `githubpat` | | GitHub PAT if not managed elsewhere |

### Install

1. Copy the `templates/` folder into your target repository.
2. Copy `templates/azure-pipelines.yml` to the repository root as `azure-pipelines.yml` (or merge into your existing pipeline).
3. Copy `templates/copilotScripts/review-prompt.md` to `copilotScripts/review-prompt.md`.
4. Link variable group `vg-dataverse-mcp-ci` to the pipeline in Azure DevOps.
5. Run the pipeline and confirm the log line `Configured endpoint:` shows a real URL path.

---

## File Structure

```
templates/
├── azure-pipelines.yml              # Pipeline definition
└── copilotScripts/
    └── review-prompt.md             # Copilot review prompt
```

---

## Common Issues

### Endpoint shows placeholder instead of real path

**Symptom:** `Configured endpoint: https://org.crm.dynamics.com/$(dataverseMcpEndpointPath)`  
**Cause:** Optional variable not set; unresolved macro remained literal.  
**Fix:** The fallback logic in the template handles this automatically — no action needed.

### Prompt rejected due to invalid characters

**Symptom:** Custom prompt task fails because of unsupported characters.  
**Fix:** Keep the review prompt ASCII-only and avoid double-quote characters.

### MCP cannot validate schema

**Symptom:** Review states Dataverse MCP is not reachable.  
**Fix:** Verify `dataverseClientId`, `dataverseClientSecret`, `dataverseTenantId`, and `dataverseUrl`. Confirm the app is allowed in Dataverse MCP client settings.

---

## Security

- Never print `dataverseClientSecret` in pipeline logs.
- Store secrets only in Azure DevOps secret variables or Key Vault-linked variables.
- Use least privilege for the Entra app registration.

---

## License

MIT

## Disclaimer

Private project. Provided without warranty. Use at your own risk.

---

> Built with ❤️ for Dynamics 365 developers and architects.
