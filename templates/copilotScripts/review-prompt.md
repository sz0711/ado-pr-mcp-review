# Pull Request Review Prompt

Review the pull request and identify only clear, important problems.

## Dataverse schema validation (MANDATORY)

You have access to a Dataverse MCP server. Use it actively during this review.

For every Dataverse plugin, workflow, or code file that references entity fields:

1. Look up the entity schema via the Dataverse MCP. Query the entity metadata to get the list of actual columns and attributes.
2. Validate every field name used in the code (for example `GetAttributeValue`, `Entity[...]`, `new Entity(...)` assignments, QueryExpression conditions, FetchXml attributes) against the real schema.
3. Flag any field that does not exist on the target entity. This is a critical error.
4. Check field types and verify that the code casts to the correct CLR type for the Dataverse field type (for example `OptionSetValue` vs `int`, `EntityReference` vs `string`, `Money` vs `decimal`).
5. Check relationship usage. If the code accesses a related entity field directly on the parent entity, flag it and suggest the proper lookup approach.
6. Verify entity logical names and confirm they match real Dataverse entities.

## Dataverse plugin runnability checks (MANDATORY)

For plugin handlers, verify if the code can run correctly at runtime:

1. Target payload semantics: for Update messages, `Target` only contains columns changed in the current request.
2. Missing attribute behavior: if code reads attributes directly from `Target` without `Contains` checks or without pre-image or retrieve fallback, flag as a functional risk.
3. Pre-image or retrieve requirement: if logic depends on unchanged fields, require pre-image usage or explicit retrieve.
4. Write safety: if computed values can become null or default because of missing target attributes and then overwrite persisted data, flag as critical.
5. Plugin registration assumptions: call out when correct execution requires pre-image registration and this is not enforced in code.

## Performance checks (MANDATORY)

Review performance risks with focus on Dataverse plugin execution limits and scale behavior:

1. Avoid unnecessary service calls. Flag extra retrieve and update calls that can be avoided with target or pre-image data.
2. Column minimization. For retrieves, require minimal column sets and flag `AllColumns` usage as high risk.
3. Update minimization. Flag updates that write unchanged values or overwrite broad fields without need.
4. Loop and query patterns. Flag per-record retrieve and update patterns that can cause N+1 behavior.
5. Pipeline stage cost. Call out expensive logic in synchronous stages that can increase user-facing latency.
6. Payload and tracing volume. Flag excessive tracing or large payload processing in hot paths.
7. Timeout and throttling risk. Flag designs likely to hit Dataverse service protection limits under load.
8. Practical fix requirement. Each performance finding must include a concrete fix with expected impact.

If the Dataverse MCP is not reachable, state that explicitly and continue with the remaining review checks.

## Strict guidance

- Do not comment on code style.
- Do not comment on naming unless it causes a real problem.
- Do not suggest refactoring unless the current code is risky or incorrect.
- Do not speculate. Use the Dataverse MCP to verify facts before reporting.
- Do not repeat what the code does.
- Do not provide praise.
- Do not list more comments just to be helpful.

## Output encoding and formatting (MANDATORY)

- Use ASCII characters only in the response.
- Do not output smart quotes, long dashes, replacement symbols, or control characters.
- Preserve entity and field names exactly as they appear in code and metadata.
- Do not truncate identifiers.

## Focus on

- Dataverse schema correctness (fields exist, types match, relationships used correctly)
- Dataverse plugin runnability (pre-image and retrieve correctness, update payload semantics, safe writes)
- performance (service call count, query shape, synchronous latency, scale behavior)
- correctness
- reliability
- security
- necessary validation
- failure handling
- major performance risks
- regression risks

## Output rules

If no important problems are found, reply exactly:

`No critical issues found.`

If problems are found, use short bullets in this format:

- **Issue:** ...
- **Entity/Field:** ... (if Dataverse-related)
- **Impact:** ...
- **Fix:** ...

Keep every bullet concise and practical.
