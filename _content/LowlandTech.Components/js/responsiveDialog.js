// ResponsiveDialog JavaScript module for viewport detection
// Uses matchMedia API for efficient breakpoint monitoring

let mediaQueryList = null;
let dotNetRef = null;
let breakpoint = 768;

/**
 * Initializes the responsive dialog with viewport detection
 * @param {DotNetObjectReference} dotNetObject - The .NET object reference for callbacks
 * @param {number} mobileBreakpoint - The breakpoint in pixels for mobile (default: 768)
 */
export function initialize(dotNetObject, mobileBreakpoint = 768) {
    dotNetRef = dotNetObject;
    breakpoint = mobileBreakpoint;

    // Use matchMedia for efficient breakpoint detection
    mediaQueryList = window.matchMedia(`(max-width: ${breakpoint - 1}px)`);

    // Initial check
    handleBreakpointChange(mediaQueryList);

    // Listen for changes
    if (mediaQueryList.addEventListener) {
        mediaQueryList.addEventListener('change', handleBreakpointChange);
    } else {
        // Fallback for older browsers
        mediaQueryList.addListener(handleBreakpointChange);
    }
}

/**
 * Handles breakpoint changes and notifies .NET
 * @param {MediaQueryListEvent} e - The media query list event
 */
function handleBreakpointChange(e) {
    if (dotNetRef) {
        const isMobile = e.matches;
        dotNetRef.invokeMethodAsync('OnBreakpointChanged', isMobile);
    }
}

/**
 * Gets the current viewport state
 * @returns {boolean} True if viewport is mobile size
 */
export function getIsMobile() {
    return mediaQueryList ? mediaQueryList.matches : window.innerWidth < breakpoint;
}

/**
 * Disposes the viewport detection
 */
export function dispose() {
    if (mediaQueryList) {
        if (mediaQueryList.removeEventListener) {
            mediaQueryList.removeEventListener('change', handleBreakpointChange);
        } else {
            // Fallback for older browsers
            mediaQueryList.removeListener(handleBreakpointChange);
        }
        mediaQueryList = null;
    }
    dotNetRef = null;
}
