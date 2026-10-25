// The protected action of the plugin page. The server authorizes it against
// the authority the server holds, so a browser projection that was rendered
// earlier cannot authorize it.
window.consumerPluginAction = async () => {
    const response = await fetch('/plugin-action', { method: 'POST' });
    return response.ok ? 'granted' : 'denied';
};
