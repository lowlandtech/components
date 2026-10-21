export function focusNext(container, index) {
    if (!container) return;
    const inputs = container.querySelectorAll('input');
    const next = inputs[index + 1];
    if (next) {
        next.focus();
        next.select();
    }
}

export function focusPrevious(container, index) {
    if (!container) return;
    const inputs = container.querySelectorAll('input');
    const prev = inputs[index - 1];
    if (prev) {
        prev.focus();
        prev.select();
    }
}
