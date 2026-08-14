import { useState } from "react";
import { NavLink, Route, Routes, Navigate } from "react-router-dom";
import PlanSekvenciPage from "./pages/PlanSekvenciPage";
import VyhodnoceniPage from "./pages/VyhodnoceniPage";
import { NavSlotContext } from "./hooks/useNavSlot";
import czLogo from "./assets/cz-logo.png";
import bifactoryLogo from "./assets/bifactory-logo.png";

export default function App() {
  // Cil pro portal RefreshBar dane stranky (viz hooks/useNavSlot.ts) - drzi se ve
  // state (ne jen ref), aby zmena z null na skutecny DOM uzel po prvnim renderu
  // vyvolala re-render a stranky se do nej stihly naportovat.
  const [navSlot, setNavSlot] = useState<HTMLDivElement | null>(null);

  return (
    <div className="app">
      <nav className="app-nav">
        <NavLink to="/plan-sekvenci">Plán sekvencí</NavLink>
        <NavLink to="/vyhodnoceni">Vyhodnocení</NavLink>
        <div className="app-nav-refresh" ref={setNavSlot} />
        <div className="app-nav-logos">
          <a
            href="https://app.powerbi.com/groups/54640da3-46d2-4778-a679-7b5491b5a25c/reports/d96fb3a5-80bb-4054-aa36-961ef4776c76/ReportSection4393ebd248e268687447?experience=power-bi"
            target="_blank"
            rel="noopener noreferrer"
          >
            <img src={czLogo} alt="CZ" className="app-nav-logo" />
          </a>
          <img src={bifactoryLogo} alt="BIfactory" className="app-nav-logo app-nav-logo-vendor" />
        </div>
      </nav>
      <main className="app-content">
        <NavSlotContext.Provider value={navSlot}>
          <Routes>
            <Route path="/" element={<Navigate to="/plan-sekvenci" replace />} />
            <Route path="/plan-sekvenci" element={<PlanSekvenciPage />} />
            <Route path="/vyhodnoceni" element={<VyhodnoceniPage />} />
          </Routes>
        </NavSlotContext.Provider>
      </main>
    </div>
  );
}
