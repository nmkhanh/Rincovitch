let cachedConfig;

export async function getRuntimeConfig() {
  if (cachedConfig) {
    return cachedConfig;
  }

  const response = await fetch("/config/runtime-config.json");
  if (!response.ok) {
    throw new Error("Cannot load runtime config");
  }
  cachedConfig = await response.json();
  return cachedConfig;
}
