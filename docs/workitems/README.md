| Milestone | Title | Gate |
|---|---|---|
| M1 | Independent package consumption | Package/Consumer |
| M2 | Opt-in UI feature manifest | Package/Acceptance |
| M3 | Activation-aware UI contributions | Behavior/Consumer |
| M4 | Consumed packages and localized labels | Package/Consumer |
| M5 | Reachable session surface | Behavior/Consumer |
| M6 | Trim-safe canonical source | Package/Consumer |
| M7 | Compatible producer batch | Package/Consumer |
| M8 | Packing tool diagnostics | Tooling/Refusal |

| Work | Scope | Title | Priority | Lane | Depends on | Status | Refined | Issue |
|---|---|---|---|---|---|---|---|---|
| M1-001 | CPKG0001-US01-SC01 | [Restore independent package consumption](done/CPKG0001.US01.SC01.independent-package-consumption.md) | P1 | components | None | done | true | [#19](https://github.com/lowlandtech/components/issues/19) |
| M2-001 | CPLG0001-US01-SC01 | [Publish an opt-in UI feature manifest](done/CPLG0001.US01.SC01.ui-feature-manifest.md) | P1 | components | None | done | true | [#20](https://github.com/lowlandtech/components/issues/20) |
| M3-001 | CPAN0001-US01-SC01 | [Project active session panels](done/CPAN0001.US01.SC01.active-session-panels.md) | P1 | components | M2-001, M3-X01 | done | true | [#17](https://github.com/lowlandtech/components/issues/17) |
| M3-002 | CBLK0001-US01-SC01 | [Resolve active owned session blocks](done/CBLK0001.US01.SC01.active-owned-session-blocks.md) | P1 | components | M2-001, M3-X01 | done | true | [#16](https://github.com/lowlandtech/components/issues/16) |
| M3-003 | CTHE0001-US01-SC01 | [Resolve active provider-scoped themes](done/CTHE0001.US01.SC01.active-provider-themes.md) | P1 | components | M2-001, M3-X01 | done | true | [#21](https://github.com/lowlandtech/components/issues/21) |
| M3-004 | CPGS0001-US01-SC01 | [Resolve active plugin pages](done/CPGS0001.US01.SC01.active-plugin-pages.md) | P1 | components | M2-001, M3-X01 | done | true | [#18](https://github.com/lowlandtech/components/issues/18) |
| M4-001 | CLOC0001-US01-SC01 | [Resolve the packaged localization](done/CLOC0001.US01.SC01.packaged-localization.md) | P1 | components | M1-001 | done | true | [#22](https://github.com/lowlandtech/components/issues/22) |
| M4-002 | CPKG0002-US01-SC01 | [Consume the published feed in the packaging scenario](done/CPKG0002.US01.SC01.published-feed-consumption.md) | P2 | components | M1-001 | done | true | [#23](https://github.com/lowlandtech/components/issues/23) |
| M5-001 | CBLK0002-US01-SC01 | [Reach the plugin tab of the session surface](done/CBLK0002.US01.SC01.session-plugin-tab-reachability.md) | P1 | components | M3-001 | done | true | [#24](https://github.com/lowlandtech/components/issues/24) |
| M5-002 | CBLK0003-US01-SC01 | [Reset a stale state of the session surface](done/CBLK0003.US01.SC01.session-surface-stale-state.md) | P1 | components | M3-001 | done | true | [#25](https://github.com/lowlandtech/components/issues/25) |
| M6-001 | CTRM0001-US01-SC01 | [Make the canonical components source trim-safe](done/CTRM0001.US01.SC01.trim-safe-canonical-components-source.md) | P1 | components | None | done | true | [#26](https://github.com/lowlandtech/components/issues/26) |
| M7-001 | CPKG0003-US01-SC01 | [Declare the batch dependency range exactly](done/CPKG0003.US01.SC01.exact-batch-declaration.md) | P1 | components | None | done | true | [#27](https://github.com/lowlandtech/components/issues/27) |
| M7-002 | CPKG0005-US01-SC01 | [Publish the compatible immutable producer batch](done/CPKG0005.US01.SC01.publish-compatible-producer-batch.md) | P1 | components | M7-001 | done | true | [#28](https://github.com/lowlandtech/components/issues/28) |
| M8-001 | CPKG0004-US01-SC01 | [Diagnose the project selection of the packing tool](done/CPKG0004.US01.SC01.diagnose-project-selection.md) | P2 | components | None | done | true | [#29](https://github.com/lowlandtech/components/issues/29) |
