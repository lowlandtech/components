window.ltPopover = {
    initialize() {
        // Click-outside handler for auto-closing is handled by Blazor
    },

    position(id, placement) {
        const anchor = document.querySelector(`[data-popover-anchor="${id}"]`) || document.getElementById(`popover-anchor-${id}`);
        const content = document.getElementById(`popover-content-${id}`);
        if (!anchor || !content) return;

        const anchorRect = anchor.getBoundingClientRect();
        const gap = 4;
        if (content.classList.contains('cn-select-popover')) {
            content.style.minWidth = `${anchorRect.width}px`;
        }

        // Measure content size with a temporary visibility trick
        const prevVisibility = content.style.visibility;
        const prevDisplay = content.style.display;
        content.style.visibility = 'hidden';
        content.style.display = '';
        const contentRect = content.getBoundingClientRect();
        content.style.visibility = prevVisibility;
        content.style.display = prevDisplay;

        let top = 0;
        let left = 0;

        // anchorRect values are already viewport-relative; content is position:fixed so use them directly
        switch (placement) {
            case 'topstart':
                top = anchorRect.top - contentRect.height - gap;
                left = anchorRect.left;
                break;
            case 'top':
                top = anchorRect.top - contentRect.height - gap;
                left = anchorRect.left + (anchorRect.width - contentRect.width) / 2;
                break;
            case 'topend':
                top = anchorRect.top - contentRect.height - gap;
                left = anchorRect.right - contentRect.width;
                break;
            case 'bottomstart':
                top = anchorRect.bottom + gap;
                left = anchorRect.left;
                break;
            case 'bottom':
                top = anchorRect.bottom + gap;
                left = anchorRect.left + (anchorRect.width - contentRect.width) / 2;
                break;
            case 'bottomend':
                top = anchorRect.bottom + gap;
                left = anchorRect.right - contentRect.width;
                break;
            case 'leftstart':
                top = anchorRect.top;
                left = anchorRect.left - contentRect.width - gap;
                break;
            case 'left':
                top = anchorRect.top + (anchorRect.height - contentRect.height) / 2;
                left = anchorRect.left - contentRect.width - gap;
                break;
            case 'leftend':
                top = anchorRect.bottom - contentRect.height;
                left = anchorRect.left - contentRect.width - gap;
                break;
            case 'rightstart':
                top = anchorRect.top;
                left = anchorRect.right + gap;
                break;
            case 'right':
                top = anchorRect.top + (anchorRect.height - contentRect.height) / 2;
                left = anchorRect.right + gap;
                break;
            case 'rightend':
                top = anchorRect.bottom - contentRect.height;
                left = anchorRect.right + gap;
                break;
        }

        // Viewport clamping (all values are viewport-relative)
        const vw = window.innerWidth;
        const vh = window.innerHeight;
        if (left + contentRect.width > vw) left = vw - contentRect.width - 8;
        if (left < 0) left = 8;
        if (top + contentRect.height > vh) top = vh - contentRect.height - 8;
        if (top < 0) top = 8;

        content.style.top = `${top}px`;
        content.style.left = `${left}px`;
    }
};
