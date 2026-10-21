const instances = new Map();

export function init(elementId, dotNetRef, initialWidth, side = 'left', minWidth = 120, maxWidth = 1000) {
    dispose(elementId);

    const el = document.getElementById(elementId);
    if (!el) return;

    let isDragging = false;
    let dragStartX = 0;
    let dragStartWidth = initialWidth;
    const isRightSide = side === 'right';

    function getSizingContainer() {
        const parent = el.parentElement;
        if (!parent) return null;

        // Right-side splitters live inside a wrapper that only spans the sidebar.
        // Measure the full flex row so widening the sidebar can correctly reduce the content area.
        return isRightSide ? parent.parentElement ?? parent : parent;
    }

    function getDynamicMaxWidth() {
        // Measure the actual container that owns the available content width.
        // This is always correct regardless of app-sidebar state, transitions, or CSS
        // variable timing — it reads the true rendered width.
        const container = getSizingContainer();
        if (!container) return maxWidth;
        const containerWidth = Math.round(container.getBoundingClientRect().width);
        if (containerWidth <= 0) return maxWidth;

        // Keep at least 55% of the container (or 420px, whichever is larger) for the
        // main content area so the panel can never shove content off-canvas.
        const minContentWidth = Math.max(420, Math.floor(containerWidth * 0.55));
        return Math.max(minWidth, Math.min(maxWidth, containerWidth - minContentWidth));
    }

    function clamp(value) {
        const dynamicMax = getDynamicMaxWidth();
        return Math.max(minWidth, Math.min(dynamicMax, value));
    }

    function getCurrentWidth() {
        const panel = isRightSide ? el.nextElementSibling : el.previousElementSibling;
        if (panel && panel.getBoundingClientRect) {
            const panelWidth = Math.round(panel.getBoundingClientRect().width);
            if (panelWidth > 0) return panelWidth;
        }

        const parent = el.parentElement;
        if (!parent) return initialWidth;

        // Prefer explicit CSS width variables when present.
        const leftVar = parseInt(parent.style.getPropertyValue('--panel-width'), 10);
        if (!Number.isNaN(leftVar) && leftVar > 0) return leftVar;

        const rightVar = parseInt(parent.style.getPropertyValue('--right-panel-width'), 10);
        if (!Number.isNaN(rightVar) && rightVar > 0) return rightVar;

        const parentWidth = Math.round(parent.getBoundingClientRect().width);
        return parentWidth > 0 ? parentWidth : initialWidth;
    }

    function onPointerDown(e) {
        isDragging = true;
        dragStartX = e.clientX;
        dragStartWidth = clamp(getCurrentWidth());

        document.body.style.cursor = 'grabbing';
        document.body.style.userSelect = 'none';
        if (el.setPointerCapture && e.pointerId !== undefined) {
            el.setPointerCapture(e.pointerId);
        }
        e.preventDefault();
    }

    function onPointerMove(e) {
        if (!isDragging) return;

        const deltaX = e.clientX - dragStartX;
        const width = isRightSide
            ? clamp(dragStartWidth - deltaX)
            : clamp(dragStartWidth + deltaX);

        if (Number.isFinite(width)) {
            dotNetRef.invokeMethodAsync('OnPanelSplitterMove', Math.round(width));
        }
    }

    function onPointerUp() {
        if (!isDragging) return;
        isDragging = false;
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
        dotNetRef.invokeMethodAsync('OnPanelSplitterRelease');
    }

    function onKeyDown(e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 's') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OnKeyboardSave');
        }
    }

    el.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('pointermove', onPointerMove);
    document.addEventListener('pointerup', onPointerUp);
    document.addEventListener('pointercancel', onPointerUp);
    document.addEventListener('keydown', onKeyDown);

    // If the stored initialWidth already exceeds the dynamic max (e.g. viewport shrank
    // since it was saved), correct it immediately so content isn't pushed off-canvas.
    const clampedInitial = clamp(initialWidth);
    if (clampedInitial !== initialWidth) {
        dotNetRef.invokeMethodAsync('OnPanelSplitterMove', clampedInitial).then(() => {
            dotNetRef.invokeMethodAsync('OnPanelSplitterRelease');
        });
    }

    // ── Nav sidebar expand/collapse observer (left splitter only) ──────────────
    // When the app nav expands, the available content area shrinks. If the left panel
    // is currently wider than the new max, clamp it and save the original width so it
    // can be restored when the nav collapses again.
    let _widthBeforeNavExpand = null;
    let _navObserver = null;

    if (!isRightSide) {
        const htmlEl = document.documentElement;
        let _wasCollapsed = htmlEl.classList.contains('sidebar-collapsed');

        _navObserver = new MutationObserver(() => {
            const isCollapsed = htmlEl.classList.contains('sidebar-collapsed');
            if (isCollapsed === _wasCollapsed) return; // classList changed for unrelated reason
            _wasCollapsed = isCollapsed;

            // Read the current panel width from the CSS variable Blazor keeps updated.
            const currentWidth = parseInt(el.parentElement?.style.getPropertyValue('--panel-width'), 10)
                || getCurrentWidth();
            const newMax = getDynamicMaxWidth();

            if (!isCollapsed) {
                // Nav just EXPANDED — content area shrank.
                // Clamp the panel if it's now too wide, saving width for later restore.
                if (currentWidth > newMax) {
                    _widthBeforeNavExpand = currentWidth;
                    dotNetRef.invokeMethodAsync('OnPanelSplitterMove', newMax).then(() => {
                        dotNetRef.invokeMethodAsync('OnPanelSplitterRelease');
                    });
                }
            } else {
                // Nav just COLLAPSED — content area grew.
                // Restore the width that was saved when the nav expanded, if any.
                if (_widthBeforeNavExpand !== null) {
                    const restoreWidth = clamp(_widthBeforeNavExpand);
                    _widthBeforeNavExpand = null;
                    dotNetRef.invokeMethodAsync('OnPanelSplitterMove', restoreWidth).then(() => {
                        dotNetRef.invokeMethodAsync('OnPanelSplitterRelease');
                    });
                }
            }
        });

        _navObserver.observe(htmlEl, { attributes: true, attributeFilter: ['class'] });
    }

    instances.set(elementId, () => {
        el.removeEventListener('pointerdown', onPointerDown);
        document.removeEventListener('pointermove', onPointerMove);
        document.removeEventListener('pointerup', onPointerUp);
        document.removeEventListener('pointercancel', onPointerUp);
        document.removeEventListener('keydown', onKeyDown);
        _navObserver?.disconnect();
    });
}

export function dispose(elementId) {
    const cleanup = instances.get(elementId);
    if (cleanup) {
        cleanup();
        instances.delete(elementId);
    }
}
