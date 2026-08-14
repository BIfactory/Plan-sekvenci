import { createContext, useContext } from "react";

// Cil pro portal v horni navigacni liste (viz App.tsx) - RefreshBar logicky patri
// dane strance (ma jeji handleRefresh/loading/stale stav), ale vizualne ma byt v
// horni liste napravo pred logy. Misto zvednuti celeho refresh stavu do App.tsx
// (ktery by pak musel znat vnitrnosti obou stranek) si strana jen "portne" svuj
// RefreshBar do DOM uzlu, ktery drzi App.
export const NavSlotContext = createContext<HTMLDivElement | null>(null);

export function useNavSlot() {
  return useContext(NavSlotContext);
}
