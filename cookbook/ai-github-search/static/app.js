async function search(query, filters = {}) {
  const params = new URLSearchParams({ q: query });
  
  // Add filter parameters
  if (filters.language) params.append('language', filters.language);
  if (filters.minStars) params.append('min_stars', filters.minStars);
  if (filters.maxStars) params.append('max_stars', filters.maxStars);
  if (filters.updatedAfter) params.append('updated_after', filters.updatedAfter);
  if (filters.license) params.append('license', filters.license);
  
  const url = `/api/search?${params.toString()}`;
  const r = await fetch(url);
  if (!r.ok) throw new Error("Search failed");
  return r.json();
}

function render(results) {
  const container = document.getElementById("results");
  container.innerHTML = "";
  
  if (!results || results.length === 0) {
    container.innerHTML = `
      <div class="text-center py-16">
        <div class="inline-flex items-center justify-center w-16 h-16 bg-slate-100 rounded-full mb-4">
          <svg class="w-8 h-8 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
          </svg>
        </div>
        <h3 class="text-lg font-semibold text-slate-600 mb-2">No results found</h3>
        <p class="text-slate-500">Try adjusting your search terms or filters</p>
      </div>
    `;
    return;
  }

  // Create results grid
  const resultsGrid = document.createElement("div");
  resultsGrid.className = "grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6";
  
  for (const r of results) {
    const card = createResultCard(r);
    resultsGrid.appendChild(card);
  }
  
  container.appendChild(resultsGrid);
}

function createResultCard(repo) {
  const card = document.createElement("div");
  card.className = "bg-white/80 backdrop-blur-sm rounded-xl shadow-lg border border-white/20 hover:shadow-xl hover:scale-[1.02] transition-all duration-300 overflow-hidden";
  
  // Format date
  const updatedDate = repo.updated_at ? new Date(repo.updated_at).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }) : '';
  
  // Format size
  const formatSize = (size) => {
    if (size < 1024) return `${size} KB`;
    if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} MB`;
    return `${(size / (1024 * 1024)).toFixed(1)} GB`;
  };
  
  // Create language badge
  const languageBadge = repo.language ? `
    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
      ${repo.language}
    </span>
  ` : '';
  
  // Create license badge
  const licenseBadge = repo.license ? `
    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
      ${repo.license}
    </span>
  ` : '';
  
  card.innerHTML = `
    <div class="p-6">
      <!-- Header with repo name and stats -->
      <div class="flex items-start justify-between mb-4">
        <h3 class="text-lg font-bold text-slate-800 leading-tight">
          <a href="${repo.url}" target="_blank" rel="noopener" class="hover:text-blue-600 transition-colors">
            ${repo.name || "Repository"}
          </a>
        </h3>
        <div class="flex items-center space-x-2">
          <span class="inline-flex items-center px-2 py-1 rounded-lg text-sm font-medium bg-amber-100 text-amber-800">
            <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
            </svg>
            ${repo.stars ?? 0}
          </span>
          ${repo.forks ? `
            <span class="inline-flex items-center px-2 py-1 rounded-lg text-sm font-medium bg-blue-100 text-blue-800">
              <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
              </svg>
              ${repo.forks}
            </span>
          ` : ''}
        </div>
      </div>
      
      <!-- Description -->
      <p class="text-slate-600 mb-4 leading-relaxed">
        ${(repo.description || "No description available").slice(0, 120)}
        ${(repo.description || "").length > 120 ? '...' : ''}
      </p>
      
      <!-- Badges -->
      <div class="flex flex-wrap items-center gap-2 mb-4">
        ${languageBadge}
        ${licenseBadge}
      </div>
      
      <!-- Metadata -->
      <div class="grid grid-cols-2 gap-4 text-xs text-slate-500 border-t border-slate-200/50 pt-4">
        ${updatedDate ? `
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-2 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
            </svg>
            Updated: ${updatedDate}
          </div>
        ` : ''}
        ${repo.size ? `
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-2 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4"></path>
            </svg>
            Size: ${formatSize(repo.size)}
          </div>
        ` : ''}
        ${repo.open_issues ? `
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-2 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            Issues: ${repo.open_issues}
          </div>
        ` : ''}
      </div>
    </div>
  `;
  
  return card;
}

function setQueries(qs) {
  const el = document.getElementById("queries");
  if (!qs || qs.length === 0) {
    el.textContent = "";
    return;
  }
  el.innerHTML = `
    <div class="flex items-center">
      <svg class="w-4 h-4 mr-2 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
      </svg>
      <span class="font-medium text-slate-700">Generated Queries:</span>
      <span class="ml-2 text-slate-600">${qs.join(" | ")}</span>
    </div>
  `;
}

function setStatus(message, type = "info") {
  const el = document.getElementById("status");
  if (!el) return;
  if (!message) {
    el.textContent = "";
    el.className = "text-sm rounded-lg px-4 py-2";
    return;
  }
  
  let bgColor, textColor, icon;
  switch (type) {
    case "error":
      bgColor = "bg-red-100 text-red-800";
      textColor = "text-red-800";
      icon = `<svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
      </svg>`;
      break;
    case "warn":
      bgColor = "bg-amber-100 text-amber-800";
      textColor = "text-amber-800";
      icon = `<svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
      </svg>`;
      break;
    default:
      bgColor = "bg-blue-100 text-blue-800";
      textColor = "text-blue-800";
      icon = `<svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
      </svg>`;
  }
  
  el.innerHTML = `<div class="flex items-center ${bgColor} rounded-lg px-4 py-2">${icon}${message}</div>`;
}

function getFilters() {
  return {
    language: document.getElementById("language").value,
    minStars: document.getElementById("minStars").value || null,
    maxStars: document.getElementById("maxStars").value || null,
    updatedAfter: document.getElementById("updatedAfter").value || null,
    license: document.getElementById("license").value,
  };
}

function clearFilters() {
  document.getElementById("language").value = "";
  document.getElementById("minStars").value = "";
  document.getElementById("maxStars").value = "";
  document.getElementById("updatedAfter").value = "";
  document.getElementById("license").value = "";
}

function toggleFilters() {
  const controls = document.getElementById("filterControls");
  const toggleBtn = document.getElementById("toggleFilters");
  
  if (controls.classList.contains("hidden")) {
    controls.classList.remove("hidden");
    toggleBtn.textContent = "Hide Filters";
    toggleBtn.classList.add("text-blue-800");
  } else {
    controls.classList.add("hidden");
    toggleBtn.textContent = "Show Filters";
    toggleBtn.classList.remove("text-blue-800");
  }
}

// Event Listeners
document.addEventListener("DOMContentLoaded", function() {
  // Filter toggle
  document.getElementById("toggleFilters").addEventListener("click", toggleFilters);
  
  // Clear filters
  document.getElementById("clearFilters").addEventListener("click", clearFilters);
  
  // Apply filters
  document.getElementById("applyFilters").addEventListener("click", async () => {
    const q = document.getElementById("q").value.trim();
    if (!q) return;
    
    const btn = document.getElementById("btn");
    btn.disabled = true;
    btn.textContent = "Searching...";
    
    try {
      const filters = getFilters();
      const data = await search(q, filters);
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
  
  // Main search button
  document.getElementById("btn").addEventListener("click", async () => {
    const q = document.getElementById("q").value.trim();
    if (!q) return;
    
    const btn = document.getElementById("btn");
    btn.disabled = true;
    btn.textContent = "Searching...";
    
    try {
      const filters = getFilters();
      const data = await search(q, filters);
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
  
  // Enter key in search input
  document.getElementById("q").addEventListener("keydown", (e) => {
    if (e.key === "Enter") {
      document.getElementById("btn").click();
    }
  });
  
  // Enter key in filter inputs
  const filterInputs = ["minStars", "maxStars", "updatedAfter"];
  filterInputs.forEach(id => {
    document.getElementById(id).addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        document.getElementById("applyFilters").click();
      }
    });
  });
});


