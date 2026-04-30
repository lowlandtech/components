/**
 * containerGrid.js
 * Drag-and-drop + resize binding for ContainerGrid / ContainerGridSection.
 *
 * Ported from MudExGrid (samples/MudBlazor.Extensions). The original reads
 * positions from CSS classes (mud-ex-grid-section-row-start-N, ...). This
 * version reads from data-* attributes the Blazor components emit, so the
 * binding stays decoupled from the inline CSS-Grid styling.
 *
 * Section data contract (set by ContainerGridSection.razor.cs):
 *   data-id            unique id
 *   data-row           1-based row start
 *   data-col           1-based column start
 *   data-row-span      row count
 *   data-col-span      column count
 *   data-options       JSON: { movable, resizable, lockX, lockY, minCol,
 *                              maxCol, minRow, maxRow, minColSpan,
 *                              maxColSpan, minRowSpan, maxRowSpan }
 *
 * Grid data contract (set by ContainerGrid.razor.cs):
 *   data-rows          row track count
 *   data-cols          column track count
 *   data-options       JSON: { resolveCollisions, floatMode, compactOnDrop }
 */

// ── Read helpers ────────────────────────────────────────────────────────────

function readSection(el) {
    return {
        id: el.dataset.id,
        el,
        row: parseInt(el.dataset.row) || 1,
        col: parseInt(el.dataset.col) || 1,
        rowSpan: parseInt(el.dataset.rowSpan) || 1,
        colSpan: parseInt(el.dataset.colSpan) || 1,
    };
}

function buildModel(gridEl) {
    return [...gridEl.querySelectorAll(".lt-container-grid-section")].map(readSection);
}

function measure(gridEl) {
    const r = gridEl.getBoundingClientRect();
    const rows = parseInt(gridEl.dataset.rows) || 8;
    const cols = parseInt(gridEl.dataset.cols) || 12;
    return { rows, cols, cw: r.width / cols, ch: r.height / rows, left: r.left, top: r.top };
}

function px(item, m) {
    return {
        x: (item.col - 1) * m.cw,
        y: (item.row - 1) * m.ch,
        w: item.colSpan * m.cw,
        h: item.rowSpan * m.ch,
    };
}

function readGridOpts(gridEl) {
    try { return JSON.parse(gridEl.dataset.options || "{}"); }
    catch { return {}; }
}

function readSectionOpts(el) {
    try { return JSON.parse(el.dataset.options || "{}"); }
    catch { return {}; }
}

// ── Geometry / layout solver (unchanged from MudExGrid) ────────────────────

function rectsOverlap(a, b) {
    return !(a.row + a.rowSpan - 1 < b.row ||
        b.row + b.rowSpan - 1 < a.row ||
        a.col + a.colSpan - 1 < b.col ||
        b.col + b.colSpan - 1 < a.col);
}

function canPlace(layout, itm) {
    return !layout.some(x => x.id !== itm.id && rectsOverlap(itm, x));
}

function canPlaceWithOpts(layout, itm, rows, cols, optsMap) {
    const o = optsMap.get(itm.id) || {};
    const minc = Math.max(1, o.minCol ?? 1);
    const maxc = Math.min(cols - itm.colSpan + 1, o.maxCol ?? Number.MAX_SAFE_INTEGER);
    const minr = Math.max(1, o.minRow ?? 1);
    const maxr = Math.min(rows - itm.rowSpan + 1, o.maxRow ?? Number.MAX_SAFE_INTEGER);
    if (itm.col < minc || itm.col > maxc) return false;
    if (itm.row < minr || itm.row > maxr) return false;
    if (itm.row + itm.rowSpan - 1 > rows || itm.col + itm.colSpan - 1 > cols) return false;
    return !layout.some(x => x.id !== itm.id && rectsOverlap(itm, x));
}

function bfsPush(layout, moving, rows, cols, optsMap) {
    if (canPlaceWithOpts(layout, moving, rows, cols, optsMap)) return layout;
    const q = [moving];
    const seen = new Set([moving.id]);
    while (q.length) {
        const cur = q.shift();
        for (const other of layout) {
            if (other.id === cur.id) continue;
            if (rectsOverlap(cur, other)) {
                const opts = [
                    { row: cur.row + cur.rowSpan, col: other.col },
                    { row: other.row, col: cur.col + cur.colSpan },
                ];
                let placed = false;
                for (const t of opts) {
                    let r = Math.max(1, t.row), c = Math.max(1, t.col);
                    while (r <= rows && !placed) {
                        while (c <= cols) {
                            const cand = { ...other, row: r, col: c };
                            if (!layout.some(x => x.id !== cand.id && rectsOverlap(cand, x)) &&
                                cand.row + cand.rowSpan - 1 <= rows &&
                                cand.col + cand.colSpan - 1 <= cols) {
                                other.row = cand.row;
                                other.col = cand.col;
                                placed = true;
                                break;
                            }
                            c++;
                        }
                        if (!placed) { c = 1; r++; }
                    }
                }
                if (!seen.has(other.id)) { q.push(other); seen.add(other.id); }
            }
        }
    }
    return layout;
}

function trySwapOrPush(layout, moving, rows, cols, optsMap) {
    if (canPlaceWithOpts(layout, moving, rows, cols, optsMap)) return layout;
    const overlappers = layout.filter(x => x.id !== moving.id && rectsOverlap(moving, x));
    if (overlappers.length === 1) {
        const target = overlappers[0];
        const moverOld = layout.find(x => x.id === moving.id);
        const candidateInOld = { ...target, row: moverOld.row, col: moverOld.col };
        const canSwap = !layout.some(x => x.id !== target.id && rectsOverlap(candidateInOld, x)) &&
            candidateInOld.row + candidateInOld.rowSpan - 1 <= rows &&
            candidateInOld.col + candidateInOld.colSpan - 1 <= cols;
        if (canSwap) {
            target.row = moverOld.row;
            target.col = moverOld.col;
            return layout;
        }
    }
    return bfsPush(layout, moving, rows, cols, optsMap);
}

function floatUp(layout, rows) {
    for (const it of layout) {
        let moved = true;
        while (moved) {
            const cand = { ...it, row: Math.max(1, it.row - 1) };
            if (cand.row === it.row) break;
            if (cand.row + cand.rowSpan - 1 > rows) break;
            const tmp = layout.map(x => x.id === it.id ? cand : x);
            if (canPlace(tmp, cand)) it.row = cand.row;
            else moved = false;
        }
    }
    return layout;
}

function compactVertical(layout) {
    const ordered = [...layout].sort((a, b) => a.col - b.col || a.row - b.row);
    for (const it of ordered) {
        let best = it.row;
        for (let r = 1; r < it.row; r++) {
            const cand = { ...it, row: r };
            const tmp = layout.map(x => x.id === it.id ? cand : x);
            if (canPlace(tmp, cand)) best = r;
            else break;
        }
        it.row = best;
    }
    return layout;
}

function nearestFree(layout, moving, rows, cols) {
    const maxR = rows - moving.rowSpan + 1;
    const maxC = cols - moving.colSpan + 1;
    let best = null, bestDist = Infinity;
    for (let r = 1; r <= maxR; r++) {
        for (let c = 1; c <= maxC; c++) {
            const cand = { ...moving, row: r, col: c };
            const tmp = layout.map(x => x.id === moving.id ? cand : x);
            if (canPlace(tmp, cand)) {
                const d = Math.abs(r - moving.row) + Math.abs(c - moving.col);
                if (d < bestDist) { best = cand; bestDist = d; }
            }
        }
    }
    return best || moving;
}

function clearTransforms(gridEl) {
    gridEl.querySelectorAll(".lt-container-grid-section").forEach(el => {
        el.style.transform = "";
        el.style.transition = "";
    });
}

// ── Pointer event wiring ───────────────────────────────────────────────────

function commit(gridEl, finalProposal) {
    const dotnet = gridEl._containerGrid?.dotnet;
    if (!dotnet) return Promise.resolve();
    const changes = finalProposal.map(x => ({
        id: x.id, row: x.row, column: x.col, rowSpan: x.rowSpan, colSpan: x.colSpan,
    }));
    return dotnet.invokeMethodAsync("ContainerGrid_CommitLayout", changes);
}

function placeRect(el, rect) {
    el.style.left = `${rect.x}px`;
    el.style.top = `${rect.y}px`;
    el.style.width = `${rect.w}px`;
    el.style.height = `${rect.h}px`;
}

function wireDrag(gridEl, itemEl, gripEl) {
    // Drag is initiated from a dedicated grip handle (.lt-container-grid-grip)
    // rather than the whole section body — gives the user an unambiguous
    // pressed/released signal and stops accidental drags when interacting
    // with section content.
    const overlay = gridEl.querySelector(".lt-container-grid-overlay");
    const placeholder = gridEl.querySelector(".lt-container-grid-placeholder");
    const handle = gripEl ?? itemEl.querySelector(".lt-container-grid-grip");
    if (!handle) return;

    let start = null, ctx = null, lastProposal = null;

    const onDown = e => {
        const opts = readSectionOpts(itemEl);
        if (!opts.movable || e.button !== 0) return;
        e.stopPropagation();

        handle.setPointerCapture(e.pointerId);
        itemEl.classList.add("lt-container-grid-section--dragging");
        // Drag applies `transform: translate(…)` to sections to animate
        // them between cells. ResizeObserver does NOT fire on transform
        // changes, so the portaled bezel host would lag the section.
        // Run an rAF pump for the duration of the gesture to keep every
        // bezel host glued to its section.
        startBezelRafSync();
        start = { x: e.clientX, y: e.clientY };
        const base = buildModel(gridEl);
        const me = base.find(x => x.el === itemEl);
        const others = base.filter(x => x !== me);
        const m = measure(gridEl);

        const clone = itemEl.cloneNode(true);
        clone.classList.add("lt-container-grid-clone");
        clone.style.position = "absolute";
        clone.style.pointerEvents = "none";
        clone.style.zIndex = 30;
        // Strip CSS-Grid placement from the clone so absolute positioning wins.
        clone.style.gridColumn = "";
        clone.style.gridRow = "";
        overlay.appendChild(clone);
        placeRect(clone, px(me, m));

        placeholder.style.display = "block";
        placeRect(placeholder, px(me, m));

        ctx = { base, me, others, m, clone };
    };

    const onMove = e => {
        if (!start || !ctx) return;
        const opts = readSectionOpts(itemEl);
        const dx = e.clientX - start.x, dy = e.clientY - start.y;
        const stepX = Math.trunc((dx + Math.sign(dx) * 0.4 * ctx.m.cw) / ctx.m.cw);
        const stepY = Math.trunc((dy + Math.sign(dy) * 0.4 * ctx.m.ch) / ctx.m.ch);
        const dCol = opts.lockX ? 0 : stepX;
        const dRow = opts.lockY ? 0 : stepY;
        const { rows, cols } = ctx.m;
        const minc = Math.max(1, opts.minCol ?? 1);
        const maxc = Math.min(cols - ctx.me.colSpan + 1,
            Number.isFinite(opts.maxCol) ? opts.maxCol : cols);
        const minr = Math.max(1, opts.minRow ?? 1);
        const maxr = Math.min(rows - ctx.me.rowSpan + 1,
            Number.isFinite(opts.maxRow) ? opts.maxRow : rows);
        const target = {
            ...ctx.me,
            row: Math.max(minr, Math.min(maxr, ctx.me.row + dRow)),
            col: Math.max(minc, Math.min(maxc, ctx.me.col + dCol)),
        };

        const optsMap = new Map([[ctx.me.id, opts]]);
        let proposal = [target, ...ctx.others.map(o => ({ ...o }))];
        const gopts = readGridOpts(gridEl);
        if (gopts.resolveCollisions) {
            proposal = trySwapOrPush(proposal, target, rows, cols, optsMap);
        } else if (!canPlace(proposal, target)) {
            const nf = nearestFree(proposal, target, rows, cols);
            proposal = [nf, ...ctx.others.map(o => ({ ...o }))];
        }
        if (gopts.floatMode) proposal = floatUp(proposal, rows);

        lastProposal = proposal;
        // Clone follows the cursor (raw delta), placeholder snaps to the
        // resolved target cell so the user sees both intent + landing zone.
        const cursorPx = { x: px(ctx.me, ctx.m).x + dx, y: px(ctx.me, ctx.m).y + dy };
        ctx.clone.style.left = `${cursorPx.x}px`;
        ctx.clone.style.top = `${cursorPx.y}px`;
        placeRect(placeholder, px(target, ctx.m));

        for (const it of proposal) {
            const baseIt = ctx.base.find(x => x.id === it.id);
            if (!baseIt) continue;
            const src = px(baseIt, ctx.m), dst = px(it, ctx.m);
            it.el.style.transition = "transform 70ms ease";
            it.el.style.transform = `translate(${dst.x - src.x}px,${dst.y - src.y}px)`;
        }
    };

    const onUp = () => {
        if (!ctx) { start = null; return; }
        const gopts = readGridOpts(gridEl);
        let finalProposal = lastProposal || [];
        if (gopts.compactOnDrop) finalProposal = compactVertical(finalProposal);

        const cleanup = () => {
            clearTransforms(gridEl);
            ctx?.clone?.remove();
            placeholder.style.display = "none";
            itemEl.classList.remove("lt-container-grid-section--dragging");
            start = null; ctx = null; lastProposal = null;
            // Stop the rAF pump and run one final sync so chrome lands
            // on the committed CSS-Grid placement.
            stopBezelRafSync();
        };
        commit(gridEl, finalProposal).then(cleanup, cleanup);
    };

    handle.addEventListener("pointerdown", onDown);
    handle.addEventListener("pointermove", onMove);
    handle.addEventListener("pointerup", onUp);
    handle.addEventListener("pointercancel", onUp);
}

function wireResize(gridEl, itemEl, handles) {
    const overlay = gridEl.querySelector(".lt-container-grid-overlay");
    const placeholder = gridEl.querySelector(".lt-container-grid-placeholder");

    const attach = (handle, mode) => {
        if (!handle) return;
        let start = null, ctx = null, lastProposal = null;

        const onDown = e => {
            const opts = readSectionOpts(itemEl);
            if (!opts.resizable) return;
            e.stopPropagation();
            handle.setPointerCapture(e.pointerId);
            // Resize gestures animate sections via transform-translate
            // (push/swap of overlapping siblings) — same RO-blind animation
            // path as drag — so we run the same gesture-time bezel pump.
            startBezelRafSync();
            start = { x: e.clientX, y: e.clientY };
            const base = buildModel(gridEl);
            const me = base.find(x => x.el === itemEl);
            const others = base.filter(x => x !== me);
            const m = measure(gridEl);

            const clone = itemEl.cloneNode(true);
            clone.classList.add("lt-container-grid-clone");
            clone.style.position = "absolute";
            clone.style.pointerEvents = "none";
            clone.style.zIndex = 30;
            clone.style.gridColumn = "";
            clone.style.gridRow = "";
            overlay.appendChild(clone);
            placeRect(clone, px(me, m));
            placeholder.style.display = "block";
            placeRect(placeholder, px(me, m));
            ctx = { base, me, others, m, clone };
        };

        const onMove = e => {
            if (!start || !ctx) return;
            const opts = readSectionOpts(itemEl);
            const dx = e.clientX - start.x, dy = e.clientY - start.y;
            const stepCols = (mode === "s") ? 0 :
                Math.trunc((dx + Math.sign(dx) * 0.4 * ctx.m.cw) / ctx.m.cw);
            const stepRows = (mode === "e") ? 0 :
                Math.trunc((dy + Math.sign(dy) * 0.4 * ctx.m.ch) / ctx.m.ch);
            const { rows, cols } = ctx.m;
            let newW = ctx.me.colSpan + stepCols;
            let newH = ctx.me.rowSpan + stepRows;
            newW = Math.max(opts.minColSpan ?? 1, Math.min(opts.maxColSpan ?? cols, newW));
            newH = Math.max(opts.minRowSpan ?? 1, Math.min(opts.maxRowSpan ?? rows, newH));
            newW = Math.min(newW, cols - ctx.me.col + 1);
            newH = Math.min(newH, rows - ctx.me.row + 1);
            const target = { ...ctx.me, colSpan: newW, rowSpan: newH };

            const optsMap = new Map([[ctx.me.id, opts]]);
            let proposal = [target, ...ctx.others.map(o => ({ ...o }))];
            const gopts = readGridOpts(gridEl);
            if (gopts.resolveCollisions) {
                proposal = trySwapOrPush(proposal, target, rows, cols, optsMap);
            } else if (!canPlace(proposal, target)) {
                const nf = nearestFree(proposal, target, rows, cols);
                proposal = [nf, ...ctx.others.map(o => ({ ...o }))];
            }
            if (gopts.floatMode) proposal = floatUp(proposal, rows);

            lastProposal = proposal;
            const tpx = px(target, ctx.m);
            placeRect(ctx.clone, tpx);
            placeRect(placeholder, tpx);
            for (const it of proposal) {
                const baseIt = ctx.base.find(x => x.id === it.id);
                if (!baseIt) continue;
                const src = px(baseIt, ctx.m), dst = px(it, ctx.m);
                it.el.style.transition = "transform 70ms ease";
                it.el.style.transform = `translate(${dst.x - src.x}px,${dst.y - src.y}px)`;
            }
        };

        const onUp = () => {
            if (!ctx) { start = null; return; }
            const gopts = readGridOpts(gridEl);
            let finalProposal = lastProposal || [];
            if (gopts.compactOnDrop) finalProposal = compactVertical(finalProposal);

            const cleanup = () => {
                clearTransforms(gridEl);
                ctx?.clone?.remove();
                placeholder.style.display = "none";
                start = null; ctx = null; lastProposal = null;
                stopBezelRafSync();
            };
            commit(gridEl, finalProposal).then(cleanup, cleanup);
        };

        handle.addEventListener("pointerdown", onDown);
        handle.addEventListener("pointermove", onMove);
        handle.addEventListener("pointerup", onUp);
        handle.addEventListener("pointercancel", onUp);
    };

    const [hE, hS, hSE] = handles || [];
    attach(hE, "e");
    attach(hS, "s");
    attach(hSE, "se");
}

// ── Bezel host portal + positioning ───────────────────────────────────────
// The bezel host renders inside the section (so bUnit drift-guard tests
// that query [data-id='…'] [data-slot='container-grid-bezel-…'] still
// resolve), but on attach we *teleport* it to the root popover-provider
// div so it escapes:
//   1. the design pane's overflow:hidden / overflow:auto clipping (which
//      was chopping resize handles + edge badges that protrude outside
//      the section box via translate(50%) transforms);
//   2. any transformed ancestor that would reparent a `position: fixed`
//      element (Tailwind transform-gpu / split-pane wrappers / etc.) —
//      the popover-provider sits at `<body>` level so its descendants
//      have no transformed ancestors.
//
// After teleport the host loses its CSS-descendant relationship to the
// section, so the visibility rules can no longer say
// `.lt-container-grid-section:hover .lt-bezel-toolbar`. Instead we mirror
// the section's class set onto the host (data-section-classes) and pipe
// :hover/:focus-within through pointer/focus listeners as host classes
// (lt-bezel-host--hovered / --focused). The CSS rules in container-grid.css
// match on those instead.

function portalRoot() {
    return document.getElementById("lt-popover-provider")
        || document.body;
}

function syncBezelHost(sectionEl, hostEl) {
    const r = sectionEl.getBoundingClientRect();
    hostEl.style.position = "fixed";
    hostEl.style.top    = r.top    + "px";
    hostEl.style.left   = r.left   + "px";
    hostEl.style.width  = r.width  + "px";
    hostEl.style.height = r.height + "px";
}

// rAF pump — keeps every portaled bezel host bbox-synced to its section
// while a drag/resize gesture is in flight. ResizeObserver fires on
// dimension changes but NOT on `transform: translate(…)`, which is how
// wireDrag.onMove animates moving sections — so without this pump the
// chrome lags behind the section the moment the user starts dragging.
// Capture-phase scroll + window resize already cover idle bbox drift;
// this pump is purely the gesture-time supplement.
let _bezelRafHandle = 0;
let _bezelRafActive = 0;

function syncAllBezels() {
    document
        .querySelectorAll(".lt-container-grid-section")
        .forEach(sec => {
            const state = sec._bezelSync;
            if (state && state.hostEl) {
                syncBezelHost(sec, state.hostEl);
            }
        });
}

function startBezelRafSync() {
    _bezelRafActive++;
    if (_bezelRafHandle) return;
    const tick = () => {
        syncAllBezels();
        _bezelRafHandle = requestAnimationFrame(tick);
    };
    _bezelRafHandle = requestAnimationFrame(tick);
}

function stopBezelRafSync() {
    _bezelRafActive = Math.max(0, _bezelRafActive - 1);
    if (_bezelRafActive > 0) return;
    if (_bezelRafHandle) {
        cancelAnimationFrame(_bezelRafHandle);
        _bezelRafHandle = 0;
    }
    // Final sync after the gesture ends so the chrome lands on the
    // committed bbox (the transforms get cleared in clearTransforms,
    // and the section snaps to its CSS-Grid placement — we want the
    // chrome to follow that final landing).
    syncAllBezels();
}

function mirrorSectionClasses(sectionEl, hostEl) {
    hostEl.dataset.sectionClasses = [...sectionEl.classList].join(" ");
}

function attachBezel(sectionEl) {
    const hostEl = sectionEl.querySelector(".lt-bezel-host");
    if (!hostEl) return;

    // Teleport into the popover provider portal so the host escapes any
    // overflow / transform ancestor on the section's normal DOM path.
    const root = portalRoot();
    if (root && hostEl.parentNode !== root) {
        hostEl._bezelOriginalParent = hostEl.parentNode;
        root.appendChild(hostEl);
    }

    const sync = () => syncBezelHost(sectionEl, hostEl);
    const mirror = () => mirrorSectionClasses(sectionEl, hostEl);
    sync();
    mirror();

    // Resize/layout changes on the section → re-sync bbox.
    const ro = new ResizeObserver(sync);
    ro.observe(sectionEl);

    // Class changes on the section (e.g. --selected toggling) → mirror to host.
    const mo = new MutationObserver(mirror);
    mo.observe(sectionEl, { attributes: true, attributeFilter: ["class"] });

    // Capture-phase scroll catches *any* scrolling ancestor (the design
    // pane's overflow:auto wrapper, page scroll, etc.). window resize
    // catches viewport changes.
    const onScroll = sync;
    const onResize = sync;
    window.addEventListener("scroll", onScroll, true);
    window.addEventListener("resize", onResize);

    // :hover / :focus-within can't traverse the portal boundary, so
    // simulate them by listening on the section and toggling host classes
    // that the CSS rules match against.
    const onPointerEnter = () => hostEl.classList.add("lt-bezel-host--hovered");
    const onPointerLeave = () => hostEl.classList.remove("lt-bezel-host--hovered");
    const onFocusIn      = () => hostEl.classList.add("lt-bezel-host--focused");
    const onFocusOut     = () => hostEl.classList.remove("lt-bezel-host--focused");
    sectionEl.addEventListener("pointerenter", onPointerEnter);
    sectionEl.addEventListener("pointerleave", onPointerLeave);
    sectionEl.addEventListener("focusin",  onFocusIn);
    sectionEl.addEventListener("focusout", onFocusOut);

    sectionEl._bezelSync = {
        ro, mo, onScroll, onResize,
        onPointerEnter, onPointerLeave, onFocusIn, onFocusOut, hostEl,
    };
}

function detachBezel(sectionEl) {
    const state = sectionEl._bezelSync;
    if (!state) return;
    state.ro.disconnect();
    state.mo.disconnect();
    window.removeEventListener("scroll", state.onScroll, true);
    window.removeEventListener("resize", state.onResize);
    sectionEl.removeEventListener("pointerenter", state.onPointerEnter);
    sectionEl.removeEventListener("pointerleave", state.onPointerLeave);
    sectionEl.removeEventListener("focusin",  state.onFocusIn);
    sectionEl.removeEventListener("focusout", state.onFocusOut);

    // Move the bezel host back to its original DOM parent so Razor's
    // diffing can dispose the node cleanly when the section unmounts.
    const hostEl = state.hostEl;
    const original = hostEl?._bezelOriginalParent;
    if (hostEl && original && hostEl.parentNode !== original) {
        try { original.appendChild(hostEl); } catch { /* parent gone */ }
    }
    delete sectionEl._bezelSync;
}

// ── Public API (matches what ContainerGrid / ContainerGridSection invoke) ──

export const containerGrid = {
    bind(el, dotnet, rows, columns) {
        if (!el) return;
        el._containerGrid = { dotnet, rows, columns };
        // Mirror grid options onto data-options so per-section JS can read them.
        // (rows/columns are also written as data-rows/data-cols by the C# side.)
    },

    unbind(el) {
        if (!el || !el._containerGrid) return;
        delete el._containerGrid;
    },

    bindSection(el, dotnet, options) {
        if (!el) return;
        // Attach options as JSON for pointer handlers to read on each event.
        try { el.dataset.options = JSON.stringify(options || {}); } catch { }
        el._containerGridSection = { dotnet, options };

        const gridEl = el.closest(".lt-container-grid");
        if (!gridEl) return;

        if (options?.movable) {
            const grip = el.querySelector(".lt-container-grid-grip");
            wireDrag(gridEl, el, grip);
        }
        if (options?.resizable) {
            const hE  = el.querySelector(".lt-handle-e");
            const hS  = el.querySelector(".lt-handle-s");
            const hSE = el.querySelector(".lt-handle-se");
            wireResize(gridEl, el, [hE, hS, hSE]);
        }

        // Portal the bezel host into the root popover-provider div and
        // wire bbox sync + class mirroring. See attachBezel for rationale.
        attachBezel(el);
    },

    unbindSection(el) {
        if (!el || !el._containerGridSection) return;
        detachBezel(el);
        delete el._containerGridSection;
    },
};
