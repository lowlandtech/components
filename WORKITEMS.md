# Workitem migration

Imported from the local Foundry checkout at `D:/lowlandtech/develop` and `lowlandtech/foundry` GitHub issues. Source documents are preserved under [planning/imported](planning/imported). Original source issues were left unchanged.

## Open backlog

New local plugin-integration proposals are listed in
[docs/workitems/README.md](docs/workitems/README.md), with the
[compatibility review](docs/plugin-recommendations.md). They are mirrored to
GitHub issues #16–#21 and do not change the imported issue states below.

- [SPEC1015: verify and close Combobox review gaps](https://github.com/lowlandtech/components/issues/1) — `doc-SPEC1015`
- [SPEC1043: verify Sidebar behavior and accessibility review gaps](https://github.com/lowlandtech/components/issues/2) — `doc-SPEC1043`
- [SPEC1062: compose DataGrid pager and internal controls from reusable components](https://github.com/lowlandtech/components/issues/3) — `doc-SPEC1062`
- [SPEC1072: compose DataForm and FormField editors from shared controls](https://github.com/lowlandtech/components/issues/4) — `doc-SPEC1072`
- [SPEC1078US04: verify DynamicBlock shared-control composition and mutation behavior](https://github.com/lowlandtech/components/issues/5) — `doc-SPEC1078US04`
- [SPEC1077: complete advanced theming implementation review gates](https://github.com/lowlandtech/components/issues/6) — `doc-SPEC1077`
- [SPEC2100US50: expand descriptor-ready component palette coverage](https://github.com/lowlandtech/components/issues/7) — `doc-SPEC2100US50`
- [SPEC1080: introduce deterministic WorkApp scenarios and derived dashboard metrics](https://github.com/lowlandtech/components/issues/8) — `doc-SPEC1080`

## Completed history

- [SPEC1007US02: feat(Button) — Variant + Color scheme parameters](https://github.com/lowlandtech/components/issues/9) — `issue-27`
- [SPEC1069US01SC01: bug — fix(explorer) responsive stacking CSS not taking effect on narrow viewports](https://github.com/lowlandtech/components/issues/10) — `issue-28`
- [SPEC1110: feat(components) real-file StoryCard examples and runnable sandbox path](https://github.com/lowlandtech/components/issues/11) — `issue-139`
- [SPEC2200US08: feat(ui-blocks) establish reusable template UI block library](https://github.com/lowlandtech/components/issues/12) — `issue-141`
- [SPEC1078US02: feat(components/session) add advanced session composition blocks](https://github.com/lowlandtech/components/issues/13) — `issue-150`
- [SPEC1078US03: feat(session) add client-side SessionApp runtime pipelines](https://github.com/lowlandtech/components/issues/14) — `issue-151`
- [SPEC1073: establish the Blazor Showcode component explorer](https://github.com/lowlandtech/components/issues/15) — `doc-blazor-book-showcode`

## Selection rules

- Per-component accepted/completed reviews override the older summary inventory and next-components list; accepted items were not reopened.
- Only the remaining reviewed components and explicit debt handovers were imported as open backlog, with triage and estimation required.
- The local completed Showcode workitem and six completed GitHub component issues were imported as closed.
- API-key, graph, session and work-plugin issues without component ownership were excluded.
- Historical issue bodies and discussions retain their original text and source references; old monorepo paths may require translation from `src/components/` to `src/`.
- Existing SPEC collisions were retained rather than silently renumbered. No new SPEC numbers, sprint milestones, assignees or estimates were invented.
