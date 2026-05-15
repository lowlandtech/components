const instances = new Map();

export function init(handleElement, dotNetRef, direction) {
    dispose(handleElement);

    let isDragging = false;
    let startPosition = 0;

    function onPointerMove(e) {
        if (!isDragging) return;
        const currentPosition = direction === 'horizontal' ? e.clientX : e.clientY;
        const delta = currentPosition - startPosition;
        dotNetRef.invokeMethodAsync('OnPointerMoveJs', delta);
    }

    function onPointerUp(e) {
        if (!isDragging) return;
        isDragging = false;
        document.removeEventListener('pointermove', onPointerMove);
        document.removeEventListener('pointerup', onPointerUp);
        document.removeEventListener('pointercancel', onPointerUp);
        dotNetRef.invokeMethodAsync('OnPointerUpJs');
    }

    function onPointerDown(e) {
        isDragging = true;
        startPosition = direction === 'horizontal' ? e.clientX : e.clientY;

        if (handleElement.setPointerCapture) {
            handleElement.setPointerCapture(e.pointerId);
        }

        document.addEventListener('pointermove', onPointerMove);
        document.addEventListener('pointerup', onPointerUp);
        document.addEventListener('pointercancel', onPointerUp);

        dotNetRef.invokeMethodAsync('OnPointerDownJs');
    }

    handleElement.addEventListener('pointerdown', onPointerDown);

    instances.set(handleElement, () => {
        handleElement.removeEventListener('pointerdown', onPointerDown);
        document.removeEventListener('pointermove', onPointerMove);
        document.removeEventListener('pointerup', onPointerUp);
        document.removeEventListener('pointercancel', onPointerUp);
    });
}

export function dispose(handleElement) {
    const cleanup = instances.get(handleElement);
    if (cleanup) {
        cleanup();
        instances.delete(handleElement);
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
