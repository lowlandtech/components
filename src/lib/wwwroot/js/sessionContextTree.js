(function () {
    function getExplorerButtons() {
        const active = document.activeElement;
        if (!active?.matches('[data-slot="session-context-node"]')) return [];

        const explorer = active.closest('[data-slot="session-context-explorer"]');
        if (!explorer) return [];

        return Array.from(explorer.querySelectorAll('[data-slot="session-context-node"]'));
    }

    function focusButton(button) {
        if (!button) return;
        button.focus({ preventScroll: true });
        button.scrollIntoView({ block: 'nearest', inline: 'nearest' });
    }

    window.ltSessionContextTree = window.ltSessionContextTree || {};

    window.ltSessionContextTree.focusNext = function () {
        const buttons = getExplorerButtons();
        const active = document.activeElement;
        const index = buttons.indexOf(active);
        if (index >= 0 && index + 1 < buttons.length) {
            focusButton(buttons[index + 1]);
        }
    };

    window.ltSessionContextTree.focusPrevious = function () {
        const buttons = getExplorerButtons();
        const active = document.activeElement;
        const index = buttons.indexOf(active);
        if (index > 0) {
            focusButton(buttons[index - 1]);
        }
    };

    // Prevent arrow keys from scrolling the page / container while the user is
    // navigating the tree. Blazor still receives the keydown event for its own
    // handlers (expand/collapse/project).
    document.addEventListener('keydown', function (event) {
        if (!event.key.startsWith('Arrow')) return;
        if (event.target?.matches('[data-slot="session-context-node"]') ||
            event.target?.closest('[data-slot="session-context-node"]')) {
            event.preventDefault();
        }
    }, true);
})();
