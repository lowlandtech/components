const activeAreas = new Map();

function updateThumb(viewport, thumb, track, orientation) {
    if (!viewport || !thumb || !track) return;

    const isHorizontal = orientation === 'horizontal';
    const clientSize = isHorizontal ? viewport.clientWidth : viewport.clientHeight;
    const scrollSize = isHorizontal ? viewport.scrollWidth : viewport.scrollHeight;
    const trackSize = isHorizontal ? track.clientWidth : track.clientHeight;

    if (scrollSize <= clientSize) {
        thumb.style.display = 'none';
        return;
    }

    thumb.style.display = 'block';
    const minThumbSize = 20;
    const thumbSize = Math.max(minThumbSize, (clientSize / scrollSize) * trackSize);
    const scrollPos = isHorizontal ? viewport.scrollLeft : viewport.scrollTop;
    const maxScroll = scrollSize - clientSize;
    const maxThumbPos = trackSize - thumbSize;
    const thumbPos = maxScroll > 0 ? (scrollPos / maxScroll) * maxThumbPos : 0;

    if (isHorizontal) {
        thumb.style.width = thumbSize + 'px';
        thumb.style.height = '100%';
        thumb.style.left = thumbPos + 'px';
        thumb.style.top = '0';
    } else {
        thumb.style.height = thumbSize + 'px';
        thumb.style.width = '100%';
        thumb.style.top = thumbPos + 'px';
        thumb.style.left = '0';
    }
}

function addDragSupport(viewport, thumb, track, orientation) {
    const isHorizontal = orientation === 'horizontal';
    let dragging = false;
    let startPointer = 0;
    let startScroll = 0;

    thumb.style.cursor = 'grab';

    thumb.addEventListener('pointerdown', e => {
        e.preventDefault();
        dragging = true;
        startPointer = isHorizontal ? e.clientX : e.clientY;
        startScroll = isHorizontal ? viewport.scrollLeft : viewport.scrollTop;
        thumb.style.cursor = 'grabbing';
        thumb.setPointerCapture(e.pointerId);
    });

    thumb.addEventListener('pointermove', e => {
        if (!dragging) return;
        const clientSize = isHorizontal ? viewport.clientWidth : viewport.clientHeight;
        const scrollSize = isHorizontal ? viewport.scrollWidth : viewport.scrollHeight;
        const trackSize = isHorizontal ? track.clientWidth : track.clientHeight;
        const thumbSize = isHorizontal ? thumb.offsetWidth : thumb.offsetHeight;
        const maxThumbPos = trackSize - thumbSize;
        const maxScroll = scrollSize - clientSize;
        const delta = (isHorizontal ? e.clientX : e.clientY) - startPointer;
        const scrollDelta = maxThumbPos > 0 ? (delta / maxThumbPos) * maxScroll : 0;
        if (isHorizontal) {
            viewport.scrollLeft = startScroll + scrollDelta;
        } else {
            viewport.scrollTop = startScroll + scrollDelta;
        }
    });

    thumb.addEventListener('pointerup', e => {
        dragging = false;
        thumb.style.cursor = 'grab';
        thumb.releasePointerCapture(e.pointerId);
    });

    // Click on track to jump
    track.addEventListener('pointerdown', e => {
        if (e.target === thumb) return;
        const rect = track.getBoundingClientRect();
        const clientSize = isHorizontal ? viewport.clientWidth : viewport.clientHeight;
        const scrollSize = isHorizontal ? viewport.scrollWidth : viewport.scrollHeight;
        const trackSize = isHorizontal ? track.clientWidth : track.clientHeight;
        const clickPos = isHorizontal ? e.clientX - rect.left : e.clientY - rect.top;
        const ratio = trackSize > 0 ? clickPos / trackSize : 0;
        const maxScroll = scrollSize - clientSize;
        if (isHorizontal) {
            viewport.scrollLeft = ratio * maxScroll;
        } else {
            viewport.scrollTop = ratio * maxScroll;
        }
    });
}

export function init(areaId, viewport, scrollbar, thumb, track, orientation) {
    if (!viewport || !scrollbar || !thumb) return;

    const onScroll = () => updateThumb(viewport, thumb, track || scrollbar, orientation);
    viewport.addEventListener('scroll', onScroll, { passive: true });

    const resizeObs = new ResizeObserver(onScroll);
    resizeObs.observe(viewport);
    resizeObs.observe(scrollbar);

    addDragSupport(viewport, thumb, track || scrollbar, orientation);

    // Redirect vertical wheel to horizontal scroll for horizontal orientation
    if (orientation === 'horizontal') {
        const onWheel = e => {
            if (e.shiftKey) return; // user is already doing horizontal scroll
            e.preventDefault();
            viewport.scrollLeft += e.deltaY !== 0 ? e.deltaY : e.deltaX;
        };
        viewport.addEventListener('wheel', onWheel, { passive: false });
        activeAreas.set(areaId, { onScroll, resizeObs, onWheel, viewport });
    } else {
        activeAreas.set(areaId, { onScroll, resizeObs });
    }

    // Initial sync
    onScroll();
}

export function destroy(areaId) {
    const state = activeAreas.get(areaId);
    if (!state) return;
    state.resizeObs.disconnect();
    if (state.onWheel && state.viewport) {
        state.viewport.removeEventListener('wheel', state.onWheel);
    }
    activeAreas.delete(areaId);
}
