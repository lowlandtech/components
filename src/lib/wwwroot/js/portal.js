// Portal JavaScript support for LowlandTech.Components

window.ltPortal = {
    moveToBody: function (portalId) {
        const portal = document.getElementById(portalId);
        if (portal && document.body) {
            document.body.appendChild(portal);
        }
    }
};
