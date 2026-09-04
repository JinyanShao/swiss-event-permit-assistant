const draftKey = "sepa.assessmentDraft.v1";
const resultKey = "sepa.lastAssessment.v1";

document.querySelectorAll("[data-analytics-event]").forEach((link) => {
  link.addEventListener("click", () => {
    if (typeof window.plausible === "function") {
      window.plausible(link.dataset.analyticsEvent);
    }
  });
});

const form = document.querySelector("#assessmentForm");

if (form) {
  const steps = Array.from(form.querySelectorAll(".question-step"));
  const previous = form.querySelector("#previousStep");
  const next = form.querySelector("#nextStep");
  const submit = form.querySelector("#submitAssessment");
  const counter = form.querySelector("#stepCounter");
  const title = form.querySelector("#stepTitle");
  const progress = form.querySelector("#progressBar");
  const hiddenJson = form.querySelector("#assessmentJson");
  let currentStep = 0;

  restoreDraft();
  if (selectedCommune() === "VilleDeFribourg") {
    currentStep = Number(sessionStorage.getItem("sepa.currentStep") || "0");
  }

  showStep(currentStep, false);
  refreshConditionals();

  form.addEventListener("change", () => {
    refreshConditionals();
    saveDraft();
  });

  form.addEventListener("input", saveDraft);

  previous?.addEventListener("click", () => {
    showStep(currentStep - 1);
  });

  next?.addEventListener("click", () => {
    if (validateStep()) {
      if (currentStep === 0 && selectedCommune() !== "VilleDeFribourg") {
        form.requestSubmit();
        return;
      }

      showStep(currentStep + 1);
    }
  });

  form.addEventListener("submit", (event) => {
    if (!validateStep()) {
      event.preventDefault();
      return;
    }

    const payload = currentStep === 0 && selectedCommune() !== "VilleDeFribourg"
      ? collectScopePayload()
      : collectPayload();
    const json = JSON.stringify(payload);
    hiddenJson.value = json;
    localStorage.setItem(draftKey, json);
    localStorage.setItem(resultKey, json);
  });

  function showStep(index, moveFocus = true) {
    currentStep = Math.max(0, Math.min(index, steps.length - 1));
    steps.forEach((step, stepIndex) => {
      const active = stepIndex === currentStep;
      step.classList.toggle("is-active", active);
      step.setAttribute("aria-hidden", String(!active));
    });

    counter.textContent = `Étape ${currentStep + 1} sur ${steps.length}`;
    title.textContent = steps[currentStep].dataset.title;
    progress.style.width = `${((currentStep + 1) / steps.length) * 100}%`;
    previous.disabled = currentStep === 0;
    next.hidden = currentStep === steps.length - 1;
    submit.hidden = currentStep !== steps.length - 1;
    sessionStorage.setItem("sepa.currentStep", String(currentStep));
    if (moveFocus) {
      steps[currentStep].focus({ preventScroll: false });
    }
  }

  function validateStep() {
    const activeStep = steps[currentStep];
    const fields = Array.from(activeStep.querySelectorAll("[required]"));
    let valid = true;
    let firstInvalid = null;

    activeStep.querySelectorAll(".field-error.is-visible").forEach((error) => {
      error.classList.remove("is-visible");
    });
    activeStep.querySelectorAll("[aria-invalid='true']").forEach((field) => {
      field.setAttribute("aria-invalid", "false");
    });

    for (const field of fields) {
      if (field.type === "radio") {
        const group = activeStep.querySelectorAll(`input[name="${field.name}"]`);
        if (!Array.from(group).some((radio) => radio.checked)) {
          valid = false;
          group.forEach((radio) => radio.setAttribute("aria-invalid", "true"));
          field.closest(".field-group")?.querySelector(".field-error")?.classList.add("is-visible");
          firstInvalid ??= field;
        }
      } else if (!field.value) {
        valid = false;
        field.setAttribute("aria-invalid", "true");
        field.closest(".field")?.querySelector(".field-error")?.classList.add("is-visible");
        firstInvalid ??= field;
      }
    }

    firstInvalid?.focus();
    return valid;
  }

  function refreshConditionals() {
    form.querySelectorAll(".conditional").forEach((section) => {
      const rules = section.dataset.showWhen.split(",");
      const visible = rules.some((rule) => {
        const [name, value] = rule.split(":");
        const selected = form.querySelector(`[name="${name}"]:checked`);
        return selected?.value === value;
      });
      section.hidden = !visible;
    });
  }

  function collectPayload() {
    const data = new FormData(form);
    const text = (name) => data.get(name)?.toString() || null;
    const bool = (name) => data.get(name) === "on";
    const number = (name) => {
      const value = text(name);
      return value ? Number(value) : null;
    };

    return {
      eventName: text("eventName"),
      eventDate: text("eventDate"),
      expectedAttendance: number("expectedAttendance"),
      commune: text("commune") || "Unknown",
      venueKind: text("venueKind") || "NotSure",
      isPublicEvent: text("isPublicEvent") || "Unknown",
      beverageMode: text("beverageMode") || "NotSure",
      foodMode: text("foodMode") || "NotSure",
      alcoholMode: text("alcoholMode") || "NotSure",
      hasAmplifiedMusicOrSound: text("hasAmplifiedMusicOrSound") || "Unknown",
      eventEndTime: text("eventEndTime"),
      hasTemporaryInstallations: text("hasTemporaryInstallations") || "Unknown",
      affectsTrafficOrParking: text("affectsTrafficOrParking") || "Unknown",
      hasProcessionOrRoute: text("hasProcessionOrRoute") || "Unknown",
      isSportCompetitionOnPublicRoad: text("isSportCompetitionOnPublicRoad") || "Unknown",
      needsMunicipalMaterialOrDecorations: bool("needsMunicipalMaterialOrDecorations"),
      needsAdvertisingBannerOrPublicPosting: bool("needsAdvertisingBannerOrPublicPosting"),
      usesGasGrillOrHeater: text("usesGasGrillOrHeater") || "Unknown",
      hasLiabilityInsurance: text("hasLiabilityInsurance") || "Unknown"
    };
  }

  function collectScopePayload() {
    return {
      commune: selectedCommune()
    };
  }

  function selectedCommune() {
    return form.querySelector('[name="commune"]:checked')?.value || "Unknown";
  }

  function saveDraft() {
    localStorage.setItem(draftKey, JSON.stringify(collectPayload()));
  }

  function restoreDraft() {
    const saved = localStorage.getItem(draftKey) || localStorage.getItem(resultKey);
    if (!saved) return;

    const payload = JSON.parse(saved);
    for (const [name, value] of Object.entries(payload)) {
      const fields = form.querySelectorAll(`[name="${name}"]`);
      fields.forEach((field) => {
        if (field.type === "radio") {
          field.checked = field.value === value;
        } else if (field.type === "checkbox") {
          field.checked = Boolean(value);
        } else if (value !== null && value !== undefined) {
          field.value = value;
        }
      });
    }
  }
}
