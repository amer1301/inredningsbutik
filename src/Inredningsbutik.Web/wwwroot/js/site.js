// Autosubmit för filter-formulär (input + select)
document.querySelectorAll('form[data-autosubmit="true"]').forEach((form) => {
  const input = form.querySelector(
    'input[type="search"], input[name="q"], input[type="text"]'
  );
  const select = form.querySelector("select");

  let timeout;

  if (input) {
    input.addEventListener("input", () => {
      clearTimeout(timeout);
      timeout = setTimeout(() => {
        form.submit();
      }, 250);
    });

    input.addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        e.preventDefault();
        form.submit();
      }
    });
  }

  if (select) {
    select.addEventListener("change", () => {
      form.submit();
    });
  }
});

(() => {
  // AJAX-sök i topbaren (uppdaterar #productResults om det finns)
  const form = document.querySelector(".topbar-search");
  if (!form) return;

  const input = form.querySelector('input[name="q"]');
  if (!input) return;

  let timeout;

  const run = async () => {
    const url = new URL(
      form.action || window.location.href,
      window.location.origin
    );

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
      // Om fetch misslyckas, fallback till vanlig navigation
      window.location.href = url.toString();
      return;
    }

    const html = await res.text();
    const doc = new DOMParser().parseFromString(html, "text/html");

    const next = doc.querySelector("#productResults");
    const current = document.querySelector("#productResults");

    // ✅ Fallback: om sidan inte har #productResults (eller svaret saknar det),
    // gå till sök-URL:en som vanligt.
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

// Hero-video loopar fram och tillbaka
const video = document.getElementById("heroVideo");

if (video) {
  let reversing = false;

  video.addEventListener("ended", () => {
    reversing = true;
    reverse();
  });

  function reverse() {
    if (!reversing) return;

    video.currentTime -= 0.03;

    if (video.currentTime <= 0) {
      reversing = false;
      video.play();
      return;
    }

    requestAnimationFrame(reverse);
  }
}

// Live uppdatering av kundvagn
document.querySelectorAll(".cart-qty").forEach((input) => {
  let timeout;

  const update = async () => {
    const productId = input.dataset.productId;
    const quantity = Number(input.value);

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const res = await fetch("/Cart/UpdateQuantity", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...(token ? { "RequestVerificationToken": token } : {})
      },
      body: JSON.stringify({ productId: Number(productId), quantity })
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

document.addEventListener("DOMContentLoaded", () => {
  const cartToast = document.getElementById("cartToast");
  if (cartToast && window.bootstrap?.Toast) {
    new bootstrap.Toast(cartToast, { delay: 2000 }).show();
  }

  const adminToast = document.getElementById("adminToast");
  if (adminToast && window.bootstrap?.Toast) {
    new bootstrap.Toast(adminToast, { delay: 2000 }).show();
  }
});
