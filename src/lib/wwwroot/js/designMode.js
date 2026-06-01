/**
 * Generic design-mode click support for StoryCard / canvas previews.
 * Attach to a preview container; clicks on any descendant with
 * [data-design-selectable="true"] are forwarded to the Blazor
 * TinyComponentBase.OnDesignElementClicked static method.
 *
 * A MutationObserver watches for dynamically added/removed designable
 * components so the preview acts as a live, discoverable collection.
 */

export function initPreviewContainer(container) {
    if (!container) return { dispose: () => {} };

    const updateSelection = () => {
        // Ensure selected state classes match the data attribute
        container.querySelectorAll('[data-design-selected="true"]').forEach(el => {
            el.classList.add('lt-design-selected');
        });
        container.querySelectorAll('[data-design-selected]:not([data-design-selected="true"])').forEach(el => {
            el.classList.remove('lt-design-selected');
        });
    };

    const clickHandler = (e) => {
        const el = e.target.closest('[data-design-selectable="true"]');
        if (!el) return;

        const instanceId = el.getAttribute('data-design-instance-id');
        if (!instanceId) return;

        DotNet.invokeMethodAsync('LowlandTech.Components', 'OnDesignElementClicked', instanceId);
    };

    const observer = new MutationObserver(() => updateSelection());
    observer.observe(container, { childList: true, subtree: true, attributes: true, attributeFilter: ['data-design-selected', 'data-design-selectable'] });

    // Initial scan
    updateSelection();
    container.addEventListener('click', clickHandler);

    return {
        dispose: () => {
            observer.disconnect();
            container.removeEventListener('click', clickHandler);
        }
    };
}
