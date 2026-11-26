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
| M9 | Residual trim coverage | Package/Consumer |
| M10 | Trimmable shared assemblies | Package/Consumer |
| M11 | Annotation chain | Package/Consumer |
| M12 | Consolidation decisions | Assessment |
| M13 | Current Core generation | Package/Consumer |
| M14 | Trimmed block pages | Package/Consumer |
| M15 | Showcase trim cleanliness | Test/Consumer |
| M16 | Semantic page routes | Test/Fixture |
| M17 | Grid row window | Proposal |
| M18 | User-requested component features | Package/Consumer |
| M19 | Target framework discipline | Test/Toolchain |
| M20 | Pinned SDK host | Test/Toolchain |
| M21 | Document-defined forms | Behavior/Consumer |
| M22 | Result-case transitions | Behavior/Consumer |
| M23 | Documents and transitions render from data | Behavior/Consumer |

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
| M9-001 | CPKG0006-US01-SC01 | [Prove the residual trim suppressions in a host](done/CPKG0006.US01.SC01.prove-residual-trim-suppressions.md) | P2 | components | None | done | true | [#30](https://github.com/lowlandtech/components/issues/30) |
| M10-001 | CPKG0007-US01-SC01 | [Let a consumer trim the shared assemblies](done/CPKG0007.US01.SC01.trim-the-shared-assemblies.md) | P1 | components | None | done | true | [#31](https://github.com/lowlandtech/components/issues/31) |
| M11-001 | CPKG0008-US01-SC01 | [Close the annotation chain of the descriptor](done/CPKG0008.US01.SC01.close-the-annotation-chain.md) | P1 | components | None | done | true | [#32](https://github.com/lowlandtech/components/issues/32) |
| M12-001 | CPKG0009-US01-SC01 | [Assess the central-source consolidation for the components packages](done/CPKG0009.US01.SC01.assess-central-source-consolidation.md) | P2 | components | None | done | true | [#33](https://github.com/lowlandtech/components/issues/33) |
| M12-002 | CPKG0010-US01-SC01 | [Record the trimming contract for consumers](done/CPKG0010.US01.SC01.record-consumer-trimming-contract.md) | P3 | components | None | done | true | [#34](https://github.com/lowlandtech/components/issues/34) |
| M13-001 | CPKG0011-US01-SC01 | [Move the batch to the current Core generation](done/CPKG0011.US01.SC01.move-batch-to-current-core.md) | P1 | components | None | done | true | [#35](https://github.com/lowlandtech/components/issues/35) |
| M14-001 | CPKG0012-US01-SC01 | [Keep the injected members of a block alive under trimming](done/CPKG0012.US01.SC01.keep-block-injections-alive.md) | P1 | components | None | done | true | [#36](https://github.com/lowlandtech/components/issues/36) |
| M15-001 | CPKG0013-US01-SC01 | [Let the showcase pages render trim-clean](done/CPKG0013.US01.SC01.showcase-pages-trim-clean.md) | P3 | components | None | done | true | [#37](https://github.com/lowlandtech/components/issues/37) |
| M16-001 | CPKG0014-US01-SC01 | [Take the lane identity out of the page fixture's route](done/CPKG0014.US01.SC01.semantic-page-fixture-route.md) | P2 | components | None | done | true | [#38](https://github.com/lowlandtech/components/issues/38) |
| M17-001 | CPKG0015-US01-SC01 | [A host-supplied visible-row window for grid templates](done/CPKG0015.US01.SC01.host-supplied-row-window.md) | P3 | components | None | done | true | [#39](https://github.com/lowlandtech/components/issues/39) |
| M18-001 | CPKG0016-US01-SC01 | [Bind NativeSelect and draw the separator tab strip](done/CPKG0016.US01.SC01.nativeselect-binding-and-separator-tabs.md) | P1 | components | None | done | true | [#40](https://github.com/lowlandtech/components/issues/40) |
| M19-001 | CPKG0017-US01-SC01 | [Keep the packaging harness on the target framework band](done/CPKG0017.US01.SC01.harness-target-framework-band.md) | P1 | components | None | done | true | [#41](https://github.com/lowlandtech/components/issues/41) |
| M20-001 | CPKG0018-US01-SC01 | [Resolve a host that satisfies the pinned SDK](done/CPKG0018.US01.SC01.resolve-sdk-satisfying-host.md) | P1 | components | None | done | true | [#42](https://github.com/lowlandtech/components/issues/42) |
| M21-001 | CPKG0019-US01-SC01 | [A form renders fields the document declares](done/CPKG0019.US01.SC01.dynamic-dataform-model.md) | P1 | components | None | done | true | [#43](https://github.com/lowlandtech/components/issues/43) |
| M21-002 | CPKG0020-US01-SC01 | [A view-only relation does not look editable](done/CPKG0020.US01.SC01.readonly-edge-form.md) | P2 | components | None | done | true | [#44](https://github.com/lowlandtech/components/issues/44) |
| M22-001 | CPKG0021-US01-SC01 | [A transition result carries its cases](done/CPKG0021.US01.SC01.transition-result-cases.md) | P1 | components | None | done | true | [#45](https://github.com/lowlandtech/components/issues/45) |
| M23-001 | CPKG0022-US01-SC01 | [A document and its transitions render from data](todo/CPKG0022.US01.SC01.document-view-and-transition-trigger.proposal.md) | P1 | components | CPKG0019-US01-SC01, CPKG0020-US01-SC01, CPKG0021-US01-SC01 | proposal | false | [#46](https://github.com/lowlandtech/components/issues/46) |
