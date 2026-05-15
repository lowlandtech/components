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

export function init(areaId, viewport, scrollbar, thumb, track, orientation) {
    if (!viewport || !scrollbar || !thumb) return;

    const onScroll = () => updateThumb(viewport, thumb, track || scrollbar, orientation);
    viewport.addEventListener('scroll', onScroll, { passive: true });

    const resizeObs = new ResizeObserver(onScroll);
    resizeObs.observe(viewport);
    resizeObs.observe(scrollbar);

    // Initial sync
    onScroll();

    activeAreas.set(areaId, { onScroll, resizeObs });
}

export function destroy(areaId) {
    const state = activeAreas.get(areaId);
    if (!state) return;
    state.resizeObs.disconnect();
    activeAreas.delete(areaId);
}
