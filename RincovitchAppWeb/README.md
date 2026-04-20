# RincovitchAppWeb

Vanilla JavaScript rewrite of `RincovitchApp` using Web Components.

## Run

```bash
npm install
npm run dev
```

## Build

```bash
npm run build
```

## Runtime config

Update `public/config/runtime-config.json` with a valid Supabase anon key.

## Implemented parity slices

- App shell and role-based navigation
- Core modules: Dashboard, Timeline, Email, Users, Projects, Temporary, Schedule, Notify, Leave, Settings
- Theme tokens and light/dark switcher
- Supabase read model for User/Project/Task/Temporary/Leave/Notify
- Realtime update reconciliation for the same entities
- Timeline/Schedule synchronized scroll columns

## Desktop-only features replaced

- Tray icon and toast activation removed
- Outlook COM and registry integrations removed (web-safe only)
- Local app settings replaced with `localStorage`

## Regression checklist

- [ ] Role visibility matrix (user/leader/middle/admin)
- [ ] Module routing and navigation state
- [ ] Theme switching
- [ ] Supabase read and realtime update
- [ ] Timeline and schedule scroll sync
- [ ] CRUD hooks for each entity table
