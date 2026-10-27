| Milestone | Title | Gate |
|---|---|---|
| M1 | Independent package consumption | Package/Consumer |
| M2 | Opt-in UI feature manifest | Package/Acceptance |
| M3 | Activation-aware UI contributions | Behavior/Consumer |
| M4 | Consumed packages and localized labels | Package/Consumer |
| M5 | Reachable session surface | Behavior/Consumer |

| Work | Scope | Title | Priority | Lane | Depends on | Status | Refined |
|---|---|---|---|---|---|---|---|
| M1-001 | CPKG0001-US01-SC01 | [Restore independent package consumption](done/CPKG0001.US01.SC01.independent-package-consumption.md) | P1 | components | None | done | true |
| M2-001 | CPLG0001-US01-SC01 | [Publish an opt-in UI feature manifest](done/CPLG0001.US01.SC01.ui-feature-manifest.md) | P1 | components | None | done | true |
| M3-001 | CPAN0001-US01-SC01 | [Project active session panels](done/CPAN0001.US01.SC01.active-session-panels.md) | P1 | components | M2-001, M3-X01 | done | true |
| M3-002 | CBLK0001-US01-SC01 | [Resolve active owned session blocks](done/CBLK0001.US01.SC01.active-owned-session-blocks.md) | P1 | components | M2-001, M3-X01 | done | true |
| M3-003 | CTHE0001-US01-SC01 | [Resolve active provider-scoped themes](done/CTHE0001.US01.SC01.active-provider-themes.md) | P1 | components | M2-001, M3-X01 | done | true |
| M3-004 | CPGS0001-US01-SC01 | [Resolve active plugin pages](done/CPGS0001.US01.SC01.active-plugin-pages.md) | P1 | components | M2-001, M3-X01 | done | true |
| M4-001 | CLOC0001-US01-SC01 | [Resolve the packaged localization](done/CLOC0001.US01.SC01.packaged-localization.md) | P1 | components | M1-001 | done | true |
| M4-002 | CPKG0002-US01-SC01 | [Consume the published feed in the packaging scenario](todo/CPKG0002.US01.SC01.published-feed-consumption.md) | P2 | components | M1-001 | todo | true |
| M5-001 | CBLK0002-US01-SC01 | [Reach the plugin tab of the session surface](todo/CBLK0002.US01.SC01.session-plugin-tab-reachability.md) | P1 | components | M3-001 | todo | true |
| M5-002 | CBLK0003-US01-SC01 | [Reset a stale state of the session surface](todo/CBLK0003.US01.SC01.session-surface-stale-state.md) | P1 | components | M3-001 | todo | true |
