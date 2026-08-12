import { NavLink, Route, Routes, Navigate } from "react-router-dom";
import PlanSekvenciPage from "./pages/PlanSekvenciPage";
import VyhodnoceniPage from "./pages/VyhodnoceniPage";
import czLogo from "./assets/cz-logo.png";
import bifactoryLogo from "./assets/bifactory-logo.png";

export default function App() {
  return (
    <div className="app">
      <nav className="app-nav">
        <NavLink to="/plan-sekvenci">Plán sekvencí</NavLink>
        <NavLink to="/vyhodnoceni">Vyhodnocení</NavLink>
        <div className="app-nav-logos">
          <img src={czLogo} alt="CZ" className="app-nav-logo" />
          <img src={bifactoryLogo} alt="BIfactory" className="app-nav-logo app-nav-logo-vendor" />
        </div>
      </nav>
      <main className="app-content">
        <Routes>
          <Route path="/" element={<Navigate to="/plan-sekvenci" replace />} />
          <Route path="/plan-sekvenci" element={<PlanSekvenciPage />} />
          <Route path="/vyhodnoceni" element={<VyhodnoceniPage />} />
        </Routes>
      </main>
    </div>
  );
}
