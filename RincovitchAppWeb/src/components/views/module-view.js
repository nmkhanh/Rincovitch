import { subscribe } from "../../state/store";

class ModuleView extends HTMLElement {
  #unsubscribe = null;
  #state;

  connectedCallback() {
    this.#unsubscribe = subscribe((state) => {
      this.#state = state;
      this.render();
    });
  }

  disconnectedCallback() {
    if (this.#unsubscribe) this.#unsubscribe();
  }

  render() {
    if (!this.#state) return;
    const view = this.getAttribute("view") || this.#state.currentView;
    const cards = this.cardsFor(view);
    this.innerHTML = `
      <div class="module-page">
        <h2>${label(view)}</h2>
        <div class="cards">${cards}</div>
      </div>
    `;

    if (view === "timeline" || view === "schedule") {
      this.setupSyncScroll();
    }
  }

  cardsFor(view) {
    switch (view) {
      case "dashboard":
        return metricCards(this.#state);
      case "users":
        return listCards("Users", this.#state.users, ["fullname", "email", "role"]);
      case "projects":
        return listCards("Projects", this.#state.projects, ["name", "createby"]);
      case "temporary":
        return listCards("Temporary Tasks", this.#state.temporaryTasks, ["name", "status", "createby"]);
      case "notify":
        return listCards("Notifications", this.#state.notifications, ["name", "sendto", "isread"]);
      case "leave":
        return listCards("Leaves", this.#state.leaves, ["userid", "status", "datefrom"]);
      case "settings":
        return settingsCard(this.#state);
      case "email":
        return `<article class="card"><h3>Email Queue</h3><p>Module shell mapped from desktop Email panel.</p></article>`;
      case "timeline":
      case "schedule":
        return scheduleLikeView(this.#state.tasks);
      default:
        return `<article class="card"><p>Unknown view</p></article>`;
    }
  }

  setupSyncScroll() {
    const left = this.querySelector(".sync-left");
    const right = this.querySelector(".sync-right");
    if (!left || !right) return;
    let lock = false;
    left.addEventListener("scroll", () => {
      if (lock) return;
      lock = true;
      right.scrollTop = left.scrollTop;
      lock = false;
    });
    right.addEventListener("scroll", () => {
      if (lock) return;
      lock = true;
      left.scrollTop = right.scrollTop;
      lock = false;
    });
  }
}

function label(view) {
  return view[0].toUpperCase() + view.slice(1);
}

function metricCards(state) {
  return `
    <article class="card"><h3>Tasks</h3><p>${state.tasks.length}</p></article>
    <article class="card"><h3>Projects</h3><p>${state.projects.length}</p></article>
    <article class="card"><h3>Users</h3><p>${state.users.length}</p></article>
    <article class="card"><h3>Unread Notifications</h3><p>${state.notifications.filter((n) => !n.isread).length}</p></article>
  `;
}

function listCards(title, list, fields) {
  const rows = list
    .slice(0, 25)
    .map((item) => `<tr>${fields.map((field) => `<td>${String(item[field] ?? "")}</td>`).join("")}</tr>`)
    .join("");
  const headers = fields.map((field) => `<th>${field}</th>`).join("");
  return `
    <article class="card full">
      <h3>${title}</h3>
      <table><thead><tr>${headers}</tr></thead><tbody>${rows}</tbody></table>
    </article>
  `;
}

function settingsCard(state) {
  return `
    <article class="card">
      <h3>Settings</h3>
      <p>Theme: ${state.theme}</p>
      <p>Role preset: ${state.role}</p>
      <p>Desktop-only integrations are replaced with web-safe equivalents.</p>
    </article>
  `;
}

function scheduleLikeView(tasks) {
  const leftRows = tasks
    .slice(0, 60)
    .map((task) => `<div class="row">${task.name || task.id || "-"}</div>`)
    .join("");
  const rightRows = tasks
    .slice(0, 60)
    .map((task) => `<div class="row">${task.datefrom || ""} - ${task.dateto || ""}</div>`)
    .join("");

  return `
    <article class="card full">
      <h3>Grouped Timeline/Schedule</h3>
      <div class="sync-wrap">
        <div class="sync-left">${leftRows}</div>
        <div class="sync-right">${rightRows}</div>
      </div>
    </article>
  `;
}

customElements.define("module-view", ModuleView);
