import { getSupabaseClient } from "./supabase-client";

const TABLES = {
  users: "NMK_User",
  projects: "NMK_Project",
  tasks: "NMK_Task",
  temporaryTasks: "NMK_Task_Temporary",
  leaves: "NMK_Leave",
  notifications: "NMK_Notify"
};

export async function loadAllData() {
  const supabase = await getSupabaseClient();
  const [users, projects, tasks, temporaryTasks, leaves, notifications] = await Promise.all([
    getRows(supabase, TABLES.users),
    getRows(supabase, TABLES.projects),
    getRows(supabase, TABLES.tasks),
    getRows(supabase, TABLES.temporaryTasks),
    getRows(supabase, TABLES.leaves),
    getRows(supabase, TABLES.notifications)
  ]);

  return { users, projects, tasks, temporaryTasks, leaves, notifications };
}

async function getRows(supabase, table) {
  const { data, error } = await supabase.from(table).select("*");
  if (error) {
    throw error;
  }
  return data || [];
}

export async function upsertRow(key, row) {
  const supabase = await getSupabaseClient();
  const table = TABLES[key];
  const { data, error } = await supabase.from(table).upsert(row).select().single();
  if (error) {
    throw error;
  }
  return data;
}

export async function deleteRow(key, id) {
  const supabase = await getSupabaseClient();
  const table = TABLES[key];
  const { error } = await supabase.from(table).delete().eq("id", id);
  if (error) {
    throw error;
  }
}

export async function subscribeRealtime(onEvent) {
  const supabase = await getSupabaseClient();
  const channel = supabase.channel("nmk-live");

  Object.values(TABLES).forEach((tableName) => {
    channel.on(
      "postgres_changes",
      { event: "*", schema: "public", table: tableName },
      (payload) => onEvent(tableName, payload)
    );
  });

  await channel.subscribe();
  return () => {
    supabase.removeChannel(channel);
  };
}
