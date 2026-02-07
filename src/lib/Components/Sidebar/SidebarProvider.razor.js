const SIDEBAR_COOKIE_NAME = 'sidebar_state';
const SIDEBAR_COOKIE_MAX_AGE = 60 * 60 * 24 * 7; // 7 days

let dotNetRef = null;
let keyboardShortcut = 'b';
let mediaQuery = null;

export function initialize(element, dotNetReference, shortcut) {
    dotNetRef = dotNetReference;
    keyboardShortcut = shortcut;

    // Set up keyboard shortcut listener
    document.addEventListener('keydown', handleKeyDown);

    // Set up mobile detection
    mediaQuery = window.matchMedia('(max-width: 768px)');
    mediaQuery.addEventListener('change', handleMediaChange);
}

function handleKeyDown(event) {
    if (event.key === keyboardShortcut && (event.metaKey || event.ctrlKey)) {
        event.preventDefault();
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnKeyboardShortcut');
        }
    }
}

function handleMediaChange(e) {
    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnMobileChange', e.matches);
    }
}

export function isMobile() {
    return window.matchMedia('(max-width: 768px)').matches;
}

export function saveState(open) {
    document.cookie = `${SIDEBAR_COOKIE_NAME}=${open}; path=/; max-age=${SIDEBAR_COOKIE_MAX_AGE}`;
}

export function dispose() {
    document.removeEventListener('keydown', handleKeyDown);
    if (mediaQuery) {
        mediaQuery.removeEventListener('change', handleMediaChange);
    }
    dotNetRef = null;
}
