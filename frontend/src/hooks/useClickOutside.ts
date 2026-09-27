import { useEffect, type RefObject } from "react";

// Zavreni panelu kliknutim kamkoliv mimo nej (stejne chovani jako
// DetailOperacePanel, ktery ma vlastni fixed backdrop s onClick). SideListPanel
// zadny backdrop nema (je to jen doku dolni bocni panel, ne modal s prekryvem), takze
// misto pridavani vizualniho prekryvu se poslouchá primo na document - mousedown
// (ne click), aby se panel zavrel uz pri zacatku kliku mimo, standardni chovani pro
// "click outside" patterny (dropdowny, modaly).
export function useClickOutside<T extends HTMLElement>(ref: RefObject<T | null>, onOutsideClick: () => void) {
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        onOutsideClick();
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, [ref, onOutsideClick]);
}
