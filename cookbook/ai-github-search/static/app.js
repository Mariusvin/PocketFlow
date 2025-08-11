async function search(query) {
  const url = `/api/search?q=${encodeURIComponent(query)}`;
  const r = await fetch(url);
  if (!r.ok) throw new Error("Search failed");
  return r.json();
}

function render(results) {
  const container = document.getElementById("results");
  container.innerHTML = "";
  if (!results || results.length === 0) {
    container.innerHTML = `<div class="text-slate-500">No results.</div>`;
    return;
  }

  for (const r of results) {
    const card = document.createElement("a");
    card.href = r.url;
    card.target = "_blank";
    card.rel = "noopener";
    card.className = "block p-4 rounded-xl bg-white ring-1 ring-slate-200 hover:ring-slate-300 hover:shadow transition";
    card.innerHTML = `
      <div class="flex items-start justify-between">
        <h3 class="font-semibold text-slate-800">${r.name || "repo"}</h3>
        <span class="text-xs px-2 py-1 rounded bg-amber-50 text-amber-700 ring-1 ring-amber-200">★ ${r.stars ?? 0}</span>
      </div>
      <p class="mt-2 text-sm text-slate-600">${(r.description || "").slice(0, 160)}</p>
      <div class="mt-3 text-xs text-slate-500">${r.language || ""}</div>
    `;
    container.appendChild(card);
  }
}

function setQueries(qs) {
  const el = document.getElementById("queries");
  if (!qs || qs.length === 0) {
    el.textContent = "";
    return;
  }
  el.textContent = `Queries: ${qs.join(" | ")}`;
}

function setStatus(message, type = "info") {
  const el = document.getElementById("status");
  if (!el) return;
  if (!message) {
    el.textContent = "";
    el.className = "text-xs mt-1";
    return;
  }
  const color = type === "error" ? "text-rose-600" : type === "warn" ? "text-amber-600" : "text-slate-500";
  el.textContent = message;
  el.className = `text-xs mt-1 ${color}`;
}

document.getElementById("btn").addEventListener("click", async () => {
  const q = document.getElementById("q").value.trim();
  if (!q) return;
  const btn = document.getElementById("btn");
  btn.disabled = true;
  btn.textContent = "Searching...";
  try {
    const data = await search(q);
    setStatus("");
    setQueries(data.gh_queries);
    render(data.results);
  } catch (e) {
    setStatus("Search failed. Please try again.", "error");
    render([]);
  } finally {
    btn.disabled = false;
    btn.textContent = "Search";
  }
});

document.getElementById("q").addEventListener("keydown", (e) => {
  if (e.key === "Enter") {
    document.getElementById("btn").click();
  }
});


