// =========================
// Autosubmit för filter-formulär (input + select)
// =========================
document.querySelectorAll('form[data-autosubmit="true"]').forEach((form) => {
  const input = form.querySelector(
    'input[type="search"], input[name="q"], input[type="text"]'
  );
  const select = form.querySelector("select");

  let timeout;

  if (input) {
    input.addEventListener("input", () => {
      clearTimeout(timeout);
      timeout = setTimeout(() => form.submit(), 250);
    });

    input.addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        e.preventDefault();
        form.submit();
      }
    });
  }

  if (select) {
    select.addEventListener("change", () => form.submit());
  }
});

// =========================
// AJAX-sök i topbaren (uppdaterar #productResults om det finns)
// =========================
(() => {
  const form = document.querySelector(".topbar-search");
  if (!form) return;

  const input = form.querySelector('input[name="q"]');
  if (!input) return;

  let timeout;

  const run = async () => {
    const url = new URL(form.action || window.location.href, window.location.origin);

    const fd = new FormData(form);
    for (const [k, v] of fd.entries()) {
      const val = String(v ?? "").trim();
      if (val.length) url.searchParams.set(k, val);
      else url.searchParams.delete(k);
    }

    const res = await fetch(url.toString(), {
      headers: { "X-Requested-With": "fetch" },
    });

    if (!res.ok) {
      window.location.href = url.toString();
      return;
    }

    const html = await res.text();
    const doc = new DOMParser().parseFromString(html, "text/html");

    const next = doc.querySelector("#productResults");
    const current = document.querySelector("#productResults");

    // Fallback: om sidan inte har #productResults -> normal navigation
    if (!(next && current)) {
      window.location.href = url.toString();
      return;
    }

    current.replaceWith(next);
    history.replaceState(null, "", url.toString());

    // Behåll fokus och caret i input
    input.focus();
    const len = input.value.length;
    input.setSelectionRange(len, len);
  };

  input.addEventListener("input", () => {
    clearTimeout(timeout);
    timeout = setTimeout(run, 350);
  });

  input.addEventListener("keydown", (e) => {
    if (e.key === "Enter") {
      e.preventDefault();
      clearTimeout(timeout);
      run();
    }
  });

  form.addEventListener("submit", (e) => {
    e.preventDefault();
    clearTimeout(timeout);
    run();
  });
})();

// =========================
// Live uppdatering av kundvagn
// =========================
document.querySelectorAll(".cart-qty").forEach((input) => {
  let timeout;

  const update = async () => {
    const productId = Number(input.dataset.productId);
    const quantity = Number(input.value);

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const res = await fetch("/Cart/UpdateQuantity", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...(token ? { RequestVerificationToken: token } : {}),
      },
      body: JSON.stringify({ productId, quantity }),
    });

    if (!res.ok) return;

    const data = await res.json();

    // uppdatera radtotal
    const row = input.closest(".cart-row");
    const lineTotalEl = row?.querySelector(".cart-line-total");
    if (lineTotalEl) lineTotalEl.textContent = data.lineTotal;

    // uppdatera totalsumma
    const totalEl = document.querySelector("#cartTotal");
    if (totalEl) totalEl.textContent = data.cartTotal;

    // uppdatera ikon i header
    const badge = document.querySelector(".cart-count");
    if (badge) badge.textContent = data.cartCount;
  };

  const debouncedUpdate = () => {
    clearTimeout(timeout);
    timeout = setTimeout(update, 300);
  };

  input.addEventListener("input", debouncedUpdate);
  input.addEventListener("change", update);
});

// =========================
// DOMContentLoaded: toasts + HERO ping-pong + scroll-fix
// =========================
document.addEventListener("DOMContentLoaded", () => {
  // Toasts
  const cartToast = document.getElementById("cartToast");
  if (cartToast && window.bootstrap?.Toast) {
    new bootstrap.Toast(cartToast, { delay: 2000 }).show();
  }

  const adminToast = document.getElementById("adminToast");
  if (adminToast && window.bootstrap?.Toast) {
    new bootstrap.Toast(adminToast, { delay: 2000 }).show();
  }

  // =========================
  // HERO ping-pong (stabil): INGEN nested DOMContentLoaded
  // =========================
  const a = document.getElementById("heroVideoA");
  const b = document.getElementById("heroVideoB");

  if (a && b) {
    [a, b].forEach((v) => {
      v.muted = true;
      v.playsInline = true;
      v.preload = "auto";
      v.setAttribute("muted", "");
      v.setAttribute("playsinline", "");
    });

    let active = a;
    let next = b;
    let swapping = false;

    const SWAP_BEFORE_END = 0.40; // sekunder innan slut

    const safePlay = async (v) => {
      try {
        const p = v.play();
        if (p && typeof p.then === "function") await p;
        return true;
      } catch (e) {
        console.warn("[hero] play failed", e);
        return false;
      }
    };

    // värm nästa så den har en frame redo (minskar svart-blink)
    const prepareNext = async (v) => {
      v.pause();
      try { v.currentTime = 0; } catch {}
      await safePlay(v);
      v.pause();
      return true;
    };

    const swap = async () => {
      if (swapping) return;
      swapping = true;

      await prepareNext(next);

      // byt visuellt
      active.classList.remove("is-active");
      next.classList.add("is-active");

      const ok = await safePlay(next);
      if (!ok) {
        // om play failar -> revert så vi inte blir svart
        next.classList.remove("is-active");
        active.classList.add("is-active");
        swapping = false;
        return;
      }

      // reset gamla
      active.pause();
      try { active.currentTime = 0; } catch {}

      // swap refs
      const tmp = active;
      active = next;
      next = tmp;

      swapping = false;
    };

    // Starta första
    safePlay(a);

    // Byt innan slut (timeupdate)
    const tick = () => {
      if (swapping) return;
      const d = active.duration;
      const t = active.currentTime;
      if (d && isFinite(d) && (d - t) < SWAP_BEFORE_END) swap();
    };

    a.addEventListener("timeupdate", tick);
    b.addEventListener("timeupdate", tick);

    // autoplay fallback (om browsern kräver user gesture)
    window.addEventListener("pointerdown", () => safePlay(active), { once: true });
  }

  // =========================
  // Scroll-fix: behåll position efter "lägg i varukorgen" (PRG reload)
  // =========================
  const scrollKey = `scrollPos:${window.location.pathname}${window.location.search}`;

  const saved = sessionStorage.getItem(scrollKey);
  if (saved) {
    const y = Number(saved);
    if (!Number.isNaN(y)) window.scrollTo(0, y);
    sessionStorage.removeItem(scrollKey);
  }

  document.addEventListener(
    "submit",
    (e) => {
      const form = e.target;
      if (!(form instanceof HTMLFormElement)) return;
      if (!form.classList.contains("product-add")) return;

      sessionStorage.setItem(scrollKey, String(window.scrollY || 0));

      // Säkerställ att returnUrl är nuvarande vy (path + query)
      const returnUrl = form.querySelector('input[name="returnUrl"]');
      if (returnUrl) {
        returnUrl.value = `${window.location.pathname}${window.location.search}`;
      }
    },
    true
  );
});
