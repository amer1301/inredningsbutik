document.querySelectorAll('form[data-autosubmit="true"]').forEach(form => {
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

    input.addEventListener("keydown", e => {
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
      headers: { "X-Requested-With": "fetch" }
    });

    if (!res.ok) return;

    const html = await res.text();
    const doc = new DOMParser().parseFromString(html, "text/html");

    const next = doc.querySelector("#productResults");
    const current = document.querySelector("#productResults");

    if (next && current) {
      current.replaceWith(next);

      history.replaceState(null, "", url.toString());

      input.focus();
      const len = input.value.length;
      input.setSelectionRange(len, len);
    }
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
