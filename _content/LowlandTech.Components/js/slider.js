export function setPointerCapture(element, pointerId) {
    if (element && element.setPointerCapture) {
        element.setPointerCapture(pointerId);
    }
}

export function releasePointerCapture(element, pointerId) {
    if (element && element.releasePointerCapture) {
        element.releasePointerCapture(pointerId);
    }
}

export function getBoundingClientRect(element) {
    if (!element) return null;
    const rect = element.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        width: rect.width,
        height: rect.height
    };
}
