import { createClient } from "@supabase/supabase-js";
import { getRuntimeConfig } from "./runtime-config";

let client;

export async function getSupabaseClient() {
  if (client) {
    return client;
  }

  const config = await getRuntimeConfig();
  if (!config.supabaseUrl || !config.supabaseAnonKey) {
    throw new Error("Missing Supabase runtime settings");
  }

  client = createClient(config.supabaseUrl, config.supabaseAnonKey);
  return client;
}
