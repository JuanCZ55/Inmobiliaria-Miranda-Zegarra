/**
 * ========================================================================
 * GESTIÓN DE IMÁGENES TOTALMENTE UNIFICADA PARA MODIFICAR
 * Versión Final
 * ========================================================================
 */

// --- FUNCIÓN GENERAL PARA MARCAR ITEMS PARA ELIMINAR ---
function marcarParaEliminar(id, containerId) {
  if (containerId) {
    const imgContainer = document.getElementById(containerId);
    if (imgContainer) {
      imgContainer.style.display = "none";
    }
  }
  const hiddenInputsContainer = document.getElementById(
    "container-eliminar-ids"
  );
  if (
    !hiddenInputsContainer ||
    document.querySelector(`input[name="EliminarIDs"][value="${id}"]`)
  ) {
    return;
  }
  const input = document.createElement("input");
  input.type = "hidden";
  input.name = "EliminarIDs";
  input.value = id;
  hiddenInputsContainer.appendChild(input);
}

/**
 * ========================================================================
 * LÓGICA PARA LA GESTIÓN DE PORTADA UNIFICADA
 * ========================================================================
 */
function inicializarPortadaUnificada(config) {
  const inputPortada = document.getElementById(config.idInputPortada);
  const previewContainer = document.getElementById(config.idPreviewContainer);
  const placeholder = document.getElementById(config.idPlaceholder);

  let portadaActual = null; // Nuestro objeto de estado para la portada

  // Carga inicial de la portada existente
  if (window.imagenPortadaExistente) {
    portadaActual = {
      id: window.imagenPortadaExistente.idImagen,
      url: window.imagenPortadaExistente.url,
      tipo: "existente",
      file: null,
    };
  }

  const renderizarPortada = () => {
    previewContainer.innerHTML = ""; // Limpia la vista

    if (portadaActual) {
      placeholder.style.display = "none";
      const div = document.createElement("div");
      div.className = "position-relative";
      div.id = `container-img-${portadaActual.id}`;

      const img = document.createElement("img");
      img.src = portadaActual.url;
      img.className = "d-block w-100";
      img.style.height = "300px";
      img.style.objectFit = "cover";
      img.style.borderRadius = "var(--bs-border-radius)";

      const btn = document.createElement("button");
      btn.innerHTML = "&times; ";
      btn.className = "btn btn-danger btn-sm position-absolute top-0 end-0 m-2";
      btn.style.zIndex = "99";
      btn.type = "button";
      btn.onclick = eliminarPortada;

      div.appendChild(img);
      div.appendChild(btn);
      previewContainer.appendChild(div);
    } else {
      placeholder.style.display = "block";
    }
  };

  const eliminarPortada = () => {
    if (!portadaActual) return;

    if (portadaActual.tipo === "existente") {
      marcarParaEliminar(portadaActual.id);
    } else {
      inputPortada.value = "";
      URL.revokeObjectURL(portadaActual.url);
    }

    portadaActual = null;
    renderizarPortada();
  };

  const manejarNuevaPortada = (event) => {
    const file = event.target.files[0];
    if (!file) {
      if (portadaActual && portadaActual.tipo === "nuevo") {
        eliminarPortada();
      }
      return;
    }

    if (portadaActual && portadaActual.tipo === "existente") {
      marcarParaEliminar(portadaActual.id);
    }

    portadaActual = {
      id: `new_${Date.now()}`,
      url: URL.createObjectURL(file),
      tipo: "nuevo",
      file: file,
    };
    renderizarPortada();
  };

  inputPortada.addEventListener("change", manejarNuevaPortada);
  renderizarPortada();
}

/**
 * ========================================================================
 * LÓGICA PARA LA GESTIÓN DE GALERÍA UNIFICADA
 * ========================================================================
 */
function inicializarGaleriaUnificada(config) {
  const inputGaleria = document.getElementById(config.idInputGaleria);
  const carousel = document.getElementById(config.idCarousel);
  const carouselInner = document.getElementById(config.idCarouselInner);

  let galeriaUnificada = [];

  if (window.imagenesExistentes && window.imagenesExistentes.length > 0) {
    galeriaUnificada = window.imagenesExistentes.map((img) => ({
      id: img.idImagen,
      url: img.url,
      tipo: "existente",
      file: null,
    }));
  }

  const renderizarGaleria = () => {
    carouselInner.innerHTML = "";
    galeriaUnificada.forEach((item, idx) => {
      const div = document.createElement("div");
      div.className = `carousel-item ${idx === 0 ? "active" : ""}`;
      const img = document.createElement("img");
      img.src = item.url;
      img.className = "d-block w-100";
      img.style.height = "300px";
      img.style.objectFit = "cover";
      const btn = document.createElement("button");
      btn.innerHTML = "&times;";
      btn.className = "btn btn-danger btn-sm position-absolute top-0 end-0 m-2";
      btn.style.zIndex = "99";
      btn.type = "button";
      btn.addEventListener("click", () => eliminarItem(item.id, item.tipo));
      div.appendChild(img);
      div.appendChild(btn);
      carouselInner.appendChild(div);
    });
    actualizarInputFile();
    carousel.style.display = galeriaUnificada.length > 0 ? "block" : "none";
    const controls = carousel.querySelectorAll(
      ".carousel-control-prev, .carousel-control-next"
    );
    controls.forEach((control) => {
      control.style.display = galeriaUnificada.length > 1 ? "flex" : "none";
    });
  };

  const agregarNuevosItems = (event) => {
    const nuevosArchivos = Array.from(event.target.files);
    nuevosArchivos.forEach((file) => {
      if (
        file.type.startsWith("image/") &&
        !galeriaUnificada.some(
          (f) =>
            f.file && f.file.name === file.name && f.file.size === file.size
        )
      ) {
        galeriaUnificada.push({
          id: `new_${Date.now()}_${file.name}`,
          url: URL.createObjectURL(file),
          tipo: "nuevo",
          file: file,
        });
      }
    });
    renderizarGaleria();
  };

  const eliminarItem = (id, tipo) => {
    const item = galeriaUnificada.find((i) => i.id === id);
    if (!item) return;
    if (tipo === "existente") {
      marcarParaEliminar(item.id);
    } else if (tipo === "nuevo") {
      URL.revokeObjectURL(item.url);
    }
    galeriaUnificada = galeriaUnificada.filter((i) => i.id !== id);
    renderizarGaleria();
  };

  const actualizarInputFile = () => {
    const dataTransfer = new DataTransfer();
    galeriaUnificada
      .filter((item) => item.tipo === "nuevo")
      .forEach((item) => dataTransfer.items.add(item.file));
    inputGaleria.files = dataTransfer.files;
  };

  inputGaleria.addEventListener("change", agregarNuevosItems);
  renderizarGaleria();
}
