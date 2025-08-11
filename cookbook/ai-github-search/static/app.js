let spinnerTimer = null;
const SPINNER_FRAMES = ['/', '-', '\\', '|'];
let spinnerIdx = 0;
let lastReqStart = 0;

function startSpinner() {
  const s = document.getElementById('spinner');
  const caret = document.getElementById('titleCaret');
  if (s) s.classList.remove('hidden');
  if (caret) caret.textContent = '/';
  clearInterval(spinnerTimer);
  spinnerTimer = setInterval(() => {
    spinnerIdx = (spinnerIdx + 1) % SPINNER_FRAMES.length;
    if (s) s.textContent = SPINNER_FRAMES[spinnerIdx];
    if (caret) caret.textContent = SPINNER_FRAMES[spinnerIdx];
  }, 120);
}
function stopSpinner() {
  const s = document.getElementById('spinner');
  const caret = document.getElementById('titleCaret');
  clearInterval(spinnerTimer);
  if (s) s.classList.add('hidden');
  if (caret) caret.textContent = '>';
}

function setAsciiBar(pct, slots=8) {
  const filled = Math.max(0, Math.min(slots, Math.round((pct / 100) * slots)));
  return `[${'#'.repeat(filled)}${'.'.repeat(slots - filled)}] ${Math.round(pct)}%`;
}

function setProgressAscii(pct) {
  const el = document.getElementById('searchingAscii');
  if (el) el.textContent = setAsciiBar(pct, 20);
  const btnProg = document.getElementById('btnProgress');
  if (btnProg) btnProg.textContent = setAsciiBar(pct, 8);
}

function showBtnProgress(show) {
  const label = document.getElementById('btnLabel');
  const prog = document.getElementById('btnProgress');
  if (!label || !prog) return;
  if (show) { label.classList.add('hidden'); prog.classList.remove('hidden'); }
  else { label.classList.remove('hidden'); prog.classList.add('hidden'); }
}

function updateHud(ms, model) {
  const hud = document.getElementById('hudText');
  if (!hud) return;
  let msDisplay = '';
  if (ms > 100) msDisplay = (ms/1000).toFixed(2) + 's';
  else msDisplay = ms + 'ms';
  const fps = ms > 0 ? (1000 / ms).toFixed(1) : '--';
  const m = (model || 'gemini').toString();
  const line = ` model: ${m}  |  req: ${msDisplay}  |  fps: ${fps} `;
  const border = '+-' + '-'.repeat(line.length) + '-+';
  const text = `${border}\n| ${line} |\n${border}`;
  hud.textContent = text;
}

async function search(query) {
  const params = new URLSearchParams({ q: query });
  const url = `/api/search?${params.toString()}`;
  const r = await fetch(url);
  if (!r.ok) throw new Error("Search failed");
  return r.json();
}

const PROMPTS = [
  "turn a csv into charts",
  "find repos with readable code",
  "websocket chat example in go",
  "tiny http server in rust",
  "next.js blog starter minimal",
  "sqlite + prisma tutorial",
  "cloudflare worker kv example",
  "react hooks library small",
  "discord bot typescript minimal",
  "fastapi jwt auth example",
  "docker compose for postgres redis",
  "terraform module aws s3 simple",
  "github action build node pnpm",
  "python cli color output",
  "bash script rename files safely",
  "vue 3 pinia example",
  "sveltekit auth templates",
  "electron app auto update",
  "tailwind css forms theme",
  "c++ http client minimal",
  "swift ui list searchable",
  "kotlin coroutine retrofit sample",
  "android navigation compose",
  "ios push notification demo",
  "ruby on rails api only",
  "elixir phoenix liveview chat",
  "django simple blog with tags",
  "flask sqlite auth minimal",
  "go fiber rest api",
  "grpc gateway example",
  "webgl shader intro",
  "three.js orbit controls scene",
  "vitest react testing examples",
  "jest tsconfig monorepo setup",
  "pnpm workspace example",
  "eslint prettier strict template",
  "openai function calling demo",
  "langchain local embeddings",
  "ollama server api node",
  "webrtc p2p file transfer",
  "remix run auth cookie",
  "nuxt content blog starter",
  "astro mdx tailwind starter",
  "hono cloudflare e2e demo",
  "bun http server router",
  "k8s helm chart minimal",
  "nginx reverse proxy https",
  "redis stream worker example",
  "posthog analytics example",
  "stripe checkout minimal",
  "paypal subscription webhook"
];

function pickPrompt() { return PROMPTS[Math.floor(Math.random()*PROMPTS.length)]; }

function renderSearchSuggestions(strategies) {
  const container = document.getElementById("suggestionButtons");
  const suggestionsDiv = document.getElementById("searchSuggestions");
  if (!container || !suggestionsDiv) return;
  if (!strategies || strategies.length === 0) { suggestionsDiv.classList.add("hidden"); return; }
  container.innerHTML = "";
  strategies.forEach((s, idx) => {
    const button = document.createElement("button");
    button.className = `strategy-card${idx===0 ? ' active' : ''}`;
    const text = (s.query || s.title || s.text || '').toString();
    button.innerHTML = `<div class="strategy-query">${text}</div>`;
    button.addEventListener("click", async () => { highlightActiveStrategy(button); await executeSearchWithStrategy(text); });
    container.appendChild(button);
  });
  suggestionsDiv.classList.remove("hidden");
}

function highlightActiveStrategy(activeButton) { document.querySelectorAll('#suggestionButtons .strategy-card').forEach(btn => btn.classList.remove('active')); activeButton.classList.add('active'); }

function toggleSearching(on) { const el = document.getElementById('searching'); if (el) el.classList.toggle('hidden', !on); if (on) setProgressAscii(0); }

async function executeSearchWithStrategy(query) {
  const btn = document.getElementById('btn'); if (btn) btn.disabled = true;
  startSpinner(); showBtnProgress(true); setProgressAscii(0); lastReqStart = performance.now();
  try {
    setProgressAscii(10);
    const data = await search(query);

    setProgressAscii(40); renderSearchSuggestions(data.search_strategies || []);
    setProgressAscii(65); setQueries(data.gh_queries);
    setProgressAscii(90); render(data.results);

    setProgressAscii(100);
    const ms = Math.max(1, Math.round(performance.now() - lastReqStart));
    updateHud(ms, (data && data.model) || 'pocketflow');
  } catch (e) { setStatus(`Search failed: ${e.message}`, "error"); render([]); }
  finally { if (btn) btn.disabled = false; stopSpinner(); showBtnProgress(false); }
}

function render(results) {
  const container = document.getElementById("results");
  // Animate slide-down existing rows
  const existing = container.querySelectorAll('.result-row');
  existing.forEach(row => row.classList.add('slide-down'));
  setTimeout(() => { container.innerHTML = ""; 
    if (!results || results.length === 0) { document.body.classList.remove('has-results'); container.innerHTML = `<div style=\"opacity:.85;color:#5b616d;padding:.8rem 0;\">No results</div>`; return; }
    const list = document.createElement("div"); list.className = "results-list";
  for (const r of results) {
      const row = createResultRow(r);
      row.classList.add('hidden-row');
      list.appendChild(row);
    }
    container.appendChild(list);
    document.body.classList.add('has-results');
    requestAnimationFrame(() => {
      container.querySelectorAll('.result-row').forEach(row => row.classList.remove('hidden-row'));
    });
  }, 150);
}

function createResultRow(repo) {
  const row = document.createElement("div"); row.className = "result-row";
  const updatedDate = repo.updated_at ? new Date(repo.updated_at).toLocaleDateString('en-US', { year:'numeric', month:'short', day:'numeric' }) : '';
  const formatSize = (size) => size < 1024 ? `${size} KB` : size < 1024*1024 ? `${(size/1024).toFixed(1)} MB` : `${(size/(1024*1024)).toFixed(1)} GB`;
  const language = repo.language ? `<span class="badge">${repo.language}</span>` : '';
  const license = repo.license ? `<span class="badge">${repo.license}</span>` : '';
  const stars = repo.stars ?? 0; const forks = repo.forks ?? 0;
  row.innerHTML = `
    <div class="result-header">
      <div class="result-name"><a href="${repo.url}" target="_blank" rel="noopener">${repo.name || "Repository"}</a></div>
      <div>
        <span class="stat">[* ${stars}]</span>
        ${forks ? `<span class="stat">[f ${forks}]</span>` : ''}
        </div>
      </div>
    <div class="result-description">${(repo.description || "No description").slice(0, 180)}${(repo.description || '').length>180?'...':''}</div>
    <div>${language}${license}</div>
    <div class="result-meta">
      ${updatedDate ? `<div>[date] ${updatedDate}</div>` : ''}
      ${repo.size ? `<div>[size] ${formatSize(repo.size)}</div>` : ''}
      ${repo.open_issues ? `<div>[issues] ${repo.open_issues}</div>` : ''}
    </div>`; return row; }

function setQueries(qs) { const el = document.getElementById("queries"); if (!el) return; if (!qs || qs.length === 0) { el.textContent = ""; el.classList.add("hidden"); return; } el.innerHTML = `<span class="queries-label">Query:</span> <span class="queries-text">${qs.join("  |  ")}</span>`; el.classList.remove("hidden"); }

function setStatus(message, type = "info") { const el = document.getElementById("status"); if (!el) return; if (!message) { el.textContent = ""; el.classList.add("hidden"); return; } let accent = type === 'error' ? '#991b1b' : type === 'warn' ? '#92400e' : '#565d6b'; el.style.background = 'rgba(245,236,206,.8)'; el.style.color = accent; el.textContent = message; el.classList.remove("hidden"); }

// Move input caret visually using prompt + mirror
function syncInputCaret() {
  const input = document.getElementById('q');
  const mirror = document.getElementById('inputMirror');
  const caret = document.getElementById('inputCaret');
  const promptEl = document.querySelector('.prompt-label');
  if (!input || !mirror || !caret || !promptEl) return;
  const promptWidth = 64; // equal to input padding-left to align after "C:/"
  // If focused, track real caret position using selectionStart
  const raw = input.value || input.placeholder || '';
  let sel = raw.length;
  if (document.activeElement === input && typeof input.selectionStart === 'number') {
    sel = Math.max(0, input.selectionStart);
  }
  mirror.textContent = raw.slice(0, sel);
  const mirrorWidth = mirror.getBoundingClientRect().width;
  // small pixel tweak to account for subpixel rounding
  caret.style.left = (promptWidth + mirrorWidth + 1) + 'px';
}

document.addEventListener("DOMContentLoaded", function() {
  const searchBtn = document.getElementById("btn");
  const searchInput = document.getElementById("q");
  const mirror = document.getElementById('inputMirror');
  const caret = document.getElementById('inputCaret');
  // Force a paint to stabilize layout; guard against null
  (document.querySelector('.content') || document.body).offsetHeight;

  const ensurePlaceholder = (ev) => {
    if (!searchInput) return;
    // If focused and empty -> always set a fresh random placeholder
    if (document.activeElement === searchInput) {
      if (!searchInput.value) {
        searchInput.placeholder = pickPrompt();
      }
  } else {
      // On blur, if empty ensure placeholder exists so caret resets to it
      if (!searchInput.value) {
        if (!searchInput.placeholder) searchInput.placeholder = pickPrompt();
      }
    }
    // Sync caret to placeholder end when empty
    if (!searchInput.value) {
      mirror.textContent = searchInput.placeholder || '';
      const promptEl = document.querySelector('.prompt-label');
      const promptWidth = (promptEl ? promptEl.getBoundingClientRect().width : 24) + 16;
      caret.style.left = (promptWidth + mirror.getBoundingClientRect().width + 1) + 'px';
    }
  };

  ['focus','blur'].forEach(evt => searchInput && searchInput.addEventListener(evt, ensurePlaceholder));
  ['input','keyup','change','click','mouseup','keydown','blur'].forEach(evt => searchInput && searchInput.addEventListener(evt, syncInputCaret));
  ensurePlaceholder(); syncInputCaret();

  if (searchBtn) searchBtn.addEventListener("click", async (e) => {
    e.preventDefault();
    const q = (searchInput && searchInput.value.trim()) || '';
      if (!q) return;
    searchBtn.disabled = true; startSpinner(); showBtnProgress(true); setProgressAscii(10);
    try {
      const t0 = performance.now();
      const data = await search(q);
      const fetchMs = Math.max(1, Math.round(performance.now() - t0));
      setStatus(""); setProgressAscii(40); renderSearchSuggestions(data.search_strategies || []);
      setProgressAscii(65); setQueries(data.gh_queries);
      setProgressAscii(90); render(data.results);
      setProgressAscii(100); updateHud(fetchMs, (data && (data.model || data.llm_model)) || 'gemini');
    } catch (e) { setStatus("Search failed. Please try again.", "error"); render([]); }
    finally { searchBtn.disabled = false; stopSpinner(); showBtnProgress(false); }
  });
  // Enter submits search
  if (searchInput) searchInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      searchBtn && searchBtn.click();
    }
  });
});


