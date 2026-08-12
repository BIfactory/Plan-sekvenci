import { useEffect, useState } from "react";
import { api } from "../api/client";
import type { Me } from "../api/types";

// PRD 3: appku vidi vsichni, editace jen pro "approvery" (AD skupina nebo whitelist).
// Server je autoritativni zdroj pravdy - tohle jen rika UI, ktere ovladaci prvky nabidnout.
export function useMe() {
  const [me, setMe] = useState<Me | null>(null);

  useEffect(() => {
    let cancelled = false;
    api.me().then((result) => {
      if (!cancelled) setMe(result);
    });
    return () => {
      cancelled = true;
    };
  }, []);

  return me;
}
