// LowlandTech.Components JavaScript Utilities
// Provides JSInterop helpers for Blazor components

// Portal utilities - render content outside normal DOM hierarchy
export const portalUtils = {
    portals: new Map(),

    /**
     * Creates a portal container element in the target (defaults to document.body)
     * @param {string} portalId - Unique identifier for this portal
     * @param {string} target - CSS selector for target element (default: 'body')
     * @param {string} className - Optional CSS classes for the portal container
     * @returns {boolean} True if created successfully
     */
    create(portalId, target = 'body', className = '') {
        if (this.portals.has(portalId)) {
            console.warn(`Portal ${portalId} already exists`);
            return false;
        }

        const targetElement = document.querySelector(target);
        if (!targetElement) {
            console.error(`Portal target "${target}" not found`);
            return false;
        }

        const container = document.createElement('div');
        container.id = `portal-${portalId}`;
        if (className) {
            container.className = className;
        }
        container.setAttribute('data-portal', portalId);

        targetElement.appendChild(container);
        this.portals.set(portalId, container);

        return true;
    },

    /**
     * Updates portal content (currently no-op, content is managed by Blazor)
     * @param {string} portalId - The portal to update
     */
    update(portalId) {
        // Portal content is managed by Blazor's renderer
        // This method is here for future extensibility
        return this.portals.has(portalId);
    },

    /**
     * Destroys a portal and removes its container from the DOM
     * @param {string} portalId - The portal to destroy
     * @returns {boolean} True if destroyed successfully
     */
    destroy(portalId) {
        const container = this.portals.get(portalId);
        if (container && container.parentNode) {
            container.parentNode.removeChild(container);
            this.portals.delete(portalId);
            return true;
        }
        return false;
    },

    /**
     * Gets the portal container element
     * @param {string} portalId - The portal ID
     * @returns {HTMLElement|null} The portal container
     */
    getContainer(portalId) {
        return this.portals.get(portalId) || null;
    }
};

// Theme utilities - dark mode only
export const themeUtils = {
    /**
     * Gets the currently stored theme from localStorage
     * @returns {string|null} The stored theme or null (only "dark" is valid)
     */
    getTheme() {
        return localStorage.getItem('theme');
    },

    /**
     * Sets the theme in localStorage and applies it to document root
     * Only accepts "dark" theme - dark mode only
     * @param {string} theme - The theme to set (must be "dark")
     */
    setTheme(theme) {
        // Only honor "dark" theme
        if (theme === 'dark') {
            localStorage.setItem('theme', theme);
            this.applyTheme(theme);
        }
    },

    /**
     * Applies the dark theme to the document root
     * @param {string} theme - The theme to apply (only "dark" is honored)
     */
    applyTheme(theme) {
        const root = document.documentElement;
        
        // Dark mode only - always add dark class if theme is "dark"
        if (theme === 'dark') {
            root.classList.add('dark');
        } else {
            root.classList.remove('dark');
        }
    }
};

// Focus trap utilities - for modals and overlays
export const focusTrapUtils = {
    traps: new Map(),

    /**
     * Activates a focus trap within a container
     * @param {string} trapId - Unique identifier for this trap
     * @param {string} containerId - The container element ID
     * @returns {boolean} True if activated successfully
     */
    activate(trapId, containerId) {
        const container = document.getElementById(containerId);
        if (!container) {
            console.error(`Focus trap container "${containerId}" not found`);
            return false;
        }

        const focusableSelector = 'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])';
        const focusableElements = container.querySelectorAll(focusableSelector);

        if (focusableElements.length === 0) {
            console.warn(`No focusable elements found in container "${containerId}"`);
            return false;
        }

        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        const handleKeyDown = (e) => {
            if (e.key !== 'Tab') return;

            if (e.shiftKey) {
                // Shift + Tab
                if (document.activeElement === firstElement) {
                    e.preventDefault();
                    lastElement.focus();
                }
            } else {
                // Tab
                if (document.activeElement === lastElement) {
                    e.preventDefault();
                    firstElement.focus();
                }
            }
        };

        container.addEventListener('keydown', handleKeyDown);
        this.traps.set(trapId, { container, handler: handleKeyDown, firstElement });

        // Focus first element
        firstElement.focus();

        return true;
    },

    /**
     * Deactivates a focus trap
     * @param {string} trapId - The trap to deactivate
     * @returns {boolean} True if deactivated successfully
     */
    deactivate(trapId) {
        const trap = this.traps.get(trapId);
        if (trap) {
            trap.container.removeEventListener('keydown', trap.handler);
            this.traps.delete(trapId);
            return true;
        }
        return false;
    }
};

// Positioning utilities - for dropdowns and popovers
export const positionUtils = {
    /**
     * Positions an element relative to a reference element
     * @param {string} elementId - The element to position
     * @param {string} referenceId - The reference element
     * @param {string} placement - Desired placement (top|bottom|left|right)
     * @param {number} offset - Offset in pixels
     * @returns {object} The applied position { top, left }
     */
    position(elementId, referenceId, placement = 'bottom', offset = 0) {
        const element = document.getElementById(elementId);
        const reference = document.getElementById(referenceId);

        if (!element || !reference) {
            console.error('Position: element or reference not found');
            return null;
        }

        const refRect = reference.getBoundingClientRect();
        const elemRect = element.getBoundingClientRect();

        let top, left;

        switch (placement) {
            case 'top':
                top = refRect.top - elemRect.height - offset;
                left = refRect.left + (refRect.width - elemRect.width) / 2;
                break;
            case 'bottom':
                top = refRect.bottom + offset;
                left = refRect.left + (refRect.width - elemRect.width) / 2;
                break;
            case 'left':
                top = refRect.top + (refRect.height - elemRect.height) / 2;
                left = refRect.left - elemRect.width - offset;
                break;
            case 'right':
                top = refRect.top + (refRect.height - elemRect.height) / 2;
                left = refRect.right + offset;
                break;
            default:
                top = refRect.bottom + offset;
                left = refRect.left;
        }

        element.style.position = 'fixed';
        element.style.top = `${top}px`;
        element.style.left = `${left}px`;

        return { top, left };
    },

    /**
     * Checks if an element overflows the viewport
     * @param {string} elementId - The element to check
     * @returns {object} Overflow status { top, bottom, left, right }
     */
    checkOverflow(elementId) {
        const element = document.getElementById(elementId);
        if (!element) {
            return null;
        }

        const rect = element.getBoundingClientRect();
        const viewport = {
            width: window.innerWidth,
            height: window.innerHeight
        };

        return {
            top: rect.top < 0,
            bottom: rect.bottom > viewport.height,
            left: rect.left < 0,
            right: rect.right > viewport.width
        };
    }
};

// Export window globals for non-module usage
if (typeof window !== 'undefined') {
    window.LowlandTechComponents = {
        portalUtils,
        themeUtils,
        focusTrapUtils,
        positionUtils
    };
}
