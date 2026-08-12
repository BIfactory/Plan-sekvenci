import { useEffect, useState } from "react";
import { readPersistedNodeFilters, writePersistedNodeFilters } from "../lib/filterCookie";

// Provoz/Dilna/Team Leader/Uzel/Skupina zdroju jsou spolecne pro obrazovky
// "Plan sekvenci" i "Vyhodnoceni" a prezivaji v cookie (platnost dle appsettings.json,
// viz hooks/useClientSettings.ts) - uzivatel je tak najde nastavene stejne i pri
// prepnuti stranky, dalsi pracovni den nebo po dovolene. Zmena filtru vyssi urovne
// kaskadove resetuje ty nizsi (stejne jako v puvodni appce).
export function usePersistedNodeFilters(cookieExpiryDays?: number) {
  const initial = readPersistedNodeFilters();
  const [plant, setPlantState] = useState(initial.plant);
  const [dept, setDeptState] = useState(initial.dept);
  const [teamLeader, setTeamLeaderState] = useState(initial.teamLeader);
  const [node, setNodeState] = useState(initial.node);
  const [rgid, setRgid] = useState(initial.rgid);

  useEffect(() => {
    writePersistedNodeFilters({ plant, dept, teamLeader, node, rgid }, cookieExpiryDays);
  }, [plant, dept, teamLeader, node, rgid, cookieExpiryDays]);

  const setPlant = (value: string) => {
    setPlantState(value);
    setDeptState("");
    setTeamLeaderState("");
    setNodeState("");
    setRgid("");
  };

  const setDept = (value: string) => {
    setDeptState(value);
    setTeamLeaderState("");
    setNodeState("");
    setRgid("");
  };

  const setTeamLeader = (value: string) => {
    setTeamLeaderState(value);
    setNodeState("");
    setRgid("");
  };

  const setNode = (value: string) => {
    setNodeState(value);
    setRgid("");
  };

  return { plant, setPlant, dept, setDept, teamLeader, setTeamLeader, node, setNode, rgid, setRgid };
}
