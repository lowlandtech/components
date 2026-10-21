import { chromium } from 'playwright';
import { strict as assert } from 'node:assert';
import { createServer } from 'node:http';
import { readFile } from 'node:fs/promises';
import { resolve, extname, sep } from 'node:path';

const root = resolve(process.argv[2] ?? '');
const mime = { '.html': 'text/html', '.js': 'text/javascript', '.mjs': 'text/javascript', '.css': 'text/css', '.json': 'application/json', '.wasm': 'application/wasm', '.svg': 'image/svg+xml', '.png': 'image/png' };
const server = createServer(async (req, res) => {
    const path = resolve(root, '.' + decodeURIComponent(new URL(req.url, 'http://localhost').pathname));
    if (path !== root && !path.startsWith(root + sep)) { res.writeHead(403).end(); return; }
    let file = path;
    let body;
    try { body = await readFile(file); }
    catch { file = resolve(root, 'index.html'); body = await readFile(file); }
    res.setHeader('Content-Type', mime[extname(file)] ?? 'application/octet-stream');
    res.end(body);
});
await new Promise(r => server.listen(0, '127.0.0.1', r));
const base = `http://127.0.0.1:${server.address().port}`;
const browser = await chromium.launch({ channel: 'chrome', headless: true });
let context;
try {
    context = await browser.newContext();
    let page = await context.newPage();
    const group = name => page.getByRole('button', { name, exact: true });
    await page.goto(base);
    await group('Form').waitFor();
    // Given a fresh browser, then every group starts collapsed.
    assert.equal(await page.locator('[data-slot=sidebar-menu-button]').count(), 0);
    assert.equal(await group('Form').getAttribute('aria-expanded'), 'false');
    // When a group is opened and a story selected, then the URL follows it.
    await group('Form').click();
    await group('Button').click();
    await page.waitForURL('**/admin/components/button');
    await page.waitForFunction(() => JSON.parse(localStorage.getItem('showcode-navigation-v1')).Route.endsWith('/button'));
    // When the selected group is collapsed, then reload retains both route and collapse.
    await group('Form').click();
    await page.waitForFunction(() => !JSON.parse(localStorage.getItem('showcode-navigation-v1')).ExpandedGroups.includes('Form'));
    await page.reload();
    await group('Form').waitFor();
    assert.equal(await group('Form').getAttribute('aria-expanded'), 'false');
    assert.ok(page.url().endsWith('/admin/components/button'));
    await group('Form').click();
    await page.locator('[data-slot=sidebar-menu-button][data-active=true]').waitFor();
    assert.equal((await page.locator('[data-slot=sidebar-menu-button][data-active=true]').innerText()).trim(), 'Button');
    await page.waitForFunction(() => JSON.parse(localStorage.getItem('showcode-navigation-v1')).ExpandedGroups.includes('Form'));
    // Given persisted browser storage, then a new session restores the last route.
    const storageState = await context.storageState();
    await context.close();
    context = await browser.newContext({ storageState });
    page = await context.newPage();
    await page.goto(base);
    await page.waitForURL('**/admin/components/button');
    await page.locator('[data-slot=sidebar-menu-button][data-active=true]').waitFor();
    assert.equal(await group('Form').getAttribute('aria-expanded'), 'true');
    // An explicit shared URL wins over the saved selection.
    await page.goto(base + '/admin/components/card');
    await page.locator('[data-slot=sidebar-menu-button][data-active=true]').waitFor();
    assert.equal((await page.locator('[data-slot=sidebar-menu-button][data-active=true]').innerText()).trim(), 'Card');
    // Returning to the overview clears the saved story route.
    await page.getByRole('button', { name: 'All Components', exact: true }).click();
    await page.waitForURL('**/admin/components');
    await page.waitForFunction(() => JSON.parse(localStorage.getItem('showcode-navigation-v1')).Route === '/admin/components');
    await page.reload();
    await group('Form').waitFor();
    assert.equal(await page.locator('[data-slot=sidebar-menu-button][data-active=true]').count(), 0);
    console.log('PASS: collapsed defaults, selection routing, collapsed reload, new-session restoration, explicit route precedence, overview reset.');
} finally {
    await context?.close();
    await browser.close();
    await new Promise(r => server.close(r));
}
