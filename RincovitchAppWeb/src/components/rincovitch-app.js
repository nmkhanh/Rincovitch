import { hydrateLocalSettings, setLoading, setRole, setState, setTheme, setView, subscribe } from "../state/store";
import { loadAllData, subscribeRealtime } from "../services/data-service";

const NAV_ITEMS = [
  { id: "dashboard", label: "Dashboard", roles: ["user", "leader", "middle", "admin"] },
  { id: "timeline", label: "Timeline", roles: ["user", "leader", "middle", "admin"] },
  { id: "email", label: "Email", roles: ["leader", "middle", "admin"] },
  { id: "users", label: "Users", roles: ["admin"] },
  { id: "projects", label: "Projects", roles: ["leader", "middle", "admin"] },
  { id: "temporary", label: "Temporary", roles: ["user", "leader", "middle", "admin"] },
  { id: "schedule", label: "Schedule", roles: ["leader", "middle", "admin"] },
  { id: "notify", label: "Notify", roles: ["user", "leader", "middle", "admin"] },
  { id: "leave", label: "Leave", roles: ["user", "leader", "middle", "admin"] },
  { id: "settings", label: "Settings", roles: ["user", "leader", "middle", "admin"] }
];

class RincovitchApp extends HTMLElement {
  #unsubscribe = null;
  #unsubscribeRealtime = null;
  #state;

  connectedCallback() {
    hydrateLocalSettings();
    this.#unsubscribe = subscribe((state) => {
      this.#state = state;
      this.render();
    });
    this.bootstrap().catch((error) => {
      console.error(error);
      setLoading(false);
    });
  }

  disconnectedCallback() {
    if (this.#unsubscribe) this.#unsubscribe();
    if (this.#unsubscribeRealtime) this.#unsubscribeRealtime();
  }

  async bootstrap() {
    setLoading(true);
    const data = await loadAllData();
    setState(data);
    this.#unsubscribeRealtime = await subscribeRealtime((tableName, payload) => {
      const map = {
        NMK_User: "users",
        NMK_Project: "projects",
        NMK_Task: "tasks",
        NMK_Task_Temporary: "temporaryTasks",
        NMK_Leave: "leaves",
        NMK_Notify: "notifications"
      };
      const key = map[tableName];
      if (!key) return;
      const list = [...this.#state[key]];
      const row = payload.new || payload.old;
      const idx = list.findIndex((item) => item.id === row.id);
      if (payload.eventType === "DELETE") {
        if (idx >= 0) list.splice(idx, 1);
      } else if (idx >= 0) {
        list[idx] = payload.new;
      } else {
        list.unshift(payload.new);
      }
      setState({ [key]: list });
    });
    setLoading(false);
  }

  render() {
    if (!this.#state) return;
    const roleOptions = ["user", "leader", "middle", "admin"]
      .map((role) => `<option value="${role}" ${this.#state.role === role ? "selected" : ""}>${role}</option>`)
      .join("");
    const nav = NAV_ITEMS
      .filter((item) => item.roles.includes(this.#state.role))
      .map((item) => {
        const active = item.id === this.#state.currentView ? "active" : "";
        return `<button class="nav-item ${active}" data-view="${item.id}">${item.label}</button>`;
      })
      .join("");

    this.innerHTML = `
      <div class="app-shell">
        <aside class="side-nav">
          <div class="brand">RincovitchAppWeb</div>
          ${nav}
        </aside>
        <main class="content">
          <header class="top-bar">
            <div class="top-controls">
              <label>Role <select id="role">${roleOptions}</select></label>
              <button id="theme">${this.#state.theme === "dark" ? "Light" : "Dark"} Theme</button>
            </div>
            <div class="meta">${this.#state.loading ? "Loading..." : "Ready"}</div>
          </header>
          <section class="view-host">
            <module-view view="${this.#state.currentView}"></module-view>
          </section>
        </main>
      </div>
    `;

    this.querySelectorAll(".nav-item").forEach((button) => {
      button.addEventListener("click", () => setView(button.dataset.view));
    });
    this.querySelector("#theme")?.addEventListener("click", () => {
      setTheme(this.#state.theme === "dark" ? "light" : "dark");
    });
    this.querySelector("#role")?.addEventListener("change", (event) => {
      setRole(event.target.value);
    });
  }
}

customElements.define("rincovitch-app", RincovitchApp);

import "./views/module-view.js";
