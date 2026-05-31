export function scrollBy(element, amount) {
  if (!element) {
    return;
  }

  element.scrollBy({
    left: amount,
    behavior: "smooth",
  });
}

const observers = new WeakMap();

export function observeOverflow(element, dotNetRef) {
  if (!element || !dotNetRef) {
    return;
  }

  unobserveOverflow(element);

  let lastValue = null;
  const notify = () => {
    const isOverflowing = element.scrollWidth > element.clientWidth + 1;
    if (isOverflowing !== lastValue) {
      lastValue = isOverflowing;
      dotNetRef.invokeMethodAsync("OnTabsOverflowChanged", isOverflowing);
    }
  };

  const resizeObserver = new ResizeObserver(notify);
  resizeObserver.observe(element);

  const mutationObserver = new MutationObserver(notify);
  mutationObserver.observe(element, {
    childList: true,
    subtree: true,
    characterData: true,
  });

  observers.set(element, { resizeObserver, mutationObserver });
  requestAnimationFrame(notify);
}

export function isTextTruncated(element) {
  if (!element) {
    return false;
  }
  return element.scrollWidth > element.clientWidth;
}

export function unobserveOverflow(element) {
  const observer = observers.get(element);
  if (!observer) {
    return;
  }

  observer.resizeObserver.disconnect();
  observer.mutationObserver.disconnect();
  observers.delete(element);
}
