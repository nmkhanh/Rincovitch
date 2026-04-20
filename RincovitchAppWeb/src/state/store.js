const state = {
  theme: "light",
  currentView: "dashboard",
  role: "user",
  loading: false,
  users: [],
  projects: [],
  tasks: [],
  temporaryTasks: [],
  leaves: [],
  notifications: [],
  settings: {
    timelineByUserProject: false
  }
};

const listeners = new Set();

export function getState() {
  return structuredClone(state);
}

export function setState(patch) {
  Object.assign(state, patch);
  listeners.forEach((callback) => callback(getState()));
}

export function subscribe(callback) {
  listeners.add(callback);
  callback(getState());
  return () => listeners.delete(callback);
}

export function setView(view) {
  setState({ currentView: view });
}

export function setTheme(theme) {
  localStorage.setItem("nmk_theme", theme);
  document.documentElement.dataset.theme = theme;
  setState({ theme });
}

export function hydrateLocalSettings() {
  const theme = localStorage.getItem("nmk_theme") || "light";
  document.documentElement.dataset.theme = theme;
  setState({ theme });
}

export function setRole(role) {
  setState({ role });
}

export function setLoading(loading) {
  setState({ loading });
}
