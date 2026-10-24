# Plugin integration follow-ups

Local proposals from the [compatibility review](../plugin-recommendations.md).
The issue workflow in ../../README.md remains authoritative: every proposal is
`backlog`, needs triage and estimate, and has no owner/sprint. GitHub mirrors are
listed below and linked from each workitem file.
Blocked proposals additionally carry a named blocker below and in their file.
Do not mark work ready/done before its dependencies and executable criteria pass.

| Scope | Workitem | State | Blocked by |
|---|---|---|---|
| CPKG0001-US01-SC01 | [Restore independent package consumption](CPKG0001.US01.SC01.independent-package-consumption.md) | review | none |
| CPLG0001-US01-SC01 | [Publish an opt-in UI feature manifest](CPLG0001.US01.SC01.ui-feature-manifest.md) | backlog | CPKG0001-US01-SC01 |
| CPAN0001-US01-SC01 | [Project active session panels](CPAN0001.US01.SC01.active-session-panels.md) | backlog | CPLG0001-US01-SC01; Plugins BLAZ0001-US01-SC01 |
| CBLK0001-US01-SC01 | [Resolve active owned session blocks](CBLK0001.US01.SC01.active-owned-session-blocks.md) | backlog | CPLG0001-US01-SC01; Plugins BLAZ0001-US01-SC01 |
| CTHE0001-US01-SC01 | [Resolve active provider-scoped themes](CTHE0001.US01.SC01.active-provider-themes.md) | backlog | CPLG0001-US01-SC01; Plugins BLAZ0001-US01-SC01 |
| CPGS0001-US01-SC01 | [Resolve active plugin pages](CPGS0001.US01.SC01.active-plugin-pages.md) | backlog | CPLG0001-US01-SC01; Plugins BLAZ0001-US01-SC01 |

External dependencies are in the local Plugins repository's docs/workitems.
Each scenario is independently executable; do not combine their acceptance or
infer completion from the existing imported component reviews. Coder's M6 work
consumes the reusable bridge; it retains application policy and slot ownership.

## GitHub mirror

| Scope | Issue |
|---|---|
| CPKG0001-US01-SC01 | [#19](https://github.com/lowlandtech/components/issues/19) |
| CPLG0001-US01-SC01 | [#20](https://github.com/lowlandtech/components/issues/20) |
| CPAN0001-US01-SC01 | [#17](https://github.com/lowlandtech/components/issues/17) |
| CBLK0001-US01-SC01 | [#16](https://github.com/lowlandtech/components/issues/16) |
| CTHE0001-US01-SC01 | [#21](https://github.com/lowlandtech/components/issues/21) |
| CPGS0001-US01-SC01 | [#18](https://github.com/lowlandtech/components/issues/18) |

Preview with `./scripts/Sync-PluginWorkitems.ps1`; apply with `-Apply`. The sync
matches stable workitem markers/titles, preserves imported issues and unrelated
labels, and does not invent owners, estimates or sprint assignments.
